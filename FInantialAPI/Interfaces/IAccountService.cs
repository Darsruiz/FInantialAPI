using FInantialAPI.Models;

namespace FInantialAPI.Interfaces
{
    public interface IAccountService
    {
        List<Transaction> GetMovements(int accountId);
    }
}
