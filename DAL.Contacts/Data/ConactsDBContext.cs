using System;
using System.Collections.Generic;
using DAL.Contacts.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.Data;

public partial class ConactsDBContext : DbContext
{
    public ConactsDBContext()
    {
    }

    public ConactsDBContext(DbContextOptions<ConactsDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactType> ContactTypes { get; set; }

    public virtual DbSet<ErrorCode> ErrorCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=rutvik10@#;Server=localhost;Port=5432;Database=Contacts;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Contacts_pkey");

            entity.HasOne(d => d.ContactType).WithMany(p => p.Contacts).HasConstraintName("contact_type_fkey");
        });

        modelBuilder.Entity<ContactType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ContactTypes_pkey");
        });

        modelBuilder.Entity<ErrorCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ErrorCodes_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
