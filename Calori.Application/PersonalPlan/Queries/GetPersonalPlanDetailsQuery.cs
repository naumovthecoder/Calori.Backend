using MediatR;

namespace Calori.Application.PersonalPlan.Queries
{
    public class GetPersonalPlanDetailsQuery : IRequest<PersonalPlanDetailsVm>
    {
        public string UserEmail { get; set; }
    }
}