using CK_CSharp.Models;
using CK_CSharp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CK_CSharp.Controllers
{
    [Route("authenticate")]
    [AllowAnonymous]
    public class AuthenticateController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly JwtConfiguration _jwtConfig;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticateController(IAuthenticationService authenticationService, JwtConfiguration jwtConfig, SignInManager<IdentityUser> signInManager)
        {
            _authenticationService = authenticationService;
            _jwtConfig = jwtConfig;
            _signInManager = signInManager;
        }

        [Route("login-view")] 
        public IActionResult LoginView()
        {
            return View();
        }

        [Route("register-view")]
        public IActionResult RegisterView()
        {
            return View();
        }

        [HttpPost]
        [Route("loginuser")]
        public async Task<IActionResult> LoginUser([FromForm] UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Authenticate/LoginView.cshtml", userLogin);
            }

            var result = await _authenticationService.Login(userLogin);

            if (!result.IsSuccess)
            {
                userLogin.ErrorMessage = "Login failed. Please check your username and password.";
                return View("~/Views/Authenticate/LoginView.cshtml", userLogin);
            }

            await GenerateAndWriteToken(result.User);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("registeruser")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegister userRegister)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Authenticate/RegisterView.cshtml", userRegister);
            }

            var result = await _authenticationService.RegisterUser(userRegister);

            if (!result.IsSuccess)
            {
                userRegister.ErrorMessage = result.errors;
                return View("~/Views/Authenticate/RegisterView.cshtml", userRegister);
            }

            return RedirectToAction("LoginView");
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.Logout();
            return RedirectToAction("LoginView");
        }

        private async Task GenerateAndWriteToken(IdentityUser user)
        {
            var accessToken = await _authenticationService.GenerateToken(user, _jwtConfig);
            _authenticationService.WriteAccessToken(accessToken);
        }
    }
}

