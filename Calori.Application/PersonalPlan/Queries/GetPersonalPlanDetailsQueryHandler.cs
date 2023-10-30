using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.PersonalPlan.Queries
{
    public class GetPersonalPlanDetailsQueryHandler
        : IRequestHandler<GetPersonalPlanDetailsQuery, PersonalPlanDetailsVm>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager; 
        
        public GetPersonalPlanDetailsQueryHandler(ICaloriDbContext dbContext,
            IMapper mapper, UserManager<ApplicationUser> userManager) => 
            (_dbContext, _mapper, _userManager) = (dbContext, mapper, userManager);
        
        public async Task<PersonalPlanDetailsVm> Handle(GetPersonalPlanDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var application = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(x => x.Email == request.UserEmail);
            
            var entity = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(plan =>
                    plan.Id == application.PersonalSlimmingPlanId, cancellationToken);
            
            var plan = await _dbContext.CaloriSlimmingPlan
                .FirstOrDefaultAsync(plan =>
                    plan.Id == entity.CaloriSlimmingPlanId, cancellationToken);

            entity.CaloriSlimmingPlan = plan;

            if (entity == null)
            {
                throw new NotFoundException(nameof(PersonalSlimmingPlan), application.PersonalSlimmingPlanId);
            }

            return _mapper.Map<PersonalPlanDetailsVm>(entity);
        }
    }
}