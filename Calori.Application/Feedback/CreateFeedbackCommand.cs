using Calori.Domain.Models.CaloriAccount;
using MediatR;

namespace Calori.Application.Feedback
{
    public class CreateFeedbackCommand : IRequest<CaloriFeedbackResponse>
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}