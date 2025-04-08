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
    public class ReclamosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReclamosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reclamos
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Index()
        {
         

            var reclamos = await _context.Reclamo.ToListAsync();
            return View(reclamos);
        }

        // GET: Reclamos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .Include(r => r.Pedido)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }

        // GET: Reclamos/Create
        public async Task<IActionResult> Create()
        {


            var pedidos = await _context.Pedido
        .Where(p => p.Estado == 1 || p.Estado == 2 || p.Estado == 3).ToListAsync();
            ViewBag.Pedidos = new SelectList(pedidos, "Id", "NroPedido");



            var viewModel = new ReclamoViewModel
            {
                NombreCompleto = "",
                Email = "",
                Telefono = "",
                DetalleReclamo = "",
                PedidoId = -1

            };
            if (User.IsInRole("CLIENTE"))
            {
                viewModel = await CrearReclamo();
                return View(viewModel);
            }
            else if (User.IsInRole("EMPLEADO") || User.IsInRole("ADMIN"))
            {
                return RedirectToAction("Index"); // vuelvo al listado
           }

            else
            {
                return View(viewModel);
            } 
            }
           
       


        [HttpPost]
        public async Task<IActionResult> Create([Bind("NombreCompleto,Email,Telefono,DetalleReclamo,PedidoId")] Reclamo reclamo)
        {
            
            var reclamoView = new ReclamoViewModel
            {
                NombreCompleto = reclamo.NombreCompleto,
                Email = reclamo.Email,
                Telefono = reclamo.Telefono,
                DetalleReclamo = "",
                PedidoId = -1
            };

            var pedidoValido = await _context.Pedido.AnyAsync(p =>
            p.Id == reclamo.PedidoId &&
           
            (p.Estado == 1 ||  // CONFIRMADO
             p.Estado == 2 ||  // EN REPARTO
             p.Estado == 3));  // ENTREGADO

            var reclamoCreado = await _context.Reclamo.AnyAsync(r => r.PedidoId == reclamo.PedidoId);

            if (!pedidoValido)
            {
                ViewBag.ErrorMessage = $"El número de pedido {reclamo.PedidoId} no es válido.";
                return View(reclamoView);
            }
            if (reclamoCreado)
            {
                ViewBag.ErrorMessage = $"Ya hay un reclamo con ese numero de pedido.";
                return View(reclamoView);
            }


            if (ModelState.IsValid)
            {
                

                _context.Add(reclamo);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Confirmacion");
            }

            ViewBag.ErrorMessage = "Ocurrió un error al validar el formulario.";
            return View(reclamoView);
        }






        // GET: Reclamos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", reclamo.PedidoId);
            return View(reclamo);
        }

        // POST: Reclamos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Email,Telefono,DetalleReclamo,PedidoId")] Reclamo reclamo)
        {
            if (id != reclamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reclamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReclamoExists(reclamo.Id))
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
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", reclamo.PedidoId);
            return View(reclamo);
        }

        // GET: Reclamos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .Include(r => r.Pedido)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }

        // POST: Reclamos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo != null)
            {
                _context.Reclamo.Remove(reclamo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReclamoExists(int id)
        {
            return _context.Reclamo.Any(e => e.Id == id);
        }
        public IActionResult Confirmacion()
        {
            var mensaje = TempData["Confirmacion"]?.ToString();
            ViewBag.Mensaje = mensaje ?? "El reclamo se creó correctamente.";
            return View();
        }

        public async Task<ReclamoViewModel> CrearReclamo()
        {
            var usuario = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == usuario.Email.ToUpper());

            var viewModel = new ReclamoViewModel
            {
                NombreCompleto = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                DetalleReclamo = "",
                PedidoId = -1


            };
            return viewModel;
        }

    }

}
