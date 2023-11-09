using System;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.CalculatorService;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Application.Services.UserService;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Commands.UpdateApplication
{
    public class UpdateApplicationCommandHandler 
        : IRequestHandler<UpdateApplicationCommand, UpdateApplicationResult>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly IEmailService _emailService; 

        
        public UpdateApplicationCommandHandler(ICaloriDbContext dbContext, 
            IMapper mapper, IMediator mediator, UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<UpdateApplicationResult> Handle(UpdateApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var gender = request.Gender;
            var weight = request.Weight;
            var height = request.Height;
            var age = request.Age;
            var goal = request.Goal;
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
            
            if (dailyCalories - 750 > 2500 || dailyCalories <= 1250)
            {
                var updateErrorResult = new UpdateApplicationResult();
                updateErrorResult.Message = "There is no suitable diet.";
                return updateErrorResult;
            }
            
            if (!string.IsNullOrEmpty(request.Email) && request.Email != request.IdentityUserEmail)
            {
                var user = await _userManager.FindByEmailAsync(request.IdentityUserEmail);

                if (user != null)
                {
                    var password = new PasswordGenerator().GeneratePassword();
                    
                    try
                    {
                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, resetToken, password);
                    }
                    catch (Exception e)
                    {
                        throw new AuthenticationException(e.Message);
                    }
                    
                    user.Email = request.Email;
                    await _userManager.UpdateAsync(user);
                    
                    string subject = $"Hi, \ud83d\udc4b";
                    string body = $"Welcome to Calori.App!\nYour Login - {user.Email}\nPassword - {password}";
                    
                    var message = "";
                    // var culture = CultureInfo.CurrentUICulture;
                    //
                    //
                    // if (culture.Name.ToLower() == "fi")
                    // {
                    //     message = $"Hei, {request.Email} \ud83d\udc4b\n\nKiitos, että loit ateriasuunnitelmasi Calorin kanssa." +
                    //               $"\n\nValitettavasti emme tällä hetkellä pysty huomioimaan erityisruokavalioitasi." +
                    //               $"\n\nTyöskentelemme kuitenkin ahkerasti kehittääksemme uusia, erilaisia ateriasuunnitelmia. " +
                    //               $"Olet ensimmäisten joukossa kuulemassa, kun meillä on tarjolla suunnitelmia jotka vastaavat " +
                    //               $"tarpeitasi \ud83d\ude4c\n\nYstävällisin terveisin,\nCalori";
                    // }
                    // else if (culture.Name.ToLower() == "en")
                    // {
                    //     message = $"Hi, {request.Email} \ud83d\udc4b\n\nThank you for making your meal plan with Calori." +
                    //               $"\n\nSadly, we can’t accommodate people with one of your dietary restrictions at this time." +
                    //               $"\n\nBut we’ve been working hard on developing different meal plans. You’ll be the first one " +
                    //               $"to know once we have plans that match your needs \ud83d\ude4c\n\nKind regards,\nCalori";
                    // }
                    
                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, subject, body);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            
            // var entity = await _dbContext.CaloriApplications
            //     .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            
            var entity = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a => 
                    a.Email.ToLower() == request.IdentityUserEmail.ToLower(), cancellationToken);

            var bodyParameters = await _dbContext.ApplicationBodyParameters
                .FirstOrDefaultAsync(p => 
                    p.Id == entity.ApplicationBodyParametersId, cancellationToken);
            
            bodyParameters.MinWeight = calculated.MinWeight;
            bodyParameters.MaxWeight = calculated.MaxWeight;
            bodyParameters.BMI = calculated.BMI;
            bodyParameters.BMR = calculated.BMR;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
////////
            
            var closestRation = CalculateTargetRation(dailyCalories);

            entity.GenderId = gender;
            entity.Weight = weight;
            entity.Height = height;
            entity.Age = age;
            entity.Goal = goal;
            entity.Email = request.Email;

            if (entity.Phone != null)
            {
                entity.Phone = request.Phone;
            }
            
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
            
            if (entity.PersonalSlimmingPlanId != null)
            {
                
                var planCreateCommand = new UpdatePersonalSlimmingPlanCommand();

                planCreateCommand.Id = (int)entity.PersonalSlimmingPlanId;
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
            
                var command = _mapper.Map<UpdatePersonalSlimmingPlanCommand>(planCreateCommand);
                await _mediator.Send(command);
            }

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

            var result = new UpdateApplicationResult
            {
                Application = entity
            };
            return result;
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