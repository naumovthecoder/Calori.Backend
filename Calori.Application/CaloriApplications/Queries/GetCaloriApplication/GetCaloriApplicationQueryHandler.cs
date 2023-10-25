using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.ApplicationModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.CaloriApplications.Queries.GetCaloriApplication
{
    public class GetCaloriApplicationQueryHandler
        : IRequestHandler<GetCaloriApplicationQuery, CaloriApplicationDetailsVm>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly IMapper _mapper;
    
        public GetCaloriApplicationQueryHandler(ICaloriDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);
    
        public async Task<CaloriApplicationDetailsVm> Handle(GetCaloriApplicationQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(note =>
                    note.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(CaloriApplication), request.Id);
            }

            return _mapper.Map<CaloriApplicationDetailsVm>(entity);
        }
    }
}