using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan
{
    public class GetAutoCalculatedPlanQueryHandler
        : IRequestHandler<GetAutoCalculatedPlanQuery, AutoCalculatedPlanVm>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager; 
        
        public GetAutoCalculatedPlanQueryHandler(ICaloriDbContext dbContext,
            IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        public async Task<AutoCalculatedPlanVm> Handle(GetAutoCalculatedPlanQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: DateTime.Today сменить на request.CurrentDate
            var currentDate = request.CurrentDate;
            // TODO: DateTime.Today сменить на request.CurrentDate
            
            var application = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
            
            var personalPlan = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(x => 
                    x.Id == application.PersonalSlimmingPlanId, cancellationToken);
            
            if (personalPlan == null)
            {
                return new AutoCalculatedPlanVm();
            }

            var userPayments = await _dbContext.UserPayments
                .FirstOrDefaultAsync(x => x.UserId == application.UserId, cancellationToken);

            if (personalPlan.SubscriptionStatus == SubscriptionStatus.Canceled)
            {
                return new AutoCalculatedPlanVm { Message = $"No active plan found. Status of last plan {personalPlan.SubscriptionStatus}" };
            }
            
            if (personalPlan.StartDate == null || 
                personalPlan.StartDate > currentDate)
            {
                var plan = new AutoCalculatedPlanVm();
                plan.Status = SubscriptionStatus.NotStarted;
                if (userPayments == null)
                {
                    plan.IsPaid = false;
                }
                else
                {
                    plan.IsPaid = userPayments.IsPaid;
                }
                
                plan.StartDate = personalPlan.StartDate.Value;
                plan.FinishDate = personalPlan.FinishDate.Value;
                plan.Message = "Your personal plan hasn't started yet.";
                
                return plan;
            }
            
            if (personalPlan.FinishDate != null && 
                currentDate > (DateTime)personalPlan.FinishDate)
            {
                return new AutoCalculatedPlanVm { Message = "Your personal plan is complete." };
            }
            
            if (personalPlan.SubscriptionStatus == SubscriptionStatus.Paused)
            {
                if (personalPlan.PausedAt != null) currentDate = personalPlan.PausedAt.Value;
            }
            
            if (personalPlan.SubscriptionStatus == SubscriptionStatus.Resumed)
            {
                if (personalPlan.PausedAt != null) currentDate = personalPlan.PausedAt.Value;
            }
            
            int currentWeekNumber = CalculateWeekNumber(
                (DateTime)personalPlan.StartDate, currentDate);
            
            var calcModel = new CalculateSlimmingPlanModel
            {
                CurrentWeek = currentWeekNumber,
                Weight = (decimal)application.Weight!,
                CaloriNeeds = (int)application.DailyCalories!,
                Gender = (int)application.GenderId!,
                Goal = (int)application.Goal!,
                CaloriActivityLevel = (CaloriActivityLevel)application.ActivityLevelId!
            };

            var daysToChangePlanFromStartDate = CalculateDaysToChangePlan(calcModel) - 1;
            var daysLeft = (currentDate - personalPlan.StartDate).Value.Days;
            int daysToChangePlan = daysToChangePlanFromStartDate - (currentDate - personalPlan.StartDate).Value.Days;
            
            var currentPlanParameters = CalculateSlimmingPlan(calcModel);

            var personalPlanId = application.PersonalSlimmingPlanId;
            var weightLost = currentPlanParameters.TotalFatBurned;
            var hoursSaved = daysLeft * 0.5;
            var startDate = personalPlan.StartDate;
            var finishDate = personalPlan.FinishDate;

            var currentCaloriPlan = await _dbContext.CaloriSlimmingPlan
                .FirstOrDefaultAsync(x => 
                    x.Id == personalPlan.CaloriSlimmingPlanId, cancellationToken);

            var entity = new AutoCalculatedPlanVm();

            var userPayment = await _dbContext.UserPayments
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            entity.PersonalPlanId = (int)personalPlanId!;
            entity.WeightLost = (double)weightLost!;
            entity.HoursSaved = hoursSaved!;
            entity.DaysToChangePlan = daysToChangePlan!;
            entity.DaysLeft = daysLeft!;
            entity.StartDate = (DateTime)startDate!;
            entity.FinishDate = (DateTime)finishDate!;
            entity.CurrentCaloriPlan = currentCaloriPlan;
            entity.Message = string.Empty;
            if (personalPlan.SubscriptionStatus != null) entity.Status = personalPlan.SubscriptionStatus.Value;

            if (userPayment == null)
            {
                entity.IsPaid = false;
            }
            else
            {
                entity.IsPaid = userPayment.IsPaid;
            }

            return _mapper.Map<AutoCalculatedPlanVm>(entity);
        }
        
        static int CalculateWeekNumber(DateTime startDate, DateTime currentDate)
        {
            int daysDifference = (int)(currentDate - startDate).TotalDays;
            
            int weekNumber = daysDifference / 7 + 1;
        
            return weekNumber;
        }
        
        private CalculateSlimmingResult CalculateSlimmingPlan(CalculateSlimmingPlanModel request)
        {
            var virtualWeek = 0;
            var weight = request.Weight * 1.00m;
            var lossWeigth = 0.00m;
            var caloriNeeds = request.CaloriNeeds;
            var eats = 0;
            var deficitOnWeek = 0.00m;
            var burnedOnWeek = 0.00m;
            var gender = (int)request.Gender!;
            var activity = request.CaloriActivityLevel;
            
            while (virtualWeek != request.CurrentWeek)
            {
                caloriNeeds = CalculateDailyCalories(weight, 165, 25, gender, activity);
                eats = CalculateTargetRation((int)caloriNeeds);
                deficitOnWeek = ((int)caloriNeeds - eats) * 7;
                burnedOnWeek = 1000 * deficitOnWeek / 8000;
                weight -= burnedOnWeek / 1000;
                lossWeigth += burnedOnWeek / 1000;
                virtualWeek++;
            }

            var result = new CalculateSlimmingResult
            {
                CurrentPlanCalories = eats,
                DeficitOnCurrentWeek = deficitOnWeek,
                FatBurnedOnCurrentWeek = burnedOnWeek,
                CurrentWeight = weight,
                TotalFatBurned = lossWeigth
            };

            return result;
        }
        
        private int CalculateDaysToChangePlan(CalculateSlimmingPlanModel request)
        {
            var days = 0;
            var weight = request.Weight * 1.00m;
            var lossWeigth = 0.00m;
            var caloriNeeds = request.CaloriNeeds;
            var eats = 0;
            var deficitOnDay = 0.00m;
            var burnedOnDay = 0.00m;
            var gender = (int)request.Gender!;
            var goal = request.Goal;
            var activity = request.CaloriActivityLevel;
            int counter = 0;
            
            
            var startCaloricNeeds = CalculateDailyCalories(weight, 165, 25, gender, activity);
            var startRation  = CalculateTargetRation(startCaloricNeeds);
            
            while (weight > goal)
            {
                caloriNeeds = CalculateDailyCalories(weight, 165, 25, gender, activity);
                eats = CalculateTargetRation(caloriNeeds);
                deficitOnDay = caloriNeeds - eats;
                burnedOnDay = 1000 * deficitOnDay / 8000;
                weight -= burnedOnDay / 1000;
                lossWeigth += burnedOnDay / 1000;
                days++;

                if (eats < startRation)
                {
                    break;
                }
            }
            
            return days;
        }


        public static int CalculateDailyCalories(
            decimal? Weight, decimal? Height, int age, int gender, CaloriActivityLevel activity)
        {
            if (Height == null || Weight == null)
            {
                throw new ArgumentNullException(
                    $"Incorrect data for: {nameof(Height)} & {nameof(Weight)}");
            }

            var offset = activity switch
            {
                CaloriActivityLevel.Inactive => 1.2m,
                CaloriActivityLevel.Light => 1.375m,
                CaloriActivityLevel.Moderate => 1.55m,
                _ => 0.0m
            };
            
                decimal heightM = Height.Value / 100.0m;

                decimal? bmr = (gender == 0)
                    ? (66.47m + (13.75m * Weight.Value) + (5.0m * (heightM * 100)) - (6.74m * age))!
                    : (655.1m + (9.6m * Weight.Value) + (1.85m * (heightM * 100)) - (4.68m * age))!;

                var dailyCalories = (int)(bmr * offset ?? 0);

                return dailyCalories;
        }

        private static int CalculateTargetRation(int currentCalori)
        {
            var rations = new[] { 1250, 1500, 1750, 2000, 2250, 2500, };

            var minCaloric = currentCalori - 700;
            var maxCaloric = currentCalori - 450;

            var targetRation = 0;

            foreach (var ration in rations)
            {
                var inRange = minCaloric <= ration &&
                              ration <= maxCaloric;

                if (inRange)
                {
                    targetRation = ration;
                }
            }

            return targetRation;
        }
        
        class CalculateSlimmingResult
        {
            public int CurrentPlanCalories { get; set; }
            public decimal DeficitOnCurrentWeek { get; set; }
            public decimal FatBurnedOnCurrentWeek { get; set; }
            public decimal CurrentWeight { get; set; }
            public decimal TotalFatBurned { get; set; }
        }
    }
}