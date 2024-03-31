﻿// <auto-generated />
using System;
using System.Collections.Generic;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InDuckTor.Credit.Infrastructure.Migrations
{
    [DbContext(typeof(LoanDbContext))]
    partial class LoanDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("credit")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("loan_personal_code_seq");

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.BillingPayoff", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("PaymentId")
                        .HasColumnType("bigint");

                    b.Property<long>("PeriodBillingId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PaymentId");

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

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDistributed")
                        .HasColumnType("boolean");

                    b.Property<long>("LoanId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("PaymentAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("Penalty")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("LoanId");

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

                    b.Property<long>("LoanId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("OneTimePayment")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("PeriodEndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PeriodStartDate")
                        .HasColumnType("timestamp with time zone");

                    b.ComplexProperty<Dictionary<string, object>>("ExpenseItems", "InDuckTor.Credit.Domain.Billing.Period.PeriodBilling.ExpenseItems#ExpenseItems", b1 =>
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

                    b.HasIndex("LoanId");

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

                    b.Property<string>("ClientAccountNumber")
                        .IsRequired()
                        .HasColumnType("text");

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

                    b.Property<TimeSpan?>("PeriodInterval")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.ToTable("LoanProgram", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.LoanManagement.CreditScore.CreditScore", b =>
                {
                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<long>("Score")
                        .HasColumnType("bigint");

                    b.HasKey("ClientId");

                    b.ToTable("CreditScore", "credit");
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

                    b.Property<string>("ClientAccountNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("numeric");

                    b.Property<string>("LoanAccountNumber")
                        .HasColumnType("text");

                    b.Property<int>("PaymentScheduleType")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentType")
                        .HasColumnType("integer");

                    b.Property<TimeSpan?>("PeriodInterval")
                        .HasColumnType("interval");

                    b.Property<int>("PlannedPaymentsNumber")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.ComplexProperty<Dictionary<string, object>>("BodyAfterPayoffs", "InDuckTor.Credit.Domain.LoanManagement.Loan.BodyAfterPayoffs#ExpenseItem", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("CurrentBody", "InDuckTor.Credit.Domain.LoanManagement.Loan.CurrentBody#ExpenseItem", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Debt", "InDuckTor.Credit.Domain.LoanManagement.Loan.Debt#ExpenseItem", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Penalty", "InDuckTor.Credit.Domain.LoanManagement.Loan.Penalty#ExpenseItem", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");
                        });

                    b.HasKey("Id");

                    b.ToTable("Loan", "credit");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.BillingPayoff", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.Billing.Payment.Payment", null)
                        .WithMany("BillingsPayoffs")
                        .HasForeignKey("PaymentId");

                    b.HasOne("InDuckTor.Credit.Domain.Billing.Period.PeriodBilling", "PeriodBilling")
                        .WithMany()
                        .HasForeignKey("PeriodBillingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("InDuckTor.Credit.Domain.Expenses.ExpenseItems", "ExpenseItems", b1 =>
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

                    b.Navigation("ExpenseItems")
                        .IsRequired();

                    b.Navigation("PeriodBilling");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.Payment", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.LoanManagement.Loan", null)
                        .WithMany()
                        .HasForeignKey("LoanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Period.PeriodBilling", b =>
                {
                    b.HasOne("InDuckTor.Credit.Domain.LoanManagement.Loan", "Loan")
                        .WithMany("PeriodsBillings")
                        .HasForeignKey("LoanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("InDuckTor.Credit.Domain.Expenses.ExpenseItems", "RemainingPayoff", b1 =>
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

                    b.Navigation("Loan");

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

            modelBuilder.Entity("InDuckTor.Credit.Domain.LoanManagement.Loan", b =>
                {
                    b.OwnsOne("InDuckTor.Credit.Domain.LoanManagement.PeriodAccruals", "PeriodAccruals", b1 =>
                        {
                            b1.Property<long>("LoanId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("ChargingForServices")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("InterestAccrual")
                                .HasColumnType("numeric");

                            b1.Property<decimal>("LoanBodyPayoff")
                                .HasColumnType("numeric");

                            b1.Property<DateTime>("PeriodEndDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<DateTime>("PeriodStartDate")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("LoanId");

                            b1.ToTable("Loan", "credit");

                            b1.WithOwner()
                                .HasForeignKey("LoanId");
                        });

                    b.Navigation("PeriodAccruals");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.Billing.Payment.Payment", b =>
                {
                    b.Navigation("BillingsPayoffs");
                });

            modelBuilder.Entity("InDuckTor.Credit.Domain.LoanManagement.Loan", b =>
                {
                    b.Navigation("PeriodsBillings");
                });
#pragma warning restore 612, 618
        }
    }
}
