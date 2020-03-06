using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KidsPrize.Repository.Npgsql.Migrations
{
    [DbContext(typeof(KidsPrizeContext))]
    [Migration("20170111015717_AddRedeem")]
    partial class AddRedeem
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("KidsPrize")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', 'public', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("KidsPrize.Models.Child", b =>
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

            modelBuilder.Entity("KidsPrize.Models.Preference", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(250);

                    b.Property<int>("TimeZoneOffset");

                    b.HasKey("UserId");

                    b.ToTable("Preference");
                });

            modelBuilder.Entity("KidsPrize.Models.Redeem", b =>
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

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
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

            modelBuilder.Entity("KidsPrize.Models.SortableTask", b =>
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

            modelBuilder.Entity("KidsPrize.Models.TaskGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChildId");

                    b.Property<DateTime>("EffectiveDate");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "EffectiveDate");

                    b.ToTable("TaskGroup");
                });

            modelBuilder.Entity("KidsPrize.Models.Redeem", b =>
                {
                    b.HasOne("KidsPrize.Models.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.HasOne("KidsPrize.Models.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Models.SortableTask", b =>
                {
                    b.HasOne("KidsPrize.Models.TaskGroup")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Models.TaskGroup", b =>
                {
                    b.HasOne("KidsPrize.Models.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
