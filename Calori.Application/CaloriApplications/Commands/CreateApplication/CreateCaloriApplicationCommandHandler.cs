using System;
using System.Threading;
using System.Threading.Tasks;
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
                AnotherAllergy = anotherAllergy
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
    }
}