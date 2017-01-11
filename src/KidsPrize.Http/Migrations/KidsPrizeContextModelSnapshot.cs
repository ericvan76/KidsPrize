using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using KidsPrize;

namespace KidsPrize.Http.Migrations
{
    [DbContext(typeof(KidsPrizeContext))]
    partial class KidsPrizeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("KidsPrize")
                .HasAnnotation("Npgsql:PostgresExtension:public.uuid-ossp", "'uuid-ossp', 'public', ''")
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("KidsPrize.Models.Child", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Gender")
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<int>("TotalScore");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Child");
                });

            modelBuilder.Entity("KidsPrize.Models.Preference", b =>
                {
                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<int>("TimeZoneOffset");

                    b.HasKey("UserId");

                    b.ToTable("Preference");
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ChildId")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "Date", "Task");

                    b.HasIndex("ChildId");

                    b.ToTable("Score");
                });

            modelBuilder.Entity("KidsPrize.Models.SortableTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 50);

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

                    b.Property<Guid?>("ChildId")
                        .IsRequired();

                    b.Property<DateTime>("EffectiveDate");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "EffectiveDate");

                    b.HasIndex("ChildId");

                    b.ToTable("TaskGroup");
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
