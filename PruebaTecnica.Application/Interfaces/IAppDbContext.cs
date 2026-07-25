using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Category { get; }

    DbSet<Product> Product { get; }

    DbSet<MovInv> MovInv { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}