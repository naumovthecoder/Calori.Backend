using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.ApplicationModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Queries
{
    public class GetApplicationDetailsQueryHandler
        : IRequestHandler<GetApplicationDetailsQuery, ApplicationDetailsVm>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public GetApplicationDetailsQueryHandler(ICaloriDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);
        
        public async Task<ApplicationDetailsVm> Handle(GetApplicationDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var application = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a =>
                    a.Email.ToLower() == request.Email.ToLower(), cancellationToken);

            var applicationAllergies = await _dbContext.ApplicationAllergies
                .Where(a => a.ApplicationId == application.Id).ToListAsync();

            var personalPlan = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(p =>
                    p.Id == application.PersonalSlimmingPlanId, cancellationToken);
            
            var caloriPlan = await _dbContext.CaloriSlimmingPlan
                .FirstOrDefaultAsync(cp =>
                    cp.Id == personalPlan.CaloriSlimmingPlanId, cancellationToken);

            var bodyParameters = await _dbContext.ApplicationBodyParameters
                .FirstOrDefaultAsync(p => 
                    p.Id == application.ApplicationBodyParametersId);
            
            personalPlan.CaloriSlimmingPlan = caloriPlan;
            application.PersonalSlimmingPlan = personalPlan;
            application.ApplicationAllergies = applicationAllergies;
            application.ApplicationBodyParameters = bodyParameters;

            if (application == null)
            {
                throw new NotFoundException(nameof(CaloriApplication), request.Email);
            }

            return _mapper.Map<ApplicationDetailsVm>(application);
        }
    }
}