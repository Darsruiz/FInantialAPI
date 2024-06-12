namespace FInantialAPI.Models.Infrastructure
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Iban { get; set; }
        public decimal Balance { get; set; }
        public decimal CreditBalance { get; set; }
        public int IdBankEntity { get; set; }
    }
}
