﻿// <auto-generated />
using KidsPrize.Repository.Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace KidsPrize.Repository.Npgsql.Migrations
{
    [DbContext(typeof(KidsPrizeContext))]
    [Migration("20180521131009_Refactor")]
    partial class Refactor
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("KidsPrize")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Child", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Gender")
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<int>("TotalScore");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Child");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Preference", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(250);

                    b.Property<int>("TimeZoneOffset");

                    b.HasKey("UserId");

                    b.ToTable("Preference");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Redeem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChildId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset>("Timestamp");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ChildId", "Timestamp");

                    b.ToTable("Redeem");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChildId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "Date", "Task");

                    b.ToTable("Score");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.SortableTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<int>("Order");

                    b.Property<int?>("TaskGroupId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("TaskGroupId");

                    b.ToTable("SortableTask");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.TaskGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChildId");

                    b.Property<DateTime>("EffectiveDate");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "EffectiveDate");

                    b.ToTable("TaskGroup");
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Redeem", b =>
                {
                    b.HasOne("KidsPrize.Repository.Npgsql.Entities.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.Score", b =>
                {
                    b.HasOne("KidsPrize.Repository.Npgsql.Entities.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.SortableTask", b =>
                {
                    b.HasOne("KidsPrize.Repository.Npgsql.Entities.TaskGroup")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Repository.Npgsql.Entities.TaskGroup", b =>
                {
                    b.HasOne("KidsPrize.Repository.Npgsql.Entities.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
