using System;

namespace Calori.Domain.Models.CaloriAccount
{
    public class UserPayment
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public double Cost { get; set; }
        public PaymentStatus? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}