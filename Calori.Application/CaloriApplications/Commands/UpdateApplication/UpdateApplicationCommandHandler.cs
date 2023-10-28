using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.CalculatorService;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Commands.UpdateApplication
{
    public class UpdateApplicationCommandHandler 
        : IRequestHandler<UpdateApplicationCommand>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        protected IMediator _mediator;
        
        public UpdateApplicationCommandHandler(ICaloriDbContext dbContext, IMapper mapper, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var gender = request.Gender;
            var weight = request.Weight;
            var height = request.Height;
            var age = request.Age;
            var goal = request.Goal;
            var email = request.Email;
            var activity = request.Activity;
            var allergies = request.Allergies;
            var anotherAllergy = request.AnotherAllergy;
            
            var calculatorRequest = new CreateCaloriApplicationCommand
            {
                Gender = request.Gender,
                Weight = request.Weight,
                Height = request.Height,
                Age = request.Age,
                Goal = request.Goal,
                Email = request.Email,
                Activity = request.Activity,
                Allergies = request.Allergies,
                AnotherAllergy = request.AnotherAllergy
            };

            var calculator = new ApplicationCalculator();
            var calculated = calculator.CalculateApplicationParameters(calculatorRequest);

            var entity = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            var bodyParameters = await _dbContext.ApplicationBodyParameters
                .FirstOrDefaultAsync(p => 
                    p.Id == entity.ApplicationBodyParametersId, cancellationToken);
            
            bodyParameters.MinWeight = calculated.MinWeight;
            bodyParameters.MaxWeight = calculated.MaxWeight;
            bodyParameters.BMI = calculated.BMI;
            bodyParameters.BMR = calculated.BMR;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            var offset = 0.0m;

            switch (activity)
            {
                case CaloriActivityLevel.Inactive:
                    offset = 1.2m;
                    break;
                case CaloriActivityLevel.Light:
                    offset = 1.375m;
                    break;
                case CaloriActivityLevel.Moderate:
                    offset = 1.55m;
                    break;
            }
            
            var dailyCalories = (int)(calculated.BMR * offset ?? 0);
        
            var closestRation = CalculateTargetRation(dailyCalories);

            entity.GenderId = gender;
            entity.Weight = weight;
            entity.Height = height;
            entity.Age = age;
            entity.Goal = goal;
            entity.Email = email;
            entity.ActivityLevelId = activity;
            entity.CreatedAt = DateTime.UtcNow;
            entity.AnotherAllergy = anotherAllergy;
            entity.ApplicationBodyParametersId = bodyParameters.Id;
            entity.DailyCalories = dailyCalories;
            entity.Ration = closestRation;
            entity.UpdatedAt = DateTime.UtcNow;
            
            var deficitOnWeek = dailyCalories - closestRation;
            var burnedOnWeek = (deficitOnWeek * 1000) / 8000;

            var caloriSlimmingPlan = await _dbContext.CaloriSlimmingPlan
                .Where(p => p.Calories == closestRation)
                .FirstOrDefaultAsync(cancellationToken);

            var planCreateCommand = new UpdatePersonalSlimmingPlanCommand
            {
                Id = caloriSlimmingPlan.Id,
                WeekNumber = 1,
                Weight = weight,
                CaloricNeeds = dailyCalories,
                Goal = goal,
                CurrentWeekDeficit = deficitOnWeek,
                BurnedThisWeek = burnedOnWeek,
                TotalBurned = 0,
                CaloriSlimmingPlanId = caloriSlimmingPlan.Id,
                Gender = gender,
                CurrentCalori = closestRation,
                CaloriActivityLevel = activity
            };
            
            var command = _mapper.Map<UpdatePersonalSlimmingPlanCommand>(planCreateCommand);
            await _mediator.Send(command);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (allergies != null)
            {
                foreach (var allergyId in allergies)
                {
                    var allergy = await _dbContext.ApplicationAllergies
                        .FirstOrDefaultAsync(p => 
                            p.ApplicationId == entity.Id, cancellationToken);

                    allergy.Allergy = (Allergies)allergyId;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
        
        private int CalculateTargetRation(int currentCalori)
        {
            var rations = new[] { 1250, 1500, 1750, 2000, 2250, 2500 };
            // var maxDifferent = gender == 0 ? 700 : 600;
            var minCalori = currentCalori - 700;
            var maxCalori = currentCalori - 450;
            var targetRation = 0;

            foreach (var ration in rations)
            {
                var inRange = minCalori <= ration &&
                              ration <= maxCalori;

                if (inRange)
                {
                    targetRation = ration;
                }
            }

            return targetRation;
        }
    }
}