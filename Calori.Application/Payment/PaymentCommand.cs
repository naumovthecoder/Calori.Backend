using Calori.Domain.Models.Auth;
using Calori.Domain.Models.Payment;
using MediatR;

namespace Calori.Application.Payment
{
    public class PaymentCommand : IRequest<PaymentResponse>
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
    }
}