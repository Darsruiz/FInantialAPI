namespace FInantialAPI.Models.Infrastructure
{
    public class AccountModel
    {
        int Id { get; set; }
        public string Iban { get; set; }
        decimal Balance { get; set; }
        decimal CreditBalance { get; set; }
        int IdBankEntity { get; set; }
    }
}
