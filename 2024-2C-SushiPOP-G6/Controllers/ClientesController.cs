using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using _2024_2C_SushiPOP_G6.Models.ViewModels;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class ClientesController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientesController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clientes
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cliente.ToListAsync());
        }

        // GET: Clientes/Details/5
        [Authorize(Roles = "CLIENTE,EMPLEADO,ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect fromoverposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Nombre,Apellido,Direccion,Telefono,FechaNacimiento,Email,Clave")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Crear cliente en Identity
                IdentityUser user = new IdentityUser();
                user.Email = cliente.Email;
                user.UserName = cliente.Email;
                var result = await _userManager.CreateAsync(user, cliente.Clave);

                // Asignar Rol
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "CLIENTE");
                    //ESTO NO VA ACA. LE DEBE ASIGNAR LEGAJO DESPUES DE INICIAR POR PRIMERA VEZ
                    if (_context.Cliente.Any())
                    {
                        int max = _context.Cliente.Max(i => i.NumeroCliente);
                        cliente.NumeroCliente = max + 1;
                    }
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = "CLIENTE,EMPLEADO,ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE,EMPLEADO,ADMIN")]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,Nombre,Apellido,Direccion,Telefono,FechaNacimiento,Email")] Cliente cliente
         )
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "CLIENTE")]
        //public async Task<IActionResult> Edit(int id, [Bind("NumeroCliente,Telefono,Email")] EditCliente cliente)
        //{
        //    if (id != cliente.NumeroCliente)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(cliente);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ClienteExists(cliente.NumeroCliente))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(cliente);
        //}

        // GET: Clientes/Delete/5
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO,ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var cliente = await _context.Cliente.FindAsync(id);
            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente != null)
            {
                _context.Cliente.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.Id == id);
        }
    }
}
