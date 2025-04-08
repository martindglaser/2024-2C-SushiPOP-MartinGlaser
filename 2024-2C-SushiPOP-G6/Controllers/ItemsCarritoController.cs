using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using _2024_2C_SushiPOP_G6.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class ItemsCarritoController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemsCarritoController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ItemsCarrito
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Index()
        {
            var dbContext = _context.ItemCarrito
                .Include(i => i.Carrito)
                .Where(i => !i.Carrito.Cancelado);
            return View(await dbContext.ToListAsync());
        }

        // GET: ItemsCarrito/Details/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCarrito = await _context.ItemCarrito
                .Include(i => i.Carrito)
                .Include(i => i.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemCarrito == null)
            {
                return NotFound();
            }

            return View(itemCarrito);
        }

        // GET: ItemsCarrito/Create
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Producto, "Id", "Id");
            return View();
        }

        // POST: ItemsCarrito/Create
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id,PrecioUnitConDto,Cantidad,CarritoId,ProductoId")] Item itemCarrito)
        {
            if (ModelState.IsValid)
            {
                Producto producto = await _context.Producto.FindAsync(itemCarrito.ProductoId);

                if(producto.Strock < itemCarrito.Cantidad)
                {
                    return View(itemCarrito);
                }
                // busca quien hace la compra 
                var usuarioLogueado = await _userManager.GetUserAsync(User);

                // pregunta a la base si el cliente existe 
                Cliente cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email == usuarioLogueado.Email);

                // lo mismo pero con el carrito
                Carrito carrito = await _context.Carrito
                    .Include(c => c.Cliente)
                    .Include(c => c.ItemsCarrito)
                    .ThenInclude(ci => ci.Productos)
                    .FirstOrDefaultAsync(c => c.Cliente.Email == usuarioLogueado.Email && !c.Procesado && !c.Cancelado);

                
                decimal precio = producto.Precio;
                decimal descuentoAplicable;
                int dia = (int)DateTime.Today.DayOfWeek;

                var descuentoBuscado = await _context.Descuento.Where(x => x.Activo && x.ProductoId == producto.Id
                && x.Dia == dia).FirstOrDefaultAsync();


                if (descuentoBuscado != null)
                {
                    descuentoAplicable = precio * descuentoBuscado.Porcentaje / 100;

                    if (descuentoAplicable > descuentoBuscado.DescuentoMax)
                    {
                        precio = precio - descuentoBuscado.DescuentoMax;
                    }
                    else
                    {
                        precio = precio - descuentoAplicable;
                    }

                }
                //veo si el cliente tiene un carrito, sino, lo creo 
                if (carrito == null)
                {
                    carrito = new()
                    {
                        Procesado = false,
                        Cancelado = false,
                        ClienteId = cliente.Id,
                        ItemsCarrito = new()
                    };

                    // guarda en el contexto
                    _context.Add(carrito);
                    // guarda en la base de datos
                    await _context.SaveChangesAsync();

                    ItemCarrito item = new()
                    {
                        PrecioUnitConDto = precio, 
                        Cantidad = itemCarrito.Cantidad,
                        ProductoId = producto.Id,
                        Productos = [producto],
                        CarritoId = carrito.Id
                    };

                    _context.Add(item);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //buscar si tiene el producto en el carrito
                    ItemCarrito itemBuscado = carrito.ItemsCarrito.FirstOrDefault(ci  => ci.ProductoId == itemCarrito.ProductoId);

                    if (itemBuscado == null)
                    {
                       

                        itemBuscado = new()
                        {
                            PrecioUnitConDto = precio,
                            Cantidad = itemCarrito.Cantidad,
                            ProductoId = producto.Id,
                            Productos = [producto],
                            CarritoId = carrito.Id
                        };

                        _context.Add(itemBuscado);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        itemBuscado.Cantidad += itemCarrito.Cantidad;
                        _context.Update(itemBuscado);
                        await _context.SaveChangesAsync();
                    }
                }

                //modificar el stock de producto
                // toDo stock
                producto.Strock -= itemCarrito.Cantidad;
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Carritos");
        }

        // GET: ItemsCarrito/Edit/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCarrito = await _context.ItemCarrito.FindAsync(id);
            if (itemCarrito == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", itemCarrito.CarritoId);
            return View(itemCarrito);
        }

        // POST: ItemsCarrito/Edit/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrecioUnitConDto,Cantidad,CarritoId,ProductoId")] ItemCarrito itemCarrito)
        {
            if (id != itemCarrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemCarrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemCarritoExists(itemCarrito.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", itemCarrito.CarritoId);
            return View(itemCarrito);
        }

        // GET: ItemsCarrito/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCarrito = await _context.ItemCarrito
                .Include(i => i.Carrito)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemCarrito == null)
            {
                return NotFound();
            }

            return View(itemCarrito);
        }

        // POST: ItemsCarrito/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemCarrito = await _context.ItemCarrito.FindAsync(id);

            Producto producto = await _context.Producto.FindAsync(itemCarrito.ProductoId);

            var carrito = await _context.Carrito.Include(c => c.ItemsCarrito).FirstOrDefaultAsync(c => c.Id == itemCarrito.CarritoId);


            if (itemCarrito == null)
            {
                return NotFound();
            }


            if(itemCarrito.Cantidad > 0)
            {
                itemCarrito.Cantidad--;
                producto.Strock++;
                if (itemCarrito.Cantidad == 0)
                {
                    _context.ItemCarrito.Remove(itemCarrito);

                    bool isCarritoEmpty = !carrito.ItemsCarrito.Any(i => i.Cantidad > 0);

                    if (isCarritoEmpty)
                    {
                        _context.Carrito.Remove(carrito);
                    }
                }
                else
                {
                    _context.Update(itemCarrito);
                }
            }
            
            _context.Update(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Carritos");
        }

        private bool ItemCarritoExists(int id)
        {
            return _context.ItemCarrito.Any(e => e.Id == id);
        }
    }
}
