using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using KidsPrize.Models;

namespace KidsPrize.Migrations
{
    [DbContext(typeof(KidsPrizeDbContext))]
    partial class KidsPrizeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901");

            modelBuilder.Entity("KidsPrize.Models.Child", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Gender")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<int>("TotalScore");

                    b.Property<Guid>("Uid");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Child");
                });

            modelBuilder.Entity("KidsPrize.Models.Day", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChildId")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Tasks");

                    b.HasKey("Id");

                    b.HasIndex("ChildId");

                    b.ToTable("Day");
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DayId");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.ToTable("Score");
                });

            modelBuilder.Entity("KidsPrize.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("FamilyName")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("GivenName")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<Guid>("Uid");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("KidsPrize.Models.Child", b =>
                {
                    b.HasOne("KidsPrize.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("KidsPrize.Models.Day", b =>
                {
                    b.HasOne("KidsPrize.Models.Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KidsPrize.Models.Score", b =>
                {
                    b.HasOne("KidsPrize.Models.Day")
                        .WithMany()
                        .HasForeignKey("DayId");
                });
        }
    }
}
