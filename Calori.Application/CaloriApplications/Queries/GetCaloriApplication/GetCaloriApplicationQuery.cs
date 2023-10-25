using MediatR;

namespace Calori.Application.CaloriApplications.Queries.GetCaloriApplication
{
    public class GetCaloriApplicationQuery : IRequest<CaloriApplicationDetailsVm>
    {
        public int Id { get; set; }
    }
}