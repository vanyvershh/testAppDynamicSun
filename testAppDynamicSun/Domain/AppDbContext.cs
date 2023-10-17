using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace testAppDynamicSun.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WeatherValue> WeatherValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)//добавляемм одну новую запись
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherValue>().HasData(new WeatherValue
            {
                Id = new Guid("716C2E99-6F6C-4472-81A5-43C56E11637C"),
                WindDirection = "winddirection test",
                Date = new DateTime(2010, 1, 1),
                Time = "00:00"
            });
        }
    }
}
