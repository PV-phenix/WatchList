using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WatchList.Models;

namespace WatchList.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Film> Films { get; set; }
        public DbSet<FilmUtilisateur> FilmsUtilisateur { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FilmUtilisateur>()
            .HasKey(t => new { t.IdUtilisateur, t.IdFilm });
        }

        public DbSet<WatchList.Models.ModeleVueFilm> ModeleVueFilm { get; set; } = default!;
    }
}