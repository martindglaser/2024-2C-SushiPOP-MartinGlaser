using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using _2024_2C_SushiPOP_G6.Models.ViewModels;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class ReservasController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservasController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var dbContext = _context.Reserva.Include(r => r.Cliente);
            return View(await dbContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        [Authorize(Roles = "CLIENTE, EMPLEADO")]
        public async Task<IActionResult> Create()
        {
            
                var locales = new List<string> { "CABALLITO", "BELGRANO", "RECOLETA", "PALERMO" };
                ViewBag.Locales = new SelectList(locales); //creo y envio lista de locales al formulario

                var r = new ReservasViewModel
                {
                    Nombre = "",
                    Apellido = "",
                    ClienteId = -1,
                    Local = locales.First(),
                    FechaHora =DateTime.Now ,
                    Confirmada = false                  //creo modelo vacio
                };                                  
           
            if (User.IsInRole("CLIENTE"))           //si el usuario es cliente relleno parte del formulario con sus datos
            {
                r = await CargarFormulario();
                return View(r);
            }
            else if (User.IsInRole("EMPLEADO"))
            {
                return RedirectToAction("Index");
            } else
            {
                return View(r);
            }
            
        

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Local,FechaHora,Confirmada,Nombre,Apellido,ClienteId")] Reserva reserva)
        {
            

            var fechaReservaActual = reserva.FechaHora.Date;
            var existeReserva =  await _context.Reserva
        .AnyAsync(r => r.ClienteId == reserva.ClienteId && r.FechaHora.Date == fechaReservaActual);

            if (existeReserva)
            {
               
                
                return RedirectToAction("ReservaExistente");
            }
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction("Confirmacion");

            }
          
            return View();
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            if (reserva.Confirmada)
            {
                TempData["ErrorMessage"] = "La reserva ya está confirmada y no puede ser editada.";
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Local,FechaHora,Confirmada,Nombre,Apellido,ClienteId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }
            var reservaExistente = await _context.Reserva.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

            if (reservaExistente == null)
            {
                return NotFound();
            }

            
            if (reservaExistente.Confirmada) // ver si esta confirmada, no editar si lo está
            {
                TempData["ErrorMessage"] = "No puedes editar una reserva ya confirmada.";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(reserva).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "NumeroCliente", "NumeroCliente", reserva.ClienteId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva != null)
            {
                _context.Reserva.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.Id == id);
        }
        public async Task<ReservasViewModel> CargarFormulario()
        {
            var usuario = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == usuario.Email.ToUpper());
            var nombre = cliente.Nombre;
            var apellido = cliente.Apellido;
            int identificador = cliente.Id;

            return new ReservasViewModel
            {
                Nombre = nombre,
                Apellido = apellido,
                ClienteId = identificador,
                Local = "Belgrano",
                FechaHora = DateTime.Now,
                Confirmada = false,
            };
        }
        public IActionResult Confirmacion()
        {
            return View(); 
        }
        public IActionResult ReservaExistente()
        {
            return View();
        }
    }
}
