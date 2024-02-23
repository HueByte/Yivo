using Microsoft.EntityFrameworkCore;
using Yivo.Core.Models;

namespace Yivo.Infrastructure;

public class YivoContext : DbContext
{
    public YivoContext(DbContextOptions<YivoContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(u => u.CommandLogs)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<CommandLog> CommandLogs { get; set; } = default!;
}