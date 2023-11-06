using Calori.Domain.Models.Payment;
using MediatR;

namespace Calori.Application.Payment.AfterPayment
{
    public class AfterPaymentCommand : IRequest<AfterPaymentResponse>
    {
        public string Status { get; set; }
        public string SessionId { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string UserPhone { get; set; }
        public string City { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public long? AmountTotal { get; set; }
     }
}