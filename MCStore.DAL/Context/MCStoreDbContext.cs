using MCStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.Context
{
    public class MCStoreDbContext : DbContext
    {
        public MCStoreDbContext(DbContextOptions<MCStoreDbContext> options)
                : base(options)
        {


        }

        public DbSet<Product> Product { get; set; }

        public DbSet<Invoice> Invoice { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<Stocks> Stocks { get; set; }

        public DbSet<CashInCashOutFee> CashInCashOutFee { get; set; }

        public DbSet<ProductDiscount> ProductDiscount { get; set; }

        public DbSet<WalletBalance> WalletBalance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add shadow audit properties to all entity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // skip owned types or types without CLR type
                if (entityType.IsOwned() || entityType.ClrType == null) continue;

                // CreatedBy (string), CreatedDate (DateTime), UpdatedBy (string), UpdatedDate (DateTime)
                // Use nullable DateTime for flexibility
                var entityBuilder = modelBuilder.Entity(entityType.ClrType);

                if (entityType.FindProperty("CreatedBy") == null)
                {
                    entityBuilder.Property<string>("CreatedBy");
                }

                if (entityType.FindProperty("CreatedDate") == null)
                {
                    entityBuilder.Property<DateTime?>("CreatedDate");
                }

                if (entityType.FindProperty("UpdatedBy") == null)
                {
                    entityBuilder.Property<string>("UpdatedBy");
                }

                if (entityType.FindProperty("UpdatedDate") == null)
                {
                    entityBuilder.Property<DateTime?>("UpdatedDate");
                }
            }
        }
    }
}
