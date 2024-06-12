using FInantialAPI.Interfaces;
using FInantialAPI.Models;
using FInantialAPI.Models.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FInantialAPI.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet("accounts/{accountId}/movements")]
        public IActionResult GetMovements(int accountId)
        {
            _logger.LogInformation($"Fetching movements for account {accountId}");
            var movements = _accountService.GetMovements(accountId);
            if (movements == null || !movements.Any())
            {
                _logger.LogWarning($"No movements found for account {accountId}");
                return NotFound("No movements found for the given account.");
            }
            return Ok(movements);
        }

        [HttpPost("cards/{cardId}/withdraw")]
        public IActionResult Withdraw(int cardId, [FromBody] WithdrawalRequestModel request)
        {
            _logger.LogInformation($"Attempting to withdraw {request.Amount} from card {cardId}");
            var result = _accountService.Withdraw(cardId, request);
            if (result)
            {
                _logger.LogInformation($"Withdrawal successful for card {cardId}");
                return Ok("Withdrawal successful.");
            }
            _logger.LogWarning($"Withdrawal failed for card {cardId}");
            return BadRequest("Insufficient funds or credit limit. Is the card activated?");
        }

        [HttpPost("cards/{cardId}/deposit")]
        public IActionResult Deposit(int cardId, [FromBody] DepositRequestModel request)
        {
            _logger.LogInformation($"Attempting to deposit {request.Amount} to card {cardId}");
            var result = _accountService.Deposit(cardId, request);
            if (result)
            {
                _logger.LogInformation($"Deposit successful for card {cardId}");
                return Ok("Deposit successful.");
            }
            _logger.LogWarning($"Deposit failed for card {cardId}");
            return BadRequest("Deposit failed.");
        }

        [HttpPost("accounts/transfer")]
        public IActionResult Transfer([FromBody] TransferRequestModel request)
        {
            _logger.LogInformation($"Attempting to transfer {request.Amount} from {request.SourceIban} to {request.TargetIban}");
            var result = _accountService.Transfer(request);
            if (result)
            {
                _logger.LogInformation($"Transfer from {request.SourceIban} to {request.TargetIban} successful");
                return Ok("Transfer successful.");
            }
            _logger.LogWarning($"Transfer from {request.SourceIban} to {request.TargetIban} failed");
            return BadRequest("Transfer failed.");
        }


        [HttpPost("accounts/{accountId}/cards/{cardId}/activate")]
        public IActionResult ActivateCard(int accountId, int cardId, [FromBody] PinChangeRequestModel request)
        {
            _logger.LogInformation($"Activating card {cardId} for account {accountId}");
            var result = _accountService.ActivateCard(accountId, cardId, request);
            if (result)
            {
                _logger.LogInformation($"Card {cardId} activated for account {accountId}");
                return Ok("Card activated.");
            }
            _logger.LogWarning($"Card activation failed for card {cardId}");
            return BadRequest("Card activation failed.");
        }

        [HttpPost("accounts/{accountId}/cards/{cardId}/changepin")]
        public IActionResult ChangeCardPIN(int accountId, int cardId, [FromBody] PinChangeRequestModel request)
        {
            _logger.LogInformation($"Changing PIN for card {cardId} of account {accountId}");
            var result = _accountService.ChangeCardPIN(accountId, cardId, request);
            if (result)
            {
                _logger.LogInformation($"PIN changed for card {cardId} of account {accountId}");
                return Ok("PIN changed.");
            }
            _logger.LogWarning($"PIN change failed for card {cardId}");
            return BadRequest("PIN change failed.");
        }
    }
}
