namespace FInantialAPI.Models
{
    public class TransferRequestModel
    {
        public string SourceIban { get; set; }
        public string TargetIban { get; set; }
        public decimal Amount { get; set; }
    }
}
