namespace Calori.Domain.Models.CaloriAccount
{
    public class CaloriShippingData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string City { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }
}