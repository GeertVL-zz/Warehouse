using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using InventoryAPI.Infrastructure;

namespace InventoryAPI.Migrations
{
    [DbContext(typeof(InventoryContext))]
    partial class InventoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:Sequence:.inventory_hilo", "'inventory_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:Sequence:.product_hilo", "'product_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InventoryAPI.Model.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "inventory_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<double>("Blocked");

                    b.Property<string>("BlockedReason");

                    b.Property<DateTime>("ExpiredOn");

                    b.Property<int>("LocM");

                    b.Property<double>("Quantity");

                    b.Property<double>("Reserved");

                    b.HasKey("Id");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("InventoryAPI.Model.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "product_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Description");

                    b.Property<double>("Height");

                    b.Property<int>("InventoryRef");

                    b.Property<double>("Length");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<double>("Price");

                    b.Property<int>("ProductGroup");

                    b.Property<string>("Supplier");

                    b.Property<string>("UnitMeasure");

                    b.Property<double>("Volume");

                    b.Property<double>("Weight");

                    b.Property<double>("Width");

                    b.HasKey("Id");

                    b.HasIndex("InventoryRef")
                        .IsUnique();

                    b.ToTable("Product");
                });

            modelBuilder.Entity("InventoryAPI.Model.Product", b =>
                {
                    b.HasOne("InventoryAPI.Model.Inventory")
                        .WithOne("Product")
                        .HasForeignKey("InventoryAPI.Model.Product", "InventoryRef")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
