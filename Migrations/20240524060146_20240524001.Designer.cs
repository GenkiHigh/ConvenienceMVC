﻿// <auto-generated />
using System;
using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ConvenienceMVC.Migrations
{
    [DbContext(typeof(ConvenienceMVCContext))]
    [Migration("20240524060146_20240524001")]
    partial class _20240524001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ChumonJisseki", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<DateOnly?>("ChumonDate")
                        .HasColumnType("date")
                        .HasColumnName("chumon_date");

                    b.HasKey("ChumonId", "ShiireSakiId");

                    b.HasIndex("ShiireSakiId");

                    b.ToTable("chumon_jisseki");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ChumonJissekiMeisai", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<decimal>("ChumonSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("chumon_su");

                    b.Property<decimal>("ChumonZan")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("chumon_zan");

                    b.HasKey("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.HasIndex("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("chumon_jisseki_meisai");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ShiireMaster", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<decimal>("ShiirePcsPerUnit")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shiire_pcs_unit");

                    b.Property<string>("ShiirePrdName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_prd_name");

                    b.Property<decimal>("ShiireTanka")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shiire_tanka");

                    b.Property<string>("ShiireUnit")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_unit");

                    b.HasKey("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.HasIndex("ShohinId");

                    b.ToTable("shiire_master");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ShiireSakiMaster", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("Banchi")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("banchi");

                    b.Property<string>("ShiireSakiBusho")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_saki_busho");

                    b.Property<string>("ShiireSakiKaisya")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("shiire_saki_kaisya");

                    b.Property<string>("Shikuchoson")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shikuchoson");

                    b.Property<string>("Tatemonomei")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("tatemonomei");

                    b.Property<string>("Todoufuken")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("todoufuken");

                    b.Property<string>("YubinBango")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("yubin_bango");

                    b.HasKey("ShiireSakiId");

                    b.ToTable("shiire_saki_master");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ShohinMaster", b =>
                {
                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<string>("ShohinName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("shohin_name");

                    b.Property<decimal>("ShohinZeiritu")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohi_zairitu");

                    b.Property<decimal>("ShohinZeirituGaishoku")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("shohi_zairitu_gaishoku");

                    b.HasKey("ShohinId");

                    b.ToTable("shohin_master");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Shiires.ShiireJisseki", b =>
                {
                    b.Property<string>("ChumonId")
                        .HasColumnType("character varying(20)")
                        .HasColumnName("chumon_code");

                    b.Property<DateOnly>("ShiireDate")
                        .HasColumnType("date")
                        .HasColumnName("shiire_date");

                    b.Property<long>("SeqByShiireDate")
                        .HasColumnType("bigint")
                        .HasColumnName("seq_by_shiiredate");

                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<decimal>("NonyuSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("nonyu_su");

                    b.Property<DateTime>("ShiireDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("shiire_datetime");

                    b.Property<string>("ShohinId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.HasKey("ChumonId", "ShiireDate", "SeqByShiireDate", "ShiireSakiId", "ShiirePrdId");

                    b.HasIndex("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("shiire_jisseki");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Shiires.SokoZaiko", b =>
                {
                    b.Property<string>("ShiireSakiId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_saki_code");

                    b.Property<string>("ShiirePrdId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shiire_prd_code");

                    b.Property<string>("ShohinId")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("shohin_code");

                    b.Property<DateOnly?>("LastDeliveryDate")
                        .HasColumnType("date")
                        .HasColumnName("last_delivery_date");

                    b.Property<DateOnly>("LastShiireDate")
                        .HasColumnType("date")
                        .HasColumnName("last_shiire_date");

                    b.Property<decimal>("SokoZaikoCaseSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("soko_zaiko_case_su");

                    b.Property<decimal>("SokoZaikoSu")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("soko_zaiko_su");

                    b.HasKey("ShiireSakiId", "ShiirePrdId", "ShohinId");

                    b.ToTable("soko_zaiko");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ChumonJisseki", b =>
                {
                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ShiireSakiMaster", "ShiireSakiMaster")
                        .WithMany()
                        .HasForeignKey("ShiireSakiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireSakiMaster");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ChumonJissekiMeisai", b =>
                {
                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ChumonJisseki", "ChumonJisseki")
                        .WithMany("ChumonJissekiMeisais")
                        .HasForeignKey("ChumonId", "ShiireSakiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ShiireMaster", "ShiireMaster")
                        .WithMany()
                        .HasForeignKey("ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChumonJisseki");

                    b.Navigation("ShiireMaster");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ShiireMaster", b =>
                {
                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ShiireSakiMaster", "ShiireSakiMaster")
                        .WithMany("ShiireMasters")
                        .HasForeignKey("ShiireSakiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ShohinMaster", "ShohinMaster")
                        .WithMany()
                        .HasForeignKey("ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireSakiMaster");

                    b.Navigation("ShohinMaster");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Shiires.ShiireJisseki", b =>
                {
                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ChumonJissekiMeisai", "ChumonJissekiMeisai")
                        .WithMany()
                        .HasForeignKey("ChumonId", "ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChumonJissekiMeisai");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Shiires.SokoZaiko", b =>
                {
                    b.HasOne("ConvenienceMVC.Models.Entities.Chumons.ShiireMaster", "ShiireMaster")
                        .WithMany()
                        .HasForeignKey("ShiireSakiId", "ShiirePrdId", "ShohinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiireMaster");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ChumonJisseki", b =>
                {
                    b.Navigation("ChumonJissekiMeisais");
                });

            modelBuilder.Entity("ConvenienceMVC.Models.Entities.Chumons.ShiireSakiMaster", b =>
                {
                    b.Navigation("ShiireMasters");
                });
#pragma warning restore 612, 618
        }
    }
}
