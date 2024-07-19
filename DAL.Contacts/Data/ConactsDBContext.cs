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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactType> ContactTypes { get; set; }

    public virtual DbSet<ErrorCode> ErrorCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Accounts_pkey");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AccountType_pkey");
        });

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
