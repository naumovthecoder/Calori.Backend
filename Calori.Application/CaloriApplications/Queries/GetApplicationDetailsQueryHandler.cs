using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Queries;
using Calori.Domain.Models.CaloriAccount;
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
            var entity = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a =>
                    a.Id == request.Id, cancellationToken);
            
            if (entity == null)
            {
                throw new NotFoundException(nameof(PersonalSlimmingPlan), request.Id);
            }

            return _mapper.Map<ApplicationDetailsVm>(entity);
        }
    }
}