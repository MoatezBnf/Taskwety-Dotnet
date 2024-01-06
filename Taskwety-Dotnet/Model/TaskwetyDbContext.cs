using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taskwety_Dotnet.Model;

namespace Taskwety_Dotnet.Model
{
    public class TaskwetyDbContext : IdentityDbContext<IdentityUser>
    {
        public TaskwetyDbContext(DbContextOptions<TaskwetyDbContext> options) : base(options)
        {
        }
        public DbSet<Taskwety_Dotnet.Model.UserModel>? UserModel { get; set; }
        public DbSet<Taskwety_Dotnet.Model.TaskModel>? TaskModel { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
                (
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
                );
        }
    }
}