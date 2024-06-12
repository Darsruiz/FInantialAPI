using FInantialAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FInantialAPI.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }


        [HttpGet("{accountId}/movements")]
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
    }
}
