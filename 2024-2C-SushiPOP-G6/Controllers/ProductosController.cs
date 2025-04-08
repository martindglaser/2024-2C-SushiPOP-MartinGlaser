using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using Microsoft.AspNetCore.Authorization;
using _2024_2C_SushiPOP_G6.Models.ViewModels;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class ProductosController : Controller
    {
        private readonly DbContext _context;

        public ProductosController(DbContext context)
        {
            _context = context;
        }

        //GET: Productos
        //public async Task<IActionResult> Index()
        //{
        //    var dbContext = _context.Producto.Include(p => p.Categoria);
        //    return View(await dbContext.ToListAsync());
        //}

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var dbContext = _context.Producto.Include(p => p.Categoria);
                return View(await dbContext.ToListAsync());
            }
            else
            {
                var dbContext = _context.Producto
                .Where(p => p.CategoriaId == id)
                .Include(p => p.Categoria);

                var categoria = await _context.Categoria
                .FirstOrDefaultAsync(c => c.Id == id);

                if (categoria != null)
                {
                    ViewData["Title"] = categoria.Nombre;
                }
                else
                {
                    ViewData["Title"] = "Productos";
                }
                return View(await dbContext.ToListAsync());
            }

        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Id");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "EMPLEADO")] // RN24.Solo usuarios empleados pueden crear productos.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Foto,Strock,Costo,CategoriaId")] Producto producto)
        {
            if (await ExisteNombre(producto.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
            }
            if (producto.Foto != null && await ExisteFoto(producto.Foto))
            {
                ModelState.AddModelError("Foto", "No puede utilizarse la misma foto para más de un producto");
            }
            if (ModelState.IsValid)
            {
                //Si el producto se carga sin foto, se asigma el avatar de estándar
                if (producto.Foto == null)
                {
                    producto.Foto = "DEFAULT.png";
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Id", producto.CategoriaId);

            return View(producto);
        }

        public async Task<bool> ExisteNombre(string nombre)
        {
            //RN25.El nombre del producto es único.
            return await _context.Producto.AnyAsync(c => c.Nombre == nombre);
        }
        public async Task<bool> ExisteFoto(string foto)
        {
            //RN26.No puede utilizarse la misma foto para más de un producto.
            return await _context.Producto.AnyAsync(c => c.Foto == foto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Id", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "EMPLEADO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,Foto,Strock,Costo,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Id", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                _context.Producto.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.Id == id);
        }
    }
}
