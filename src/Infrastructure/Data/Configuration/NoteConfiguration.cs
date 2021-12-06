using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noteapp.Core.Entities;

namespace Noteapp.Infrastructure.Data.Configuration
{
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasMany(p => p.Snapshots)
                .WithOne(s => s.Note)
                .HasForeignKey(s => s.NoteId);

            builder.HasOne(p => p.CurrentSnapshot)
                .WithOne()
                .HasForeignKey<Note>(p => p.CurrentSnapshotId);
        }
    }
}
