using Calori.Application.PersonalPlan.Queries;
using MediatR;

namespace Calori.Application.CaloriApplications.Queries
{
    public class GetApplicationDetailsQuery : IRequest<ApplicationDetailsVm>
    {
        public int Id { get; set; }
    }
}