using System;
using MediatR;

namespace Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan
{
    public class GetAutoCalculatedPlanQuery : IRequest<AutoCalculatedPlanVm>
    {
        public DateTime CurrentDate { get; set; }
        public string UserEmail { get; set; }
    }
}