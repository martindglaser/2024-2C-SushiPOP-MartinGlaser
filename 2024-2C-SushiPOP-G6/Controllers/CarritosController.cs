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
using Microsoft.IdentityModel.Tokens;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class CarritosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Carritos
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Index()
        {
            
            var usuarioLogueado = await _userManager.GetUserAsync(User);

            Carrito carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .Include(c => c.ItemsCarrito)
                .ThenInclude(ci => ci.Productos)
                .FirstOrDefaultAsync(c => c.Cliente.Email == usuarioLogueado.Email && !c.Procesado  && !c.Cancelado );


            if (carrito != null)
            {
                //suma total para mostrar por pantalla
                int totalProductos = carrito.ItemsCarrito.Sum(item => item.Cantidad);
                decimal precioTotal = carrito.ItemsCarrito.Sum(item => item.Cantidad * item.PrecioUnitConDto);
                ViewBag.TotalProductos = totalProductos;
                ViewBag.PrecioTotal = precioTotal;
            }
            return View(carrito);
        }


        // GET: Carritos/Details/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "NumeroCliente", "NumeroCliente");
            return View();
        }

        // POST: Carritos/Create
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return View(carrito); 
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "NumeroCliente", "NumeroCliente", carrito.ClienteId);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "CLIENTE")]
        // GET: Carritos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "NumeroCliente", "NumeroCliente", carrito.ClienteId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "NumeroCliente", "NumeroCliente", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "CLIENTE")] 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //conseguir usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);

            //buscar cliente con email
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email == usuarioLogueado.Email);
 

            // buscar carrito 
            var carrito = await _context.Carrito
            .Include(c => c.Cliente)
            .Include(item => item.ItemsCarrito)
            .ThenInclude(ic => ic.Productos)
            .FirstOrDefaultAsync(c => c.Id == id);

            foreach (var item in carrito.ItemsCarrito)
            { 
                var producto = await _context.Producto.FindAsync(item.ProductoId);
                if (producto != null)
                { 
                    producto.Strock += item.Cantidad;
                    _context.Update(producto);
                }
            }


            //cancelar carrito 
            carrito.Cancelado = true;
            _context.Remove(carrito);
            _context.Update(carrito);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }

          private bool CarritoExists(int id)
        {
            return _context.Carrito.Any(e => e.Id == id);
        }
   
    }
}
