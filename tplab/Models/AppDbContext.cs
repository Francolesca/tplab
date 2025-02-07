using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
using tplab.Models.Entidades;

namespace tplab.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasIndex(c => c.Nombre).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(c => c.Username).IsUnique();


        }
    }
}
