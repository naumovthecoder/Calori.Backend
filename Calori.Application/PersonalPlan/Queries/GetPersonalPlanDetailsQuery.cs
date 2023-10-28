using MediatR;

namespace Calori.Application.PersonalPlan.Queries
{
    public class GetPersonalPlanDetailsQuery : IRequest<PersonalPlanDetailsVm>
    {
        public int Id { get; set; }
    }
}