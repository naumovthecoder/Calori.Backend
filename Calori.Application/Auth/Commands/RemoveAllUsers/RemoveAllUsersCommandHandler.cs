using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Auth.Commands.ResetPassword;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.Auth.Commands.RemoveAllUsers
{
    public class RemoveAllUsersCommandHandler
        : IRequestHandler<RemoveAllUsersCommand, RemoveAllUsersResponse>
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly ICaloriDbContext _dbContext;

        public RemoveAllUsersCommandHandler(UserManager<ApplicationUser> userManager, 
            IEmailService emailService, ICaloriDbContext dbContext)
        {
            _userManager = userManager;
            _emailService = emailService;
            _dbContext = dbContext;
        }

        public async Task<RemoveAllUsersResponse> Handle(RemoveAllUsersCommand request, 
            CancellationToken cancellationToken)
        {
            var users = _userManager.Users.ToList();

            if (users.Count < 1)
            {
                return new RemoveAllUsersResponse { Message = "Нечего удалять." };
            }
    
            foreach (var user in users)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return new RemoveAllUsersResponse { Message = "Что-то сломалось." };
                }
                
                var application = await _dbContext.CaloriApplications
                    .FirstOrDefaultAsync(x => x.Email == user.Email, cancellationToken);

                if (application != null)
                {
                    var entity = await _dbContext.PersonalSlimmingPlan
                        .FirstOrDefaultAsync(plan =>
                            application != null && plan.Id == application.PersonalSlimmingPlanId, cancellationToken);

                    _dbContext.CaloriApplications.Remove(application);
                    
                    if (entity != null)
                    {
                        var plan = await _dbContext.CaloriSlimmingPlan
                            .FirstOrDefaultAsync(plan => entity != null && plan.Id == entity.CaloriSlimmingPlanId, cancellationToken);

                        _dbContext.PersonalSlimmingPlan.Remove(entity);

                        if (plan != null)
                        {
                            _dbContext.CaloriSlimmingPlan.Remove(plan);
                        }
                    }
                }
            }
            
            return new RemoveAllUsersResponse { Message = "Все юзеры удалены." };
        }
    }
}