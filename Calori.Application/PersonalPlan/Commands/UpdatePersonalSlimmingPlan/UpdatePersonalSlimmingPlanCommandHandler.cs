using System;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan
{
    public class UpdatePersonalSlimmingPlanCommandHandler 
        : IRequestHandler<UpdatePersonalSlimmingPlanCommand>
    {
        private readonly ICaloriDbContext _dbContext;

        public UpdatePersonalSlimmingPlanCommandHandler(ICaloriDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Unit> Handle(UpdatePersonalSlimmingPlanCommand request,
            CancellationToken cancellationToken)
        {
            var calculatorQuery = new CreatePersonalSlimmingPlanCommand
            {
                Weight = request.Weight,
                CaloricNeeds = request.CaloricNeeds,
                Gender = request.Gender,
                Goal = request.Goal,
                CaloriActivityLevel = request.CaloriActivityLevel
            };
            
            var weeksToTarget = CalculateSlimmingPlanWeeks(calculatorQuery);

            var entity = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(p => 
                    p.Id == request.Id, cancellationToken);

            entity.StartDate = new DateTime(2024, 1, 8);
            entity.FinishDate = new DateTime(2024, 1, 8).AddDays(weeksToTarget * 7);
            entity.WeekNumber = 0;
            entity.CurrentWeight = request.Weight;
            entity.CaloricNeeds = request.CaloricNeeds;
            entity.Goal = request.Goal;
            entity.CaloriSlimmingPlanId = request.CaloriSlimmingPlanId;
            entity.CurrentWeekDeficit = request.CurrentWeekDeficit;
            entity.BurnedThisWeek = request.BurnedThisWeek;
            entity.TotalBurned = request.TotalBurned;
            entity.WeeksToTarget = weeksToTarget - 1;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
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
    }
}