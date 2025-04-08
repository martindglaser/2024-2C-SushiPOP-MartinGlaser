using _2024_2C_SushiPOP_G6.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public LoginController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }


        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> IniciarSesion([Bind("Email,Clave")] Login login)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(login.Email, login.Clave, false, false);
                if (result.Succeeded) {
                    return View("Views/PanelUsuario/Index.cshtml");
                }
            }
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return View();
        }
    }
}
