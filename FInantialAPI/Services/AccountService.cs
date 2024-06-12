using FInantialAPI.Interfaces;
using FInantialAPI.Models;
using FInantialAPI.Models.Infrastructure;
using FInantialAPI.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FInantialAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private List<TransactionModel> transactions = new List<TransactionModel>
        {
            new TransactionModel { Id = 1, Date = DateTime.Now.AddDays(-1), Amount = 100, Type = "Deposit", Description = "Salary", AccountId = 1 },
            new TransactionModel { Id = 2, Date = DateTime.Now.AddDays(-2), Amount = -20, Type = "Withdrawal", Description = "ATM Withdrawal", AccountId = 1 },
            new TransactionModel { Id = 3, Date = DateTime.Now.AddDays(-3), Amount = -5, Type = "Commission", Description = "Service Fee", AccountId = 1 },
            new TransactionModel { Id = 4, Date = DateTime.Now.AddDays(-4), Amount = 50, Type = "Transfer", Description = "Bizum", AccountId = 1 }
        };

        private List<AccountModel> accounts = new List<AccountModel>
        {
            new AccountModel { Id = 1, Iban = "DE89370400440532013000", Balance = 500, CreditBalance = 0, IdBankEntity = 1 },
            new AccountModel { Id = 2, Iban = "DE89370400440532013001", Balance = 300, CreditBalance = 0, IdBankEntity = 1 }
        };

        private List<CardModel> cards = new List<CardModel>
        {
            new CardModel { Id = 1, IsActive = false, CardType = "Debit", CreditLimit = 300, AccountId = 1, HashedPin = "", Salt = "" }
        };

        public AccountService(ILogger<AccountService> logger)
        {
            _logger = logger;
        }

        public List<TransactionModel> GetMovements(int accountId)
        {
            _logger.LogInformation($"Fetching movements for account {accountId}");
            return transactions.Where(t => t.AccountId == accountId).ToList();
        }

        public bool Withdraw(int cardId, WithdrawalRequestModel request)
        {
            _logger.LogInformation($"Attempting to withdraw {request.Amount} from card {cardId}");
            var card = cards.FirstOrDefault(c => c.Id == cardId);
            if (card == null || !card.IsActive)
            {
                _logger.LogWarning($"Card {cardId} not found or inactive.");
                return false; // Card not found or not active
            }

            var account = accounts.FirstOrDefault(a => a.Id == card.AccountId);
            if (account == null)
            {
                _logger.LogWarning($"Account for card {cardId} not found.");
                return false; // Account not found
            }

            if (card.CardType == "Debit")
            {
                if (account.Balance >= request.Amount)
                {
                    account.Balance -= request.Amount;
                    transactions.Add(new TransactionModel
                    {
                        Id = transactions.Count + 1,
                        Date = DateTime.Now,
                        Amount = -request.Amount,
                        Type = "Withdrawal",
                        Description = "ATM Withdrawal",
                        AccountId = account.Id
                    });
                    _logger.LogInformation($"Withdrawal of {request.Amount} from account {account.Id} successful.");
                    return true;
                }
                _logger.LogWarning($"Insufficient funds in account {account.Id} for withdrawal.");
                return false;
            }
            else if (card.CardType == "Credit")
            {
                if (account.CreditBalance + card.CreditLimit >= request.Amount)
                {
                    account.CreditBalance -= request.Amount;
                    transactions.Add(new TransactionModel
                    {
                        Id = transactions.Count + 1,
                        Date = DateTime.Now,
                        Amount = -request.Amount,
                        Type = "Withdrawal",
                        Description = "ATM Withdrawal",
                        AccountId = account.Id
                    });
                    _logger.LogInformation($"Withdrawal of {request.Amount} from account {account.Id} using credit successful.");
                    return true;
                }
                _logger.LogWarning($"Credit limit exceeded for account {account.Id}.");
                return false;
            }
            return false;
        }

        public bool Deposit(int cardId, DepositRequestModel request)
        {
            _logger.LogInformation($"Attempting to deposit {request.Amount} to card {cardId}");
            var card = cards.FirstOrDefault(c => c.Id == cardId);
            if (card == null || !card.IsActive)
            {
                _logger.LogWarning($"Card {cardId} not found or inactive.");
                return false; // Card not found or not active
            }

            var account = accounts.FirstOrDefault(a => a.Id == card.AccountId);
            if (account == null)
            {
                _logger.LogWarning($"Account for card {cardId} not found.");
                return false; // Account not found
            }

            account.Balance += request.Amount;
            transactions.Add(new TransactionModel
            {
                Id = transactions.Count + 1,
                Date = DateTime.Now,
                Amount = request.Amount,
                Type = "Deposit",
                Description = "ATM Deposit",
                AccountId = account.Id
            });
            _logger.LogInformation($"Deposit of {request.Amount} to account {account.Id} successful.");
            return true;
        }

        public bool Transfer(TransferRequestModel request)
        {
            _logger.LogInformation($"Attempting to transfer {request.Amount} from {request.SourceIban} to {request.TargetIban}");

            var sourceAccount = accounts.FirstOrDefault(a => a.Iban == request.SourceIban);
            if (sourceAccount == null)
            {
                _logger.LogWarning($"Source account with IBAN {request.SourceIban} not found.");
                return false; // Source account not found
            }

            var targetAccount = accounts.FirstOrDefault(a => a.Iban == request.TargetIban);
            if (targetAccount == null)
            {
                _logger.LogWarning($"Target account with IBAN {request.TargetIban} not found.");
                return false; // Target account not found
            }

            if (sourceAccount.Balance >= request.Amount)
            {
                sourceAccount.Balance -= request.Amount;
                targetAccount.Balance += request.Amount;

                transactions.Add(new TransactionModel
                {
                    Id = transactions.Count + 1,
                    Date = DateTime.Now,
                    Amount = -request.Amount,
                    Type = "Transfer",
                    Description = $"Transfer to {request.TargetIban}",
                    AccountId = sourceAccount.Id
                });

                transactions.Add(new TransactionModel
                {
                    Id = transactions.Count + 1,
                    Date = DateTime.Now,
                    Amount = request.Amount,
                    Type = "Transfer",
                    Description = $"Transfer from {request.SourceIban}",
                    AccountId = targetAccount.Id
                });

                _logger.LogInformation($"Transfer of {request.Amount} from {request.SourceIban} to {request.TargetIban} successful.");
                return true;
            }
            _logger.LogWarning($"Insufficient funds in source account {sourceAccount.Iban} for transfer.");
            return false;
        }

        public bool ActivateCard(int accountId, int cardId, PinChangeRequestModel request)
        {
            _logger.LogInformation($"Attempting to activate card {cardId} for account {accountId}");
            var card = cards.FirstOrDefault(c => c.Id == cardId && c.AccountId == accountId);
            if (card == null)
            {
                _logger.LogWarning($"Card {cardId} for account {accountId} not found.");
                return false; // Card not found
            }

            if (card.IsActive)
            {
                _logger.LogWarning($"Card {cardId} for account {accountId} is already active.");
                return false; // Card is already active
            }

            // Hash the initial PIN and update the card
            string salt = HashingUtilities.GenerateSalt();
            string hashedPin = HashingUtilities.HashPin(request.NewPin, salt);
            card.HashedPin = hashedPin;
            card.Salt = salt;
            card.IsActive = true;

            _logger.LogInformation($"Card {cardId} for account {accountId} activated successfully with PIN set.");
            return true;
        }

        public bool ChangeCardPIN(int accountId, int cardId, PinChangeRequestModel request)
        {
            _logger.LogInformation($"Attempting to change PIN for card {cardId} of account {accountId}");
            var card = cards.FirstOrDefault(c => c.Id == cardId && c.AccountId == accountId);
            if (card == null || !card.IsActive)
            {
                _logger.LogWarning($"Card {cardId} for account {accountId} not found or inactive.");
                return false; // Card not found or not active
            }

            // Validate the old PIN
            string oldHashedPin = HashingUtilities.HashPin(request.OldPin, card.Salt);
            if (card.HashedPin != oldHashedPin)
            {
                _logger.LogWarning($"Old PIN provided for card {cardId} of account {accountId} is incorrect.");
                return false; // Old PIN is incorrect
            }

            // Hash the new PIN and update the card
            string newSalt = HashingUtilities.GenerateSalt();
            string newHashedPin = HashingUtilities.HashPin(request.NewPin, newSalt);
            card.HashedPin = newHashedPin;
            card.Salt = newSalt;

            _logger.LogInformation($"PIN for card {cardId} of account {accountId} changed successfully.");
            return true;
        }

    }
}
