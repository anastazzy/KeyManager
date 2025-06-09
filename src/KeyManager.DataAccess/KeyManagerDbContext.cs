using Microsoft.EntityFrameworkCore;

namespace KeyManager.DataAccess;

public sealed class KeyManagerDbContext : DbContext
{
    public KeyManagerDbContext(DbContextOptions<KeyManagerDbContext> options) : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}