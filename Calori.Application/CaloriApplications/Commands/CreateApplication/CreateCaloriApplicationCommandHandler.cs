using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.CalculatorService;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Commands.CreateApplication
{
    public class CreateCaloriApplicationCommandHandler 
        : IRequestHandler<CreateCaloriApplicationCommand, CaloriApplication>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        protected IMediator _mediator;

        public CreateCaloriApplicationCommandHandler(ICaloriDbContext dbContext, IMapper mapper, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<CaloriApplication> Handle(CreateCaloriApplicationCommand request,
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

            var calculator = new ApplicationCalculator();
            var calculated = calculator.CalculateApplicationParameters(request);

            // TODO: remove from db
            var appBodyParameters = new ApplicationBodyParameters
            {
                MinWeight = calculated.MinWeight,
                MaxWeight = calculated.MaxWeight,
                BMI = calculated.BMI,
                BMR = calculated.BMR
            };
            
            _dbContext.ApplicationBodyParameters.Add(appBodyParameters);
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
            
            var dailyCalories = (int)(appBodyParameters.BMR * offset ?? 0);
        
            var closestRation = CalculateTargetRation(dailyCalories);

            var application = new CaloriApplication
            {
                GenderId = gender,
                Weight = weight,
                Height = height,
                Age = age,
                Goal = goal,
                Email = email,
                ActivityLevelId = activity,
                CreatedAt = DateTime.UtcNow,
                AnotherAllergy = anotherAllergy,
                ApplicationBodyParametersId = appBodyParameters.Id,
                DailyCalories = dailyCalories,
                Ration = closestRation
            };
            
            var deficitOnWeek = dailyCalories - closestRation;
            var burnedOnWeek = (deficitOnWeek * 1000) / 8000;

            var caloriSlimmingPlan = await _dbContext.CaloriSlimmingPlan
                .Where(p => p.Calories == closestRation)
                .FirstOrDefaultAsync(cancellationToken);

            var planCreateCommand = new CreatePersonalSlimmingPlanCommand
            {
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
            
            var command = _mapper.Map<CreatePersonalSlimmingPlanCommand>(planCreateCommand);
            var plan = await _mediator.Send(command);

            application.PersonalSlimmingPlanId = plan.Id;
            
            _dbContext.CaloriApplications.Add(application);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (allergies != null)
            {
                foreach (var allergyId in allergies)
                {
                    var applicationAllergy = new ApplicationAllergy
                    {
                        ApplicationId = application.Id,
                        Allergy = (Allergies)allergyId
                    };
            
                    _dbContext.ApplicationAllergies.Add(applicationAllergy);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return application;
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