using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service;

public class UserContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().Ignore(u => u.LockoutEnabled)
            .Ignore(u => u.TwoFactorEnabled)
            .Ignore(u => u.PhoneNumber)
            .Ignore(u => u.PhoneNumberConfirmed)
            .Ignore(u => u.ConcurrencyStamp)
            .Ignore(u => u.AccessFailedCount)
            .Ignore(u => u.LockoutEnd);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Conventions.Add(_ => new SnakeCaseConvention());
    }
}