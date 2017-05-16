using InventoryAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Infrastructure
{
  public class InventoryContext : DbContext
  {
    public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
    {      
    }

    public DbSet<Inventory> Inventories { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.Entity<Product>(ConfigureProduct);
      builder.Entity<Inventory>(ConfigureInventory);
    }

    void ConfigureProduct(EntityTypeBuilder<Product> builder)
    {
      builder.ToTable("Product");
      builder.Property(p => p.Id)
        .ForSqlServerUseSequenceHiLo("product_hilo")
        .IsRequired();
      builder.Property(p => p.Name)
        .IsRequired()
        .HasMaxLength(100);
      builder.Property(p => p.Price)
        .IsRequired();
    }

    void ConfigureInventory(EntityTypeBuilder<Inventory> builder)
    {
      builder.ToTable("Inventory");
      builder.Property(i => i.Id)
        .ForSqlServerUseSequenceHiLo("inventory_hilo")
        .IsRequired();
      builder.Property(i => i.LocM)
        .IsRequired();
      builder.Property(i => i.Quantity)
        .IsRequired();
      builder
        .HasOne(i => i.Product)        
        .WithOne()
        .HasForeignKey<Product>(i => i.InventoryRef);
    }
  }
}
