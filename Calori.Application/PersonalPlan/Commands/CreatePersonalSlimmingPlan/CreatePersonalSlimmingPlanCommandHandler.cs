using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Enums;
using MediatR;

namespace Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan
{
    public class CreatePersonalSlimmingPlanCommandHandler
        : IRequestHandler<CreatePersonalSlimmingPlanCommand, PersonalSlimmingPlan>
    {
        private readonly ICaloriDbContext _dbContext;

        public CreatePersonalSlimmingPlanCommandHandler(ICaloriDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<PersonalSlimmingPlan> Handle(CreatePersonalSlimmingPlanCommand request,
            CancellationToken cancellationToken)
        {
            var weeksToTarget = CalculateSlimmingPlanWeeks(request);

            var personalSlimmingPlan = new PersonalSlimmingPlan
            {
                // TODO: ONLY TEST
                // StartDate = new DateTime(2024, 1, 8),
                
                StartDate = new DateTime(2024, 1, 8),
                // TODO: ONLY TEST
                
                // TODO: ONLY TEST
                //FinishDate = new DateTime(2024, 1, 8).AddDays(weeksToTarget * 7),
                FinishDate = new DateTime(2024, 1, 8).AddDays(weeksToTarget * 7),
                // TODO: ONLY TEST
                
                WeekNumber = 0,
                CurrentWeight = request.Weight,
                CaloricNeeds = request.CaloricNeeds,
                Goal = request.Goal,
                CaloriSlimmingPlanId = request.CaloriSlimmingPlanId,
                CurrentWeekDeficit = request.CurrentWeekDeficit,
                BurnedThisWeek = request.BurnedThisWeek,
                TotalBurned = request.TotalBurned,
                WeeksToTarget = weeksToTarget - 1,
                CreatedAt = DateTime.UtcNow,
                SubscriptionStatus = SubscriptionStatus.NotStarted
            };

            if (personalSlimmingPlan.StartDate < DateTime.Now)
            {
                personalSlimmingPlan.StartDate = DateTime.UtcNow;
                personalSlimmingPlan.FinishDate = DateTime.UtcNow.AddDays(weeksToTarget * 7);
            }

            _dbContext.PersonalSlimmingPlan.Add(personalSlimmingPlan);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return personalSlimmingPlan;
        }

        private int CalculateSlimmingPlanWeeks(CreatePersonalSlimmingPlanCommand request)
        {
            var week = 1;
            var weight = request.Weight * 1.00m;
            var lossWeigth = 0.00m;
            var caloriNeeds = request.CaloricNeeds!;
            var eats = 0;
            var deficitOnWeek = 0.00m;
            var burnedOnWeek = 0.00m;
            var gender = (int)request.Gender!;
            var goal = request.Goal;
            var activity = (CaloriActivityLevel)request.CaloriActivityLevel;

            while (weight > goal)
            {
                caloriNeeds = CalculateDailyCalories(weight, 165, 25, gender, activity);
                eats = CalculateTargetRation((int)caloriNeeds);
                deficitOnWeek = ((int)caloriNeeds - eats) * 7;
                burnedOnWeek = 1000 * deficitOnWeek / 8000;
                weight -= burnedOnWeek / 1000;
                lossWeigth += burnedOnWeek / 1000;
                week++;
            }

            return week;
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

            // 3000
            
            var minCaloric = currentCalori - 700; // 2300
            var maxCaloric = currentCalori - 450; // 2550

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
        
        // int CalculateTargetRation(int gender, int? currentCalori)
        // {
        //     var rations = new[] { 1250, 1500, 1750, 2000, 2250, 2500 };
        //     var maxDifferent = gender == 0 ? 700 : 600;
        //     var minCalori = currentCalori - maxDifferent;
        //     var maxCalori = currentCalori - 450;
        //     var targetRation = 0;
        //
        //     foreach (var ration in rations)
        //     {
        //         var inRange = minCalori <= ration &&
        //                       ration <= maxCalori;
        //
        //         if (inRange)
        //         {
        //             targetRation = ration;
        //         }
        //     }
        //
        //     return targetRation;
        // }
    }
}