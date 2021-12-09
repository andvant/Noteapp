using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Noteapp.Core.Entities;
using System;
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

            // setting DateTimeKind.Utc on all DateTime values retrieved from the database
            var dateTimeUtcConverter = new ValueConverter<DateTime, DateTime>(date => date,
                date => DateTime.SpecifyKind(date, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeUtcConverter);
                    }
                }
            }
        }
    }
}
