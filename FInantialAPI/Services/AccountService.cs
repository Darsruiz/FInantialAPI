using FInantialAPI.Interfaces;
using FInantialAPI.Models;

namespace FInantialAPI.Services
{
    public class AccountService : IAccountService
    {
        private static List<Transaction> transactions =
    [
        new Transaction { Id = 1, Date = DateTime.Now.AddDays(-1), Amount = 100, Type = "Deposit", Description = "Salary", AccountId = 1 },
        new Transaction { Id = 2, Date = DateTime.Now.AddDays(-2), Amount = -20, Type = "Withdrawal", Description = "ATM Withdrawal", AccountId = 1 },
        new Transaction { Id = 3, Date = DateTime.Now.AddDays(-3), Amount = -5, Type = "Commission", Description = "Service Fee", AccountId = 1 },
        new Transaction { Id = 4, Date = DateTime.Now.AddDays(-4), Amount = 50, Type = "Transfer", Description = "Bizum", AccountId = 1 }
    ];

        public List<Transaction> GetMovements(int accountId)
        {
            return transactions.Where(t => t.AccountId == accountId).ToList();
        }
    }
}
