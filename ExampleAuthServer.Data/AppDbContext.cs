using ExampleAuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Data
{
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string> //Identity ile ilgili olan DbSetler IdentityDbContext'te
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly); // Tüm configurationları otomatik Assembly get olarak alır.
            base.OnModelCreating(builder);
        }
    }
}
