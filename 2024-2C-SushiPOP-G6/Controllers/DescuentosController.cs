using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using _2024_2C_SushiPOP_G6.Models.ViewModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class DescuentosController : Controller
    {
        private readonly DbContext _context;

        public DescuentosController(DbContext context)
        {
            _context = context;
        }

        // GET: Descuentos
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Index()
        {
            var descuentos = await _context.Descuento
                .Include(d => d.Producto)
                .Select(d => new DescuentoViewModel
                {
                    Id = d.Id,
                    Dia = d.Dia,
                    Porcentaje = d.Porcentaje,
                    DescuentoMax = d.DescuentoMax,
                    Activo = d.Activo,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre
                }).ToListAsync();

            return View(descuentos);
        }


        // GET: Descuentos/Create

        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Create()
        {
            var viewModel = await BuildDescuentoViewModel();

            return View(viewModel);
        }

        // POST: Descuentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Create([Bind("Id,Dia,Porcentaje,DescuentoMax,ProductoId")] Descuento descuento)
        {
            if (ModelState.IsValid)
            {
                if (!HayDescuentoElMismoDiaActivo(descuento.Dia)) {
                    _context.Add(descuento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Dia", "Ya existe un descuento activo para este día.");
                }
            }

            var viewModel = await BuildDescuentoViewModel(descuento);
            return View(viewModel);
        }

        private async Task<DescuentoViewModel> BuildDescuentoViewModel(Descuento descuento = null)
        {
            var productos = await _context.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToListAsync();

            return new DescuentoViewModel
            {
                Id = descuento?.Id ?? 0,
                Dia = descuento?.Dia,
                Porcentaje = descuento?.Porcentaje,
                DescuentoMax = descuento?.DescuentoMax ?? 0,
                ProductoId = descuento?.ProductoId ?? 0,
                Productos = productos
            };
        }


        private bool HayDescuentoElMismoDiaActivo(int dia)
        {
            return _context.Descuento.Any(d => d.Dia == dia && d.Activo == true);
        }

        // GET: Descuentos/Edit/5

        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var descuento = await _context.Descuento
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (descuento == null)
            {
                return NotFound();
            }


            var viewModel = await BuildDescuentoViewModel(descuento);

            return View(viewModel);
        }

        // POST: Descuentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dia,Porcentaje,DescuentoMax,Activo,ProductoId")] Descuento descuento)
        {
            if (id != descuento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descuento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescuentoExists(descuento.Id))
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
            ViewData["ProductoId"] = new SelectList(_context.Producto, "Id", "Id", descuento.ProductoId);
            return View(descuento);
        }

        // GET: Descuentos/Delete/5

        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuento
                .Include(d => d.Producto)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (descuento == null)
            {
                return NotFound();
            }

            var viewModel = new DescuentoViewModel
            {
                Id = descuento.Id,
                Dia = descuento.Dia,
                NombreProducto = descuento.Producto?.Nombre
            };

            return View(viewModel);
        }

        // POST: Descuentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var descuento = await _context.Descuento.FindAsync(id);
            if (descuento != null)
            {
                _context.Descuento.Remove(descuento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescuentoExists(int id)
        {
            return _context.Descuento.Any(e => e.Id == id);
        }
    }
}
