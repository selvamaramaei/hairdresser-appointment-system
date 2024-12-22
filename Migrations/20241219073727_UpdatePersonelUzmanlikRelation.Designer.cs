﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebProje.Context;

#nullable disable

namespace WebProje.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241219073727_UpdatePersonelUzmanlikRelation")]
    partial class UpdatePersonelUzmanlikRelation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PersonelPersonelUzmanlik", b =>
                {
                    b.Property<int>("PersonellerId")
                        .HasColumnType("int");

                    b.Property<int>("UzmanliklarId")
                        .HasColumnType("int");

                    b.HasKey("PersonellerId", "UzmanliklarId");

                    b.HasIndex("UzmanliklarId");

                    b.ToTable("PersonelUzmanliklar", (string)null);
                });

            modelBuilder.Entity("WebProje.Models.Islem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<TimeSpan>("Sure")
                        .HasColumnType("time");

                    b.Property<decimal>("Ucret")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int?>("UzmanlikId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UzmanlikId");

                    b.ToTable("Islemler");
                });

            modelBuilder.Entity("WebProje.Models.Mesai", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("BaslangicZamani")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("BitisZamani")
                        .HasColumnType("time");

                    b.Property<int?>("PersonelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PersonelId");

                    b.ToTable("Mesailer");
                });

            modelBuilder.Entity("WebProje.Models.MesaiGunu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Gun")
                        .HasColumnType("int");

                    b.Property<int>("MesaiId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MesaiId");

                    b.ToTable("MesaiGunleri");
                });

            modelBuilder.Entity("WebProje.Models.Personel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Cinsiyet")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Personeller");
                });

            modelBuilder.Entity("WebProje.Models.PersonelUzmanlik", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("UzmanlikAdi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Uzmanliklar");
                });

            modelBuilder.Entity("WebProje.Models.Randevu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IslemId")
                        .HasColumnType("int");

                    b.Property<bool>("OnayliMi")
                        .HasColumnType("bit");

                    b.Property<int>("PersonelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RandevuTarihi")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IslemId");

                    b.HasIndex("PersonelId");

                    b.HasIndex("UserId");

                    b.ToTable("Randevular");
                });

            modelBuilder.Entity("WebProje.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sifre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PersonelPersonelUzmanlik", b =>
                {
                    b.HasOne("WebProje.Models.Personel", null)
                        .WithMany()
                        .HasForeignKey("PersonellerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebProje.Models.PersonelUzmanlik", null)
                        .WithMany()
                        .HasForeignKey("UzmanliklarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebProje.Models.Islem", b =>
                {
                    b.HasOne("WebProje.Models.PersonelUzmanlik", "Uzmanlik")
                        .WithMany("Islemler")
                        .HasForeignKey("UzmanlikId");

                    b.Navigation("Uzmanlik");
                });

            modelBuilder.Entity("WebProje.Models.Mesai", b =>
                {
                    b.HasOne("WebProje.Models.Personel", "Personel")
                        .WithMany("Mesailer")
                        .HasForeignKey("PersonelId");

                    b.Navigation("Personel");
                });

            modelBuilder.Entity("WebProje.Models.MesaiGunu", b =>
                {
                    b.HasOne("WebProje.Models.Mesai", "Mesai")
                        .WithMany("CalistigiGunler")
                        .HasForeignKey("MesaiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mesai");
                });

            modelBuilder.Entity("WebProje.Models.Randevu", b =>
                {
                    b.HasOne("WebProje.Models.Islem", "Islem")
                        .WithMany()
                        .HasForeignKey("IslemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebProje.Models.Personel", "Personel")
                        .WithMany()
                        .HasForeignKey("PersonelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebProje.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Islem");

                    b.Navigation("Personel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebProje.Models.Mesai", b =>
                {
                    b.Navigation("CalistigiGunler");
                });

            modelBuilder.Entity("WebProje.Models.Personel", b =>
                {
                    b.Navigation("Mesailer");
                });

            modelBuilder.Entity("WebProje.Models.PersonelUzmanlik", b =>
                {
                    b.Navigation("Islemler");
                });
#pragma warning restore 612, 618
        }
    }
}
