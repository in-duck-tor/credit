﻿// <auto-generated />
using System;
using System.Collections.Generic;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    [DbContext(typeof(LoanDbContext))]
    [Migration("20240312194005_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("credit")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("loan_personal_code_seq");

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.LoanBilling", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.ComplexProperty<Dictionary<string, object>>("LoanBody", "InDuckTor.Credit.Domain.Billing.LoanBilling.LoanBody#BillingItem", b1 =>
                        {
                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("LoanDebt", "InDuckTor.Credit.Domain.Billing.LoanBilling.LoanDebt#BillingItem", b1 =>
                        {
                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Penalty", "InDuckTor.Credit.Domain.Billing.LoanBilling.Penalty#BillingItem", b1 =>
                        {
                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.HasKey("Id");

                    b.ToTable("LoanBilling", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.BillingPayoff", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("PaymentDistributionId")
                        .HasColumnType("bigint");

                    b.Property<long>("PeriodBillingId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PaymentDistributionId");

                    b.HasIndex("PeriodBillingId");

                    b.ToTable("BillingPayoff", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.Payment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("PaymentAmount")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Payment", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.PaymentDistribution", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    b.Property<bool>("IsDistributed")
                        .HasColumnType("boolean");

                    b.Property<decimal?>("Penalty")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Payment", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Period.PeriodBilling", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("IsDebt")
                        .HasColumnType("boolean");

                    b.Property<long>("LoanBillingId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("OneTimePayment")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("PeriodEndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PeriodStartDate")
                        .HasColumnType("timestamp with time zone");

                    b.ComplexProperty<Dictionary<string, object>>("BillingItems", "InDuckTor.Credit.Domain.Billing.Period.PeriodBilling.BillingItems#BillingItems", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("ChargingForServices")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("Interest")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("LoanBodyPayoff")
                                .HasColumnType("numeric");
                        });

                    b.HasKey("Id");

                    b.HasIndex("LoanBillingId");

                    b.ToTable("PeriodBilling", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Financing.Application.LoanApplication", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ApplicationState")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("BorrowedAmount")
                        .HasColumnType("numeric");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<long>("LoanProgramId")
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("LoanTerm")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("LoanProgramId");

                    b.ToTable("LoanApplication", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Financing.Program.LoanProgram", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("numeric");

                    b.Property<int>("PaymentScheduleType")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("LoanProgram", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.LoanManagement.Loan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("ApprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("BorrowedAmount")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("BorrowingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("numeric");

                    b.Property<int>("PaymentScheduleType")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentType")
                        .HasColumnType("integer");

                    b.Property<int>("PlannedPaymentsNumber")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Loan", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.LoanBilling", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.LoanManagement.Loan", "Loan")
                        .WithOne("LoanBilling")
                        .HasForeignKey("InDuckTor.Credit.Domain.Billing.LoanBilling", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("InDuckTor.Credit.Domain.Billing.PeriodAccruals", "PeriodAccruals", b1 =>
                        {
                            b1.Property<long>("LoanBillingId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("ChargingForServices")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("InterestAccrual")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("LoanBodyPayoff")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("OneTimePayment")
                                .HasColumnType("numeric");

                            b1.Property<DateTime>("PeriodEndDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<DateTime>("PeriodStartDate")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("LoanBillingId");

                            b1.ToTable("LoanBilling", "credit");

                            b1.WithOwner()
                                .HasForeignKey("LoanBillingId");
                        });

                    b.Navigation("Loan");

                    b.Navigation("PeriodAccruals");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.BillingPayoff", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.Billing.Payment.PaymentDistribution", null)
                        .WithMany("BillingsPayoffs")
                        .HasForeignKey("PaymentDistributionId");

                    b.HasOne("InDuckTor.Credit.Domain.Billing.Period.PeriodBilling", "PeriodBilling")
                        .WithMany()
                        .HasForeignKey("PeriodBillingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("InDuckTor.Credit.Domain.Billing.Period.BillingItems", "BillingItems", b1 =>
                        {
                            b1.Property<long>("BillingPayoffId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("ChargingForServices")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("Interest")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("LoanBodyPayoff")
                                .HasColumnType("numeric");

                            b1.HasKey("BillingPayoffId");

                            b1.ToTable("BillingPayoff", "credit");

                            b1.WithOwner()
                                .HasForeignKey("BillingPayoffId");
                        });

                    b.Navigation("BillingItems")
                        .IsRequired();

                    b.Navigation("PeriodBilling");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.PaymentDistribution", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.Billing.Payment.Payment", null)
                        .WithOne("PaymentDistribution")
                        .HasForeignKey("InDuckTor.Credit.Domain.Billing.Payment.PaymentDistribution", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Period.PeriodBilling", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.Billing.LoanBilling", "LoanBilling")
                        .WithMany("PeriodsBillings")
                        .HasForeignKey("LoanBillingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("InDuckTor.Credit.Domain.Billing.Period.BillingItems", "RemainingPayoff", b1 =>
                        {
                            b1.Property<long>("PeriodBillingId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("ChargingForServices")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("Interest")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("LoanBodyPayoff")
                                .HasColumnType("numeric");

                            b1.HasKey("PeriodBillingId");

                            b1.ToTable("PeriodBilling", "credit");

                            b1.WithOwner()
                                .HasForeignKey("PeriodBillingId");
                        });

                    b.Navigation("LoanBilling");

                    b.Navigation("RemainingPayoff");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Financing.Application.LoanApplication", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.Financing.Program.LoanProgram", "LoanProgram")
                        .WithMany()
                        .HasForeignKey("LoanProgramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoanProgram");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.LoanBilling", b =>
                {
                    b.Navigation("PeriodsBillings");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.Payment", b =>
                {
                    b.Navigation("PaymentDistribution")
                        .IsRequired();
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.PaymentDistribution", b =>
                {
                    b.Navigation("BillingsPayoffs");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.LoanManagement.Loan", b =>
                {
                    b.Navigation("LoanBilling")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
