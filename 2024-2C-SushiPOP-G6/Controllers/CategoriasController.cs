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
    public class CategoriasController : Controller
    {
        private readonly DbContext _context;

        public CategoriasController(DbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categoria.ToListAsync();
            var categoriasViewModel = new List<IndexCategoria>();

            foreach (var categoria in categorias)
            {
                var tieneProductos = await TieneProductos(categoria.Id);
                categoriasViewModel.Add(new IndexCategoria
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    TieneProductos = tieneProductos
                });
            }

            return View(categoriasViewModel);
        }

        private async Task<bool> TieneProductos(int ? categoriaId)
        {
            return await _context.Producto.AnyAsync(p => p.CategoriaId == categoriaId);
        }


        // GET: Categorias/Create

        [Authorize(Roles = "EMPLEADO")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (await ExisteCategoria(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
            }
            if (ModelState.IsValid)
            {
                      
                    _context.Add(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public async Task<bool> ExisteCategoria(string nombre)
        {
            return await _context.Categoria.AnyAsync(c => c.Nombre == nombre);
        }

        // GET: Categorias/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion")] EditCategoria editCategoria)
        {
            if (id != editCategoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var categoria = await _context.Categoria.FindAsync(id);
                    if (categoria == null)
                    {
                        return NotFound();
                    }

                    categoria.Descripcion = editCategoria.Descripcion;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(editCategoria.Id))
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
            return View(editCategoria);
        }

        // GET: Categorias/Delete/5
        [Authorize(Roles = "EMPLEADO")]

        public async Task<IActionResult> Delete(int? id)
        {
            var tieneProductos = await TieneProductos(id);
            if (tieneProductos)
            {
                return RedirectToAction(nameof(Index));
            }

            if (id == null || tieneProductos)
            {
                return NotFound();
            }

            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tieneProductos = await _context.Producto.AnyAsync(p => p.CategoriaId == id);
            if (tieneProductos)
            {
                return RedirectToAction(nameof(Index));
            }


            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria != null)
            {
                _context.Categoria.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categoria.Any(e => e.Id == id);
        }
    }
}
