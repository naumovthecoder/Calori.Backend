using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.CaloriAccount;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.PersonalPlan.Queries
{
    public class GetPersonalPlanDetailsQueryHandler
        : IRequestHandler<GetPersonalPlanDetailsQuery, PersonalPlanDetailsVm>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public GetPersonalPlanDetailsQueryHandler(ICaloriDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);
        
        public async Task<PersonalPlanDetailsVm> Handle(GetPersonalPlanDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(plan =>
                    plan.Id == request.Id, cancellationToken);
            
            var plan = await _dbContext.CaloriSlimmingPlan
                .FirstOrDefaultAsync(plan =>
                    plan.Id == entity.CaloriSlimmingPlanId, cancellationToken);

            entity.CaloriSlimmingPlan = plan;

            if (entity == null)
            {
                throw new NotFoundException(nameof(PersonalSlimmingPlan), request.Id);
            }

            return _mapper.Map<PersonalPlanDetailsVm>(entity);
        }
    }
}