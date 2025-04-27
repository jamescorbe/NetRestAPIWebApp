using BlazorAppTestREST_API.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorAppTestREST_API.Controllers
{
    [ApiController]
    [Route("signin")]
    public class SigninController : ControllerBase
    {
        private readonly ILogger<SigninController> _logger;
        private GoogleLoginService _googleLoginService;
        public SigninController(ILogger<SigninController> logger)
        {
            _logger = logger;
            _googleLoginService = new GoogleLoginService();
        }


        [HttpGet("login")]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        [HttpGet("response")]
        public async Task<ActionResult<ClaimsIdentity>> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claim = result?.Principal?.Identities.FirstOrDefault();


            if (claim == null)
                return NotFound();

            return Ok(claim);
        }
    }
}
