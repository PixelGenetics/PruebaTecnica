using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Category { get; set; } = null!;
    public DbSet<Product> Product { get; set; } = null!;
    public DbSet<MovInv> MovInv { get; set; } = null!;
    public DbSet<Usuario> Usuario { get; set; } = null!;

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(
            isolationLevel,
            cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .ToTable("Category");

        modelBuilder.Entity<Product>()
            .ToTable("Product");

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(usuario => usuario.Id);

            entity.Property(usuario => usuario.NombreUsuario)
                .HasColumnName("nombreUsuario")
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(usuario => usuario.NombreUsuario)
                .IsUnique();

            entity.Property(usuario => usuario.Contrasenia)
                .HasColumnName("contrasenia")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(usuario => usuario.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(usuario => usuario.Correo)
                .HasColumnName("correo")
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(usuario => usuario.Correo)
                .IsUnique();

            entity.Property(usuario => usuario.Estado)
                .HasColumnName("estado")
                .HasDefaultValue(true)
                .IsRequired();
        });

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