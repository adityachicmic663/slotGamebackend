﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SlotGameBackend;

#nullable disable

namespace SlotGameBackend.Migrations
{
    [DbContext(typeof(slotDataContext))]
    [Migration("20240828113930_ThirdCreate")]
    partial class ThirdCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SlotGameBackend.Models.AdminSettings", b =>
                {
                    b.Property<Guid>("settingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("minimumBetLimit")
                        .HasColumnType("int");

                    b.HasKey("settingId");

                    b.ToTable("settings");
                });

            modelBuilder.Entity("SlotGameBackend.Models.GameSession", b =>
                {
                    b.Property<Guid>("sessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("clientSeed")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("isActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("lastActivityTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("serverSeed")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("sessionEndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("sessionStartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("userId")
                        .HasColumnType("char(36)");

                    b.HasKey("sessionId");

                    b.HasIndex("userId");

                    b.ToTable("gameSessions");
                });

            modelBuilder.Entity("SlotGameBackend.Models.PayLine", b =>
                {
                    b.Property<Guid>("payLineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("multiplier")
                        .HasColumnType("int");

                    b.HasKey("payLineId");

                    b.ToTable("payLines");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Spin", b =>
                {
                    b.Property<Guid>("spinResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GameSessionsessionId")
                        .HasColumnType("char(36)");

                    b.Property<int>("betAmount")
                        .HasColumnType("int");

                    b.Property<string>("reelsOutcome")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("serverSeed")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("sessionId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("spinTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("winAmount")
                        .HasColumnType("int");

                    b.HasKey("spinResultId");

                    b.HasIndex("GameSessionsessionId");

                    b.ToTable("spinResults");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Symbol", b =>
                {
                    b.Property<Guid>("symbolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("imagePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("symbolName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("symbolId");

                    b.ToTable("symbols");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Transaction", b =>
                {
                    b.Property<Guid>("transactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("UserModeluserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("adminResponse")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("amount")
                        .HasColumnType("int");

                    b.Property<DateTime>("requestedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("transactionStatus")
                        .HasColumnType("int");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.Property<Guid>("walletId")
                        .HasColumnType("char(36)");

                    b.HasKey("transactionId");

                    b.HasIndex("UserModeluserId");

                    b.HasIndex("walletId");

                    b.ToTable("transactions");
                });

            modelBuilder.Entity("SlotGameBackend.Models.UserModel", b =>
                {
                    b.Property<Guid>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("OtpTokenExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("hashPassword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("lastName")
                        .HasColumnType("longtext");

                    b.Property<string>("otpToken")
                        .HasColumnType("longtext");

                    b.Property<string>("profilePicturePath")
                        .HasColumnType("longtext");

                    b.Property<string>("role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("userId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Wallet", b =>
                {
                    b.Property<Guid>("walletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("balance")
                        .HasColumnType("int");

                    b.Property<Guid>("userId")
                        .HasColumnType("char(36)");

                    b.HasKey("walletId");

                    b.HasIndex("userId")
                        .IsUnique();

                    b.ToTable("wallets");
                });

            modelBuilder.Entity("SlotGameBackend.Models.payLinePositions", b =>
                {
                    b.Property<Guid>("positionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.Property<Guid>("payLineId")
                        .HasColumnType("char(36)");

                    b.HasKey("positionId");

                    b.HasIndex("payLineId");

                    b.ToTable("payLinesPositions");
                });

            modelBuilder.Entity("SlotGameBackend.Models.GameSession", b =>
                {
                    b.HasOne("SlotGameBackend.Models.UserModel", "user")
                        .WithMany("sessions")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Spin", b =>
                {
                    b.HasOne("SlotGameBackend.Models.GameSession", null)
                        .WithMany("spinResults")
                        .HasForeignKey("GameSessionsessionId");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Transaction", b =>
                {
                    b.HasOne("SlotGameBackend.Models.UserModel", null)
                        .WithMany("transactions")
                        .HasForeignKey("UserModeluserId");

                    b.HasOne("SlotGameBackend.Models.Wallet", "wallet")
                        .WithMany()
                        .HasForeignKey("walletId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("wallet");
                });

            modelBuilder.Entity("SlotGameBackend.Models.Wallet", b =>
                {
                    b.HasOne("SlotGameBackend.Models.UserModel", "user")
                        .WithOne("wallet")
                        .HasForeignKey("SlotGameBackend.Models.Wallet", "userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("SlotGameBackend.Models.payLinePositions", b =>
                {
                    b.HasOne("SlotGameBackend.Models.PayLine", "PayLine")
                        .WithMany("positions")
                        .HasForeignKey("payLineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PayLine");
                });

            modelBuilder.Entity("SlotGameBackend.Models.GameSession", b =>
                {
                    b.Navigation("spinResults");
                });

            modelBuilder.Entity("SlotGameBackend.Models.PayLine", b =>
                {
                    b.Navigation("positions");
                });

            modelBuilder.Entity("SlotGameBackend.Models.UserModel", b =>
                {
                    b.Navigation("sessions");

                    b.Navigation("transactions");

                    b.Navigation("wallet")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
