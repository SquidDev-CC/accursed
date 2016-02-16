using System.IO;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;

namespace Accursed.Models
{
    public class AccursedDbContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        public DbSet<ModVersion> Versions { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = PlatformServices.Default.Application.ApplicationBasePath;
            optionsBuilder.UseSqlite("Filename=" + Path.Combine(path, "Accursed.db"));
        }
    }
}
