namespace FInantialAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // TODO: make enum!!
        public string? Description { get; set; }
        public int AccountId { get; set; }
    }
}
