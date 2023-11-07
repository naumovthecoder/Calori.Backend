using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Auth;
using Calori.Application.Auth.Commands.Register;
using Calori.Application.CaloriApplications.CalculatorService;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.CreatePersonalSlimmingPlan;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Commands.CreateApplication
{
    public class CreateCaloriApplicationCommandHandler 
        : IRequestHandler<CreateCaloriApplicationCommand, CreateApplicationResult>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        protected IMediator _mediator;
        private readonly IEmailService _emailService;

        public CreateCaloriApplicationCommandHandler(ICaloriDbContext dbContext, 
            IMapper mapper, IMediator mediator, IEmailService emailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
            _emailService = emailService;
        }

        public async Task<CreateApplicationResult> Handle(CreateCaloriApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var createApplicationResponse = new CreateApplicationResult();
            
            var gender = request.Gender;
            var weight = request.Weight;
            var height = request.Height;
            var age = request.Age;
            var goal = request.Goal;
            var email = request.Email;
            var activity = request.Activity;
            var allergies = request.Allergies;
            string anotherAllergy = "";

            if (!string.IsNullOrEmpty(request.AnotherAllergy))
            {
                anotherAllergy = request.AnotherAllergy;
            }

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

            if (dailyCalories - 750 > 2500 || dailyCalories <= 1250 || 
                goal < calculated.MinWeight || weight <= calculated.MinWeight)
            {
                createApplicationResponse.Message = "There is no suitable diet.";
                
                await _emailService.SendEmailAsync(request.Email, 
                    "There are no suitable diets for you.", 
                    "We will contact you for more details.");
                
                return createApplicationResponse;
            }
            
            _dbContext.ApplicationBodyParameters.Add(appBodyParameters);
            await _dbContext.SaveChangesAsync(cancellationToken);
        
            var closestRation = CalculateTargetRation(dailyCalories);

            var application = new CaloriApplication();

            application.GenderId = gender;
            application.Weight = weight;
            application.Height = height;
            application.Age = age;
            application.Goal = goal;
            application.Email = email;
            application.ActivityLevelId = activity;
            application.CreatedAt = DateTime.UtcNow;
            application.AnotherAllergy = anotherAllergy;
            application.ApplicationBodyParametersId = appBodyParameters.Id;
            application.DailyCalories = dailyCalories;
            application.Ration = closestRation;
            
            var deficitOnWeek = dailyCalories - closestRation;
            var burnedOnWeek = (deficitOnWeek * 1000) / 8000;
            
            var caloriSlimmingPlan = await _dbContext.CaloriSlimmingPlan
                .Where(p => p.Calories == closestRation)
                .FirstOrDefaultAsync(cancellationToken);

            var planCreateCommand = new CreatePersonalSlimmingPlanCommand();

            planCreateCommand.WeekNumber = 1;
            planCreateCommand.Weight = weight;
            planCreateCommand.CaloricNeeds = dailyCalories;
            planCreateCommand.Goal = goal;
            planCreateCommand.CurrentWeekDeficit = deficitOnWeek;
            planCreateCommand.BurnedThisWeek = burnedOnWeek;
            planCreateCommand.TotalBurned = 0;
            planCreateCommand.CaloriSlimmingPlanId = caloriSlimmingPlan.Id;
            planCreateCommand.Gender = gender;
            planCreateCommand.CurrentCalori = closestRation;
            planCreateCommand.CaloriActivityLevel = activity;
            
            var command = _mapper.Map<CreatePersonalSlimmingPlanCommand>(planCreateCommand);
            var plan = await _mediator.Send(command);
            
            var registerModel = new RegisterModel
            {
                Email = request.Email,
                UserName = request.Email
            };

            bool hasLactoseOrNone = allergies == null || allergies.Count == 0 || allergies.Contains(1);
            bool hasNoOtherAllergies = string.IsNullOrEmpty(anotherAllergy);

            RegisterResponse registerResponse;

            if (hasLactoseOrNone && hasNoOtherAllergies)
            {
                registerResponse = await RegisterUser(registerModel);
                if (registerResponse == null)
                {
                    throw new Exception("Error when register user");
                }
                
                application.PersonalSlimmingPlanId = plan.Id;
                application.UserId = registerResponse.User.Id;
                createApplicationResponse.Token = registerResponse.Token;
            }
            else
            {
                createApplicationResponse.Message = 
                    "Due to your food allergies, we are unable to offer a diet that suits your needs.";

                await _emailService.SendEmailAsync(request.Email, 
                    "There are no suitable diets.", 
                    "It is recommended that a patient with allergies to certain foods develop an individualized diet, " +
                    "eliminating allergens, and consult with a physician or dietitian to ensure a balanced diet.");
            }
            
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

            if (hasLactoseOrNone && hasNoOtherAllergies)
            {
                createApplicationResponse.Application = application;
            }
            
            return createApplicationResponse;
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

        private async Task<RegisterResponse> RegisterUser(RegisterModel model)
        {
            var command = _mapper.Map<RegisterUserCommand>(model);
            var response = await _mediator.Send(command);

            return response;
        }
    }
}