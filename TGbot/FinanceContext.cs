using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TGbot;

public partial class FinanceContext : DbContext
{
    public FinanceContext()
    {
    }

    public FinanceContext(DbContextOptions<FinanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ItemsBuy> ItemsBuys { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Finance;Data Source=DESKTOP-3E6ME6N;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemsBuy>(entity =>
        {
            entity.HasKey(e => e.IdItemsBuy);

            entity.ToTable("ItemsBuy");

            entity.Property(e => e.IdItemsBuy).HasColumnName("idItemsBuy");
            entity.Property(e => e.IdOperationItemsBuy).HasColumnName("idOperation_ItemsBuy");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Sum).HasColumnType("money");

            entity.HasOne(d => d.IdOperationItemsBuyNavigation).WithMany(p => p.ItemsBuys)
                .HasForeignKey(d => d.IdOperationItemsBuy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItemsBuy_Operations");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.IdOperations).HasName("PK_Buy");

            entity.Property(e => e.IdOperations).HasColumnName("idOperations");
            entity.Property(e => e.BuyerPhoneOrAddress)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.EcashTotalSum).HasColumnName("ECashTotalSum");
            entity.Property(e => e.FiscalDriveNumber)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.KktRegId)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.MachineNumber)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.SellerAddress)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.UserInn)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Userid)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
