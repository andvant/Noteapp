using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noteapp.Core;
using Noteapp.Core.Entities;

namespace Noteapp.Infrastructure.Data.Configuration
{
    public class NoteSnapshotConfiguration : IEntityTypeConfiguration<NoteSnapshot>
    {
        public void Configure(EntityTypeBuilder<NoteSnapshot> builder)
        {
            builder.Property(snapshot => snapshot.Text)
                .HasMaxLength(Constants.MAX_TEXT_LENGTH);
        }
    }
}
