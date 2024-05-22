using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TestApi.Models.DTOs;
using TestApi.Models.UserManager;

namespace TestApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // Define your DbSets here
        public DbSet<EmployeeUsers> emp_users { get; set; }
        public DbSet<Groups> Groups { get; set; }
       public DbSet<UserGroups> UserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure Identity entities have primary keys
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });


            modelBuilder.Entity<UserGroups>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserGroups>()
                .HasOne(ug => ug.EmpUser)
                .WithMany(u => u.User_Groups)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroups>()
                .HasOne(ug => ug.user_Groups)
                .WithMany(g => g.User_Groups)
                .HasForeignKey(ug => ug.GroupId);
        }



    }
}
