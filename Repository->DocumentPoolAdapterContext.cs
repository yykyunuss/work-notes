using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Fctr.Edison.FileAdapter.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Fctr.Edison.FileAdapter.Repositories
{
    [ExcludeFromCodeCoverage]
    public class DocumentPoolAdapterContext : DbContext
    {

        public DbSet<DocumentPoolAdapter> DocumentPoolAdapters { get; set; }

        public DocumentPoolAdapterContext(DbContextOptions<DocumentPoolAdapterContext> options) : base(options)
        {

        }

        public DocumentPoolAdapterContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentPoolAdapter>(entity =>
                {
                    entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator((_, __) => new SequenceValueGenerator("FACTORING", "SEQ_DOCUMENT_POOL_ADAPTER"));
                }
            );

        }

    }
}
