using Calori.Domain.Models.CaloriAccount;

namespace Calori.Domain.Models.Payment
{
    public class AfterPaymentResponse
    {
        public CaloriShippingData ShippingData { get; set; }
        public string Message { get; set; }
    }
}