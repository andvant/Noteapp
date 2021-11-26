using Microsoft.EntityFrameworkCore;
using Noteapp.Core.Entities;

namespace Noteapp.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<NoteSnapshot> NoteSnapshots { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}
