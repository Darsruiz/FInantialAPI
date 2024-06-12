namespace FInantialAPI.Models.Infrastructure
{
    public class CardModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

        //public string PinCode { get; set; } 
        public string CardType { get; set; } // TODO: make enum !!
        public decimal CreditLimit { get; set; } // TODO: Registrar transaccion de tarjeta de CredBalance?
        public int AccountId { get; set; }
    }
}
