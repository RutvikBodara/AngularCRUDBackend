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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactType> ContactTypes { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<ErrorCode> ErrorCodes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Category_pkey");
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

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Country_pkey");
        });

        modelBuilder.Entity<ErrorCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ErrorCodes_pkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pkey");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
