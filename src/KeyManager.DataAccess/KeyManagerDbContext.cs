using KeysManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyManager.DataAccess;

public sealed class KeyManagerDbContext : DbContext
{
    public KeyManagerDbContext(DbContextOptions<KeyManagerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}