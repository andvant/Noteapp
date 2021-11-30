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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>()
                .HasMany(p => p.Snapshots)
                .WithOne(s => s.Note)
                .HasForeignKey(s => s.NoteId);

            modelBuilder.Entity<Note>()
                .HasOne(p => p.CurrentSnapshot)
                .WithOne()
                .HasForeignKey<Note>(p => p.CurrentSnapshotId);
        }
    }
}
