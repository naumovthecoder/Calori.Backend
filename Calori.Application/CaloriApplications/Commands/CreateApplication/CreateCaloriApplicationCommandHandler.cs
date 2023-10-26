using System;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.CaloriApplications.CalculatorService;
using Calori.Application.Interfaces;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Enums;
using MediatR;

namespace Calori.Application.CaloriApplications.Commands.CreateApplication
{
    public class CreateCaloriApplicationCommandHandler 
        : IRequestHandler<CreateCaloriApplicationCommand, CaloriApplication>
    {
        private readonly ICaloriDbContext _dbContext;

        public CreateCaloriApplicationCommandHandler(ICaloriDbContext dbContext) =>
            _dbContext = dbContext;
        
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
            
            var deduction = 450;
        
            int[] rations = { 2500, 2000, 1500 };
        
            var closestRation = FindClosestRation(dailyCalories - deduction, rations);

            var application = new CaloriApplication
            {
                GenderId = gender,
                Weight = weight,
                Height = height,
                Age = age,
                Goal = goal,
                Email = email,
                ActivityLevelId = activity,
                CreatedAt = DateTime.Now,
                AnotherAllergy = anotherAllergy,
                ApplicationBodyParametersId = appBodyParameters.Id,
                DailyCalories = dailyCalories,
                Ration = closestRation
            };
        
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
        
        static int FindClosestRation(int targetCalories, int[] rations)
        {
            int closestRation = rations[0];
            int minDifference = Math.Abs(targetCalories - rations[0]);

            foreach (int ration in rations)
            {
                int difference = Math.Abs(targetCalories - ration);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestRation = ration;
                }
            }

            return closestRation;
        }
    }
}