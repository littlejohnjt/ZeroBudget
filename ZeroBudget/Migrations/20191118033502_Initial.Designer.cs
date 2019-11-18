﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZeroBudget.Data;

namespace ZeroBudget.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191118033502_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.ActualItem", b =>
                {
                    b.Property<int>("ActualItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("Money");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("int");

                    b.Property<int>("BudgetPeriodId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("ActualItemId");

                    b.HasIndex("BudgetCategoryId");

                    b.HasIndex("BudgetPeriodId");

                    b.ToTable("ActualItems","ZeroBudget");
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetCategory", b =>
                {
                    b.Property<int>("BudgetCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsTaxDeductible")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int?>("ParentBudgetCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("BudgetCategoryId");

                    b.ToTable("BudgetCategories","ZeroBudget");

                    b.HasData(
                        new
                        {
                            BudgetCategoryId = 1,
                            IsTaxDeductible = false,
                            Name = "Salary"
                        },
                        new
                        {
                            BudgetCategoryId = 2,
                            IsTaxDeductible = false,
                            Name = "Utilities"
                        },
                        new
                        {
                            BudgetCategoryId = 3,
                            IsTaxDeductible = false,
                            Name = "Savings"
                        },
                        new
                        {
                            BudgetCategoryId = 4,
                            IsTaxDeductible = false,
                            Name = "Housing"
                        },
                        new
                        {
                            BudgetCategoryId = 5,
                            IsTaxDeductible = false,
                            Name = "Transportation"
                        },
                        new
                        {
                            BudgetCategoryId = 6,
                            IsTaxDeductible = false,
                            Name = "Uncategorized"
                        });
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetItem", b =>
                {
                    b.Property<int>("BudgetItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("Money");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("int");

                    b.Property<int>("BudgetPeriodId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FrequencyQuantity")
                        .HasColumnType("int");

                    b.Property<int?>("FrequencyTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsReoccurring")
                        .HasColumnType("bit");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("BudgetItemId");

                    b.HasIndex("BudgetCategoryId");

                    b.HasIndex("BudgetPeriodId");

                    b.HasIndex("FrequencyTypeId");

                    b.ToTable("BudgetItems","ZeroBudget");
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetPeriod", b =>
                {
                    b.Property<int>("BudgetPeriodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetPeriodTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("BudgetPeriodId");

                    b.HasIndex("BudgetPeriodTypeId");

                    b.ToTable("BudgetPeriods","ZeroBudget");
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetPeriodType", b =>
                {
                    b.Property<int>("BudgetPeriodTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("BudgetPeriodTypeId");

                    b.ToTable("BudgetPeriodTypes","ZeroBudget");

                    b.HasData(
                        new
                        {
                            BudgetPeriodTypeId = 1,
                            Description = "Budget period occurs weekly",
                            Name = "Weekly"
                        },
                        new
                        {
                            BudgetPeriodTypeId = 2,
                            Description = "Budget period occurs twice a month, every other week ",
                            Name = "Bi-Weekly"
                        },
                        new
                        {
                            BudgetPeriodTypeId = 3,
                            Description = "Budget period occurs monthly",
                            Name = "Monthly"
                        },
                        new
                        {
                            BudgetPeriodTypeId = 4,
                            Description = "Budget period occurs twice a month, typically the beginning and middle of the month.",
                            Name = "Semi-Monthly"
                        });
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.FrequencyType", b =>
                {
                    b.Property<int>("FrequencyTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("FrequencyTypeId");

                    b.ToTable("FrequencyTypes","ZeroBudget");

                    b.HasData(
                        new
                        {
                            FrequencyTypeId = 1,
                            Description = "Occurs the same day each 'n' month(s)",
                            Name = "Monthly"
                        },
                        new
                        {
                            FrequencyTypeId = 2,
                            Description = "Occurrs the same day each 'n' year(s)",
                            Name = "Annually"
                        },
                        new
                        {
                            FrequencyTypeId = 3,
                            Description = "Occurs the same day each 'n' week(s)",
                            Name = "Weekly"
                        },
                        new
                        {
                            FrequencyTypeId = 4,
                            Description = "Occurs the same day each 'n' day(s)",
                            Name = "Daily"
                        },
                        new
                        {
                            FrequencyTypeId = 5,
                            Description = "Occurs every day monday through Friday",
                            Name = "Monday - Friday"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.ActualItem", b =>
                {
                    b.HasOne("ZeroBudget.Data.EntityClasses.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZeroBudget.Data.EntityClasses.BudgetPeriod", "BudgetPeriod")
                        .WithMany()
                        .HasForeignKey("BudgetPeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetItem", b =>
                {
                    b.HasOne("ZeroBudget.Data.EntityClasses.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZeroBudget.Data.EntityClasses.BudgetPeriod", "BudgetPeriod")
                        .WithMany()
                        .HasForeignKey("BudgetPeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZeroBudget.Data.EntityClasses.FrequencyType", "FrequencyType")
                        .WithMany()
                        .HasForeignKey("FrequencyTypeId");
                });

            modelBuilder.Entity("ZeroBudget.Data.EntityClasses.BudgetPeriod", b =>
                {
                    b.HasOne("ZeroBudget.Data.EntityClasses.BudgetPeriodType", "BudgetPeriodType")
                        .WithMany()
                        .HasForeignKey("BudgetPeriodTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}