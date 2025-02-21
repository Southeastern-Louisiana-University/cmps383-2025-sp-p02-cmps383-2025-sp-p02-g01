using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Features.Theaters;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Theaters.Roles;
using Selu383.SP25.P02.Api.Features.UserRoles;
using Microsoft.AspNetCore.Identity;

namespace Selu383.SP25.P02.Api.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure UserRole relationship
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.Roles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }

        public DbSet<Theater> Theaters { get; set; }
    }
}