using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using _2024_2C_SushiPOP_G6.Models.ViewModels;
using static _2024_2C_SushiPOP_G6.Controllers.PedidosController;

namespace _2024_2C_SushiPOP_G6.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Pedidos

        [Authorize(Roles = "CLIENTE,EMPLEADO")]
        public async Task<IActionResult> Index()
        {
            List<Pedido> pedidos = new List<Pedido>();
            if (User.IsInRole("CLIENTE"))
            {
                var usuarioLogueado = await _userManager.GetUserAsync(User);
                var email = usuarioLogueado.Email;

                pedidos = await _context.Pedido
                    .Include(p => p.Carrito)
                    .ThenInclude(c => c.Cliente)
                    .Where(p => p.Carrito.Cliente.Email == email && p.Estado != 6)
                    .ToListAsync();
            }
            else if (User.IsInRole("EMPLEADO"))
            {
                pedidos = await _context.Pedido
                    .Include(p => p.Carrito)
                    .ThenInclude(c => c.Cliente)
                    .Where(p =>p.Estado != 6).ToListAsync();
            }
                
             

            var estados = Enum.GetValues(typeof(Estado))
                .Cast<Estado>()
                .OrderBy(e => (int)e)
                .Select(e => e.GetType()
                              .GetMember(e.ToString())
                              .First()
                              .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString())
                .ToArray();

            ViewData["Estados"] = estados;

            return View(pedidos);
        }


        public enum Estado
        {
            [Display(Name = "Sin Confirmar")]
            SinConfirmar = 1,

            [Display(Name = "Confirmado")]
            Confirmado = 2,

            [Display(Name = "En Preparación")]
            EnPreparacion = 3,

            [Display(Name = "En Reparto")]
            EnReparto = 4,

            [Display(Name = "Entregado")]
            Entregado = 5,

            [Display(Name = "Cancelado")]
            Cancelado = 6
        }

        // GET: Pedidos/Details/5
        [Authorize(Roles = "CLIENTE,EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Carrito)
                .Include(p => p.Carrito.Cliente)
                .Include(p => p.Carrito.ItemsCarrito)
                .ThenInclude(ic => ic.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null || pedido.Carrito == null || pedido.Carrito.ItemsCarrito == null)
            {
                return NotFound();
            }

            return View(pedido);
        }


        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Create([Bind()] Pedido pedido)
        {

            var usuarioLogueado = await _userManager.GetUserAsync(User);

            Pedido pedidoSinConfirmar = await _context.Pedido
                .FirstOrDefaultAsync(p => p.Carrito.Cliente.Email == usuarioLogueado.Email && p.Estado == 1);



            Carrito carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .Include(c => c.ItemsCarrito)
                .ThenInclude(ci => ci.Productos)
                .FirstOrDefaultAsync(c => c.Cliente.Email == usuarioLogueado.Email && !c.Procesado && !c.Cancelado);

            if (carrito != null && pedidoSinConfirmar == null)
            {
                pedido = await CrearPedido(carrito);
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = pedido.Id });
            }

            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            return RedirectToAction(nameof(Index));
        }

        private async Task<Pedido> CrearPedido(Carrito carrito)
        {
            decimal subTotal = 0;
            foreach (ItemCarrito itemCarrito in carrito.ItemsCarrito)
            {
                subTotal += itemCarrito.PrecioUnitConDto * itemCarrito.Cantidad;
            }

            Pedido ultimoPedido = _context.Pedido
                             .OrderByDescending(p => p.NroPedido)
                             .FirstOrDefault();
            int nroPedido;
            if (ultimoPedido != null)
            {
                nroPedido = ultimoPedido.NroPedido + 1;
            }
            else
            {
                nroPedido = 3000;
            }

            decimal gastoEnvio = 0;

            Pedido pedido = new Pedido
            {
                NroPedido = nroPedido,
                FechaCompra = DateTime.Now,
                Subtotal = subTotal,
                GastoEnvio = gastoEnvio, //llenar
                Total = subTotal + gastoEnvio,
                Estado = 1,
                CarritoId = carrito.Id,
                Carrito = carrito
            };
            return pedido;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> FinalizarPedido(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);

            if (pedido != null)
            {
                pedido.Estado = 2;
                _context.Update(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> EditEstado(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            //  lista de estados incrementales posibles
            var estadosIncrementales = Enumerable.Range(pedido.Estado + 1, 6 - pedido.Estado)
                .Select(e => new { Value = e, Text = EstadoToString(e) });

            ViewData["Estados"] = new SelectList(estadosIncrementales, "Value", "Text");

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]

        public async Task<IActionResult> EditEstado(int id, int nuevoEstado)
        {
            var pedido = await _context.Pedido.FindAsync(id);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index)); // Redirige al listado
            }



            if (nuevoEstado <= pedido.Estado) //verifico que el cambio de estado sea incremental
            {
                TempData["ErrorMessage"] = "No puedes retroceder el estado del pedido. Solo puedes avanzar a un estado superior.";
                return RedirectToAction(nameof(Index));
            }


            pedido.Estado = nuevoEstado; //si cumple la condicion, actualizo estado.

            try
            {
                _context.Update(pedido);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estado del pedido actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hubo un error al actualizar el estado del pedido: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool PedidoExists(int id)
        {
            return _context.Pedido.Any(e => e.Id == id);
        }
        private string EstadoToString(int estado)
        {
            return estado switch
            {
                1 => "Sin Confirmar",
                2 => "Confirmado",
                3 => "En Preparación",
                4 => "En Reparto",
                5 => "Entregado",
                6 => "Cancelado",
                _ => "Desconocido"
            };
        }

        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);

            if (pedido == null || pedido.Estado != 1)
            {
                TempData["ErrorMessage"] = "Solo se pueden cancelar los pedidos que estan 'Sin confirmar'.";
                return RedirectToAction(nameof(Index));
            }

            pedido.Estado = 6;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //SeguirPedido
        [Authorize(Roles = "CLIENTE,EMPLEADO")]
        public async Task<IActionResult> SeguirPedido(int? nroPedido)
        {

            var usuarioLogueado = await _userManager.GetUserAsync(User);
            var email = usuarioLogueado.Email;

            var pedido = await _context.Pedido
                .Include(p=>p.Carrito)
                .Include(c=>c.Carrito.Cliente)
                .FirstOrDefaultAsync(p => p.NroPedido == nroPedido);

            if (nroPedido != null && pedido != null && pedido.Carrito.Cliente.Email == email)
            {
                string estadoIcono = $"/imagenes/estados/estado{pedido.Estado}.png";
                Estado[] estados = (Estado[])Enum.GetValues(typeof(Estado));
                Estado estadoActual = estados[pedido.Estado - 1];

                var model = new SeguirPedidoViewModel
                {
                    NroPedido = pedido.NroPedido,
                    Estado = estadoActual.ToString(),
                    EstadoIcono = estadoIcono
                };

                return View(model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> SeguirPedido(int nroPedido)
        {
            var pedido = await _context.Pedido.FirstOrDefaultAsync(p => p.NroPedido == nroPedido);

            if (pedido == null)
            {
                ViewData.ModelState.AddModelError("NroPedido", "Pedido no encontrado");
                return View();
            }

            string estado = pedido.Estado switch
            {
                1 => "Sin confirmar",
                2 => "Confirmado",
                3 => "En preparación",
                4 => "En reparto",
                5 => "Entregado",
                6 => "Cancelado",
                _ => "Desconocido"
            };

            string estadoIcono = $"/imagenes/estados/estado{pedido.Estado}.png";

            var model = new SeguirPedidoViewModel
            {
                NroPedido = pedido.NroPedido,
                Estado = estado,
                EstadoIcono = estadoIcono
            };

            return View(model);
        }


        private string MapEstadoTexto(int estado)
        {
            return estado switch
            {
                1 => "Sin confirmar",
                2 => "Confirmado",
                3 => "En preparación",
                4 => "En reparto",
                5 => "Entregado",
                6 => "Cancelado",
                _ => "Desconocido"
            };
        }

        private string MapEstadoIcono(int estado)
        {
            return estado switch
            {
                1 => "/imagenes/estados/estado1.png",
                2 => "/imagenes/estados/estado2.png",
                3 => "/imagenes/estados/estado3.png",
                4 => "/imagenes/estados/estado4.png",
                5 => "/imagenes/estados/estado5.png",
                6 => "/imagenes/estados/estado6.png",
                _ => "/imagenes/estados/estado1.png"
            };
        }
    }



}