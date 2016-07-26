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
                .HasAnnotation("Npgsql:PostgresExtension:.uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("KidsPrize.Models.Child", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Gender")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<int>("TotalScore");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Child");
                });

            modelBuilder.Entity("KidsPrize.Models.Day", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ChildId")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.HasAlternateKey("ChildId", "Date");

                    b.HasIndex("ChildId");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DayId")
                        .IsRequired();

                    b.Property<int>("Position");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasAlternateKey("DayId", "Task");

                    b.HasIndex("DayId");

                    b.ToTable("Score");
                });

            modelBuilder.Entity("KidsPrize.Models.Day", b =>
                {
                    b.HasOne("KidsPrize.Models.Child", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.HasOne("KidsPrize.Models.Day")
                        .WithMany("Scores")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
