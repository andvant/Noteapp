using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;
using System.Reflection;

namespace Noteapp.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<NoteSnapshot> NoteSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
