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
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace _2024_2C_SushiPOP_G6.Controllers
{

    public class ContactosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContactosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Contactos/Details/5

        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Index()
        {
            //para que solo me aparezcan los que no lei (Leido = false)
            var mensajesSinLeer = await _context.Contacto
                .Where(c => !c.Leido)
                .ToListAsync();

            return View(mensajesSinLeer); 
        }

         [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var contacto = await _context.Contacto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contacto == null)
            {
                return NotFound();
            }

            // Marcoo mensaje como leído
            if (!contacto.Leido)
            {
                contacto.Leido = true; // Cambia el flag a true
                _context.Update(contacto); // Actualiza el estado en el contexto
                await _context.SaveChangesAsync(); // Guarda los cambios
            }

            return View(contacto); // Devuelve la vista con el mensaje
        }

        // FIN DE MI EDIT

        // GET: Contactos/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ContactoCliente
            {
                NombreCompleto = "",
                Email = "",
                Telefono = "",
                Mensaje = "",
                Leido = false
            };

            if (User.IsInRole("CLIENTE"))
            {
                 viewModel = await BuildContactoCliente();
            }
            return View(viewModel);
        }

        private async Task<ContactoCliente> BuildContactoCliente()
        {
            var usuario = await _userManager.GetUserAsync(User);
            //var rol = await _userManager.GetRolesAsync(usuario);

            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == usuario.Email.ToUpper());

            if (cliente == null)
            {
                // Manejar el caso donde no se encuentra el cliente relacionado con el usuario
                // Por ejemplo, redirigir o mostrar un mensaje de error
                return null;
            }

            // Crear el objeto ContactoClienteViewModel con el Cliente y Contacto relacionados
            var contactoCliente = new ContactoCliente
            {
                NombreCompleto = cliente?.Nombre,
                Email = cliente?.Email,
                Telefono = cliente?.Telefono,
                Mensaje = "",
                Leido = false
            };

            return contactoCliente;
        }


        // POST: Contactos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,NombreCompleto,Email,Telefono,Mensaje,Leido")] Contacto contacto)
		{
            if (ModelState.IsValid)
            {
                _context.Add(contacto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(new ContactoCliente
            {
                NombreCompleto = contacto.NombreCompleto,
                Email = contacto.Email,
                Telefono = contacto.Telefono,
                Mensaje = contacto.Mensaje,
                Leido = false
            });
		}

		// GET: Contactos/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto == null)
            {
                return NotFound();
            }
            return View(contacto);
        }

        // POST: Contactos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Telefono,Mensaje,Leido")] Contacto contacto)
        {
            if (id != contacto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contacto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactoExists(contacto.Id))
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
            return View(contacto);
        }

        // GET: Contactos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contacto == null)
            {
                return NotFound();
            }

            return View(contacto);
        }

        // POST: Contactos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto != null)
            {
                _context.Contacto.Remove(contacto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactoExists(int id)
        {
            return _context.Contacto.Any(e => e.Id == id);
        }
    }
}
