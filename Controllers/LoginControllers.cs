using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Interfaces;
using tl2_tp8_2025_ElAguhs.ViewModels;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthenticationService _authenticationService; 

        public LoginController(IAuthenticationService authenticationService) 
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
           
            if (_authenticationService.Login(model.Username!, model.Password!))
            {
                
                return RedirectToAction("Index", "Home");
            }

           
            model.ErrorMessage = "Credenciales inválidas.";
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _authenticationService.Logout();
            return RedirectToAction("Index", "Login"); 
        }
    }
}