using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZeroBudget.Data.EntityClasses;

namespace ZeroBudget.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<BudgetCategory> budgetCategories { get; set; }

        public DbSet<FrequencyType> frequencyTypes { get; set; }

        public DbSet<BudgetPeriod> budgetPeriods { get; set; }

        public DbSet<BudgetItem> budgetItems { get; set; }

        public DbSet<ActualItem> actualItems { get; set; }

        public DbSet<BudgetPeriodType> budgetPeriodTypes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //
            // Budget Categories
            //
            builder.Entity<BudgetCategory>().HasKey(bc => bc.BudgetCategoryId);
            builder.Entity<BudgetCategory>().ToTable("BudgetCategories", "ZeroBudget");
            builder.Entity<BudgetCategory>().Property(bc => bc.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<BudgetCategory>().Property(bc => bc.UserId)
                .HasMaxLength(450);
            builder.Entity<BudgetCategory>().Property(bc => bc.IsTaxDeductible)
                .IsRequired();
            builder.Entity<BudgetCategory>().HasOne(bc => bc.ParentBudgetCategory)
                .WithMany()
                .HasForeignKey(bc => bc.ParentBudgetCategoryId);

            // Some starting categories
            builder.Entity<BudgetCategory>().HasData(
                new BudgetCategory { BudgetCategoryId = 1, Name = "Salary" },
                new BudgetCategory { BudgetCategoryId = 2, Name = "Utilities" },
                new BudgetCategory { BudgetCategoryId = 3, Name = "Savings" },
                new BudgetCategory { BudgetCategoryId = 4, Name = "Housing" },
                new BudgetCategory { BudgetCategoryId = 5, Name = "Transportation" },
                new BudgetCategory { BudgetCategoryId = 6, Name = "Uncategorized" });

            //
            // Frequency Types
            //
            builder.Entity<FrequencyType>().HasKey(ft => ft.FrequencyTypeId);
            builder.Entity<FrequencyType>().ToTable("FrequencyTypes", "ZeroBudget");
            builder.Entity<FrequencyType>().Property(ft => ft.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<FrequencyType>().Property(ft => ft.Description)
                .HasMaxLength(200);

            // The frequency types
            builder.Entity<FrequencyType>().HasData(
                new FrequencyType { FrequencyTypeId = 1, Name = "Monthly", Description = "Occurs the same day each 'n' month(s)" },
                new FrequencyType { FrequencyTypeId = 2, Name = "Annually", Description = "Occurrs the same day each 'n' year(s)"},
                new FrequencyType { FrequencyTypeId = 3, Name = "Weekly", Description = "Occurs the same day each 'n' week(s)"},
                new FrequencyType { FrequencyTypeId = 4, Name = "Daily", Description = "Occurs the same day each 'n' day(s)"},
                new FrequencyType { FrequencyTypeId = 5, Name = "Monday - Friday", Description = "Occurs every day monday through Friday"}
                );

            //
            // Budget Period Types
            //
            builder.Entity<BudgetPeriodType>().HasKey(bpt => bpt.BudgetPeriodTypeId);
            builder.Entity<BudgetPeriodType>().ToTable("BudgetPeriodTypes", "ZeroBudget");
            builder.Entity<BudgetPeriodType>().Property(bpt => bpt.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<BudgetPeriodType>().Property(bpt => bpt.Description)
                .HasMaxLength(200);

            // Budget period types
            builder.Entity<BudgetPeriodType>().HasData(
                new BudgetPeriodType { BudgetPeriodTypeId = 1, Name = "Weekly", Description = "Budget period occurs weekly" },
                new BudgetPeriodType { BudgetPeriodTypeId = 2, Name = "Bi-Weekly", Description = "Budget period occurs twice a month, every other week " },
                new BudgetPeriodType { BudgetPeriodTypeId = 3, Name = "Monthly", Description = "Budget period occurs monthly" },
                new BudgetPeriodType { BudgetPeriodTypeId = 4, Name = "Semi-Monthly", Description = "Budget period occurs twice a month, typically the beginning and middle of the month." }
                );

            //
            // Budget Periods
            //
            builder.Entity<BudgetPeriod>().HasKey(bp => bp.BudgetPeriodId);
            builder.Entity<BudgetPeriod>().ToTable("BudgetPeriods", "ZeroBudget");
            builder.Entity<BudgetPeriod>().Property(bp => bp.UserId)
                .HasMaxLength(450);
            builder.Entity<BudgetPeriod>().Property(bp => bp.StartDate)
                .IsRequired();
            builder.Entity<BudgetPeriod>().HasOne(bp => bp.BudgetPeriodType)
                .WithMany()
                .HasForeignKey(bp => bp.BudgetPeriodTypeId);

            // 
            // Budget Items
            //
            builder.Entity<BudgetItem>().HasKey(bi => bi.BudgetItemId);
            builder.Entity<BudgetItem>().ToTable("BudgetItems", "ZeroBudget");
            builder.Entity<BudgetItem>().Property(bi => bi.UserId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Entity<BudgetItem>().Property(bi => bi.Date).IsRequired();
            builder.Entity<BudgetItem>().Property(bi => bi.Amount)
                .IsRequired()
                .HasColumnType("Money");
            builder.Entity<BudgetItem>().Property(bi => bi.TransactionType)
                .IsRequired();
            builder.Entity<BudgetItem>().Property(bi => bi.IsReoccurring)
                .IsRequired();
            builder.Entity<BudgetItem>().Property(bi => bi.BudgetCategoryId)
                .IsRequired();
            builder.Entity<BudgetItem>().HasOne(bi => bi.BudgetCategory)
                .WithMany()
                .HasForeignKey(bi => bi.BudgetCategoryId);
            builder.Entity<BudgetItem>().Property(bi => bi.BudgetPeriodId)
                .IsRequired();
            builder.Entity<BudgetItem>().HasOne(bi => bi.BudgetPeriod)
                .WithMany()
                .HasForeignKey(bi => bi.BudgetPeriodId);
            builder.Entity<BudgetItem>().HasOne(bi => bi.FrequencyType)
                .WithMany()
                .HasForeignKey(bi => bi.FrequencyTypeId);
            builder.Entity<BudgetItem>().Property(bi => bi.TransactionType)
                .IsRequired();

            //
            // Actual Items
            //
            builder.Entity<ActualItem>().HasKey(ai => ai.ActualItemId);
            builder.Entity<ActualItem>().ToTable("ActualItems", "ZeroBudget");
            builder.Entity<ActualItem>().Property(ai => ai.UserId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Entity<ActualItem>().Property(ai => ai.BudgetCategoryId)
                .IsRequired();
            builder.Entity<ActualItem>().HasOne(ai => ai.BudgetCategory)
                .WithMany()
                .HasForeignKey(ai => ai.BudgetCategoryId);
            builder.Entity<ActualItem>().Property(ai => ai.BudgetPeriodId)
                .IsRequired();
            builder.Entity<ActualItem>().HasOne(ai => ai.BudgetPeriod)
                .WithMany()
                .HasForeignKey(ai => ai.BudgetPeriodId);
            builder.Entity<ActualItem>().Property(ai => ai.Date)
                .IsRequired();
            builder.Entity<ActualItem>().Property(ai => ai.Amount)
                .IsRequired()
                .HasColumnType("Money");
            builder.Entity<ActualItem>().Property(ai => ai.TransactionType)
                .IsRequired();
        }
    }
}
