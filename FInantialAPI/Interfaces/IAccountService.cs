using FInantialAPI.Models;
using FInantialAPI.Models.Infrastructure;

namespace FInantialAPI.Interfaces
{
    public interface IAccountService
    {
        List<TransactionModel> GetMovements(int accountId);
        bool Withdraw(int cardId, WithdrawalRequestModel request);
        bool Deposit(int cardId, DepositRequestModel request);
        bool Transfer(int accountId, TransferRequestModel request);
        bool ActivateCard(int accountId, int cardId);
        bool ChangeCardPIN(int accountId, int cardId, PinChangeRequestModel request);


    }
}
