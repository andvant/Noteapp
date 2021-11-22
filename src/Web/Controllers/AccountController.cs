using Microsoft.AspNetCore.Mvc;
using Noteapp.Core.Interfaces;
using Noteapp.Web.ViewModels;
using System.Threading.Tasks;

namespace Noteapp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiCaller _apiCaller;

        public AccountController(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _apiCaller.Login(vm.Email, vm.Password);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _apiCaller.Register(vm.Email, vm.Password);

            return RedirectToAction("Index", "Home");
        }
    }
}
