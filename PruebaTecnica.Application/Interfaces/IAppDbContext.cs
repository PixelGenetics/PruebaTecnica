using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace PruebaTecnica.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Category { get; }

    DbSet<Product> Product { get; }

    DbSet<MovInv> MovInv { get; }
    DbSet<Usuario> Usuario { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(
    IsolationLevel isolationLevel,
    CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}