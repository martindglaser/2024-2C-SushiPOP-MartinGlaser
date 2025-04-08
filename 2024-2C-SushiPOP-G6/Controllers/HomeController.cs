using _2024_2C_SushiPOP_G6.Models;
using _2024_2C_SushiPOP_G6.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, DbContext context, 
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager; 
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!_roleManager.RoleExistsAsync("ADMIN").GetAwaiter().GetResult())
            {
                // Se generan roles
                _roleManager.CreateAsync(new IdentityRole("ADMIN")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("CLIENTE")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("EMPLEADO")).GetAwaiter().GetResult();

                // Creacion del ADMIN
                IdentityUser user = new();
                user.Email = user.UserName = "admin@ort.edu.ar";
                IdentityResult result = await _userManager.CreateAsync(user, "Password1!");
                if (result.Succeeded) {
                    await _userManager.AddToRoleAsync(user, "ADMIN");
                }
            }

            int hoy = (int)DateTime.Today.DayOfWeek;

            var productoConDescuento = _context.Producto
                .Where(p => p.Descuentos.Any(d => d.Dia == hoy))
                .Select(p => new Home
                {
                    NombreProducto = p.Nombre,
                    Descuento = p.Descuentos
                        .Where(d => d.Dia == hoy)
                        .Select(d => d.Porcentaje)
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            return View(productoConDescuento);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
