using PruebaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace PruebaTecnica.Infrastructure.Persistence;
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<MovInv> MovInv { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .ToTable("Category");

            modelBuilder.Entity<Product>()
                .ToTable("Product");

            modelBuilder.Entity<MovInv>(entity =>
            {
                entity.ToTable("movInv");

                entity.HasKey(movement => movement.Id);

                entity.Property(movement => movement.Tipo)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(movement => movement.Cantidad)
                    .IsRequired();

                entity.Property(movement => movement.Fecha)
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(movement => movement.Referencia)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasOne(movement => movement.Product)
                    .WithMany(product => product.Movimientos)
                    .HasForeignKey(movement => movement.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(table =>
                {
                    table.HasCheckConstraint(
                        "ck_movInv_tipo",
                        "[Tipo] IN ('Entrada', 'Salida')");

                    table.HasCheckConstraint(
                        "ck_movInv_cantidad",
                        "[Cantidad] > 0");
                });
            });
        }
    }
