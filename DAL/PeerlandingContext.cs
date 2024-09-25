using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace DAL;

public partial class PeerlandingContext : DbContext
{
    public PeerlandingContext()
    {
    }

    public PeerlandingContext(DbContextOptions<PeerlandingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MstUser> MstUsers { get; set; }
    public virtual DbSet<MstLoans> MstLoans {  get; set; }

    public virtual DbSet<TrnFunding> TrnFundings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MstUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mst_user_pkey");

            entity.ToTable("mst_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasPrecision(18, 2)
                .HasColumnName("balance");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
        });

        modelBuilder.Entity<MstLoans>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mst_loans_pkey");

            entity.ToTable("mst_loans");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("Id");

            entity.Property(e => e.BorrowerId)
                .IsRequired()
                .HasColumnName("borrower_id");

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");

            entity.Property(e => e.InterestRate)
                .HasPrecision(5, 2)
                .HasColumnName("interest_rate");

            entity.Property(e => e.Duration)
                .IsRequired()
                .HasColumnName("duration");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("status")
                .HasDefaultValue("requested");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.HasOne(d => d.User)
                .WithMany(p => p.MstLoans)
                .HasForeignKey(d => d.BorrowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mst_loans_mst_user");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
