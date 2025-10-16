using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Define your DbSets here, for example:
    // public DbSet<User> Users { get; set; } ....
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entity mappings here
    }
}
