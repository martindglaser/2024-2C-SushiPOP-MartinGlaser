using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _2024_2C_SushiPOP_G6.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class DbContext : IdentityDbContext
    {
        public DbContext (DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<_2024_2C_SushiPOP_G6.Models.Categoria> Categoria { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Cliente> Cliente { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Producto> Producto { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Carrito> Carrito { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Contacto> Contacto { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Descuento> Descuento { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Empleado> Empleado { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.ItemCarrito> ItemCarrito { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Pedido> Pedido { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Reclamo> Reclamo { get; set; } = default!;

public DbSet<_2024_2C_SushiPOP_G6.Models.Reserva> Reserva { get; set; } = default!;
    }
