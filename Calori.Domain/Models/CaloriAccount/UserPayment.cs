namespace Calori.Domain.Models.CaloriAccount
{
    public class UserPayment
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public bool IsPaid { get; set; } = false;
    }
}