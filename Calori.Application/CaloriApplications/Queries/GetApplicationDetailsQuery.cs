using MediatR;

namespace Calori.Application.CaloriApplications.Queries
{
    public class GetApplicationDetailsQuery : IRequest<ApplicationDetailsVm>
    {
        public string UserId { get; set; }
    }
}