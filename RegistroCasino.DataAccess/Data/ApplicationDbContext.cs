using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegistroCasino.Models;

namespace RegistroCasino.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Plataforma> Plataforma { get; set; }
        public DbSet<Cajeros> Cajero { get; set; }
    }
}
