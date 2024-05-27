﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvenienceMVC.Migrations
{
    /// <inheritdoc />
    public partial class _20240524001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shiire_saki_master",
                columns: table => new
                {
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_saki_kaisya = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    shiire_saki_busho = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    yubin_bango = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    todoufuken = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shikuchoson = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    banchi = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    tatemonomei = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shiire_saki_master", x => x.shiire_saki_code);
                });

            migrationBuilder.CreateTable(
                name: "shohin_master",
                columns: table => new
                {
                    shohin_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shohin_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    shohi_zairitu = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    shohi_zairitu_gaishoku = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shohin_master", x => x.shohin_code);
                });

            migrationBuilder.CreateTable(
                name: "chumon_jisseki",
                columns: table => new
                {
                    chumon_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    chumon_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chumon_jisseki", x => new { x.chumon_code, x.shiire_saki_code });
                    table.ForeignKey(
                        name: "FK_chumon_jisseki_shiire_saki_master_shiire_saki_code",
                        column: x => x.shiire_saki_code,
                        principalTable: "shiire_saki_master",
                        principalColumn: "shiire_saki_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shiire_master",
                columns: table => new
                {
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_prd_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shohin_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_prd_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    shiire_pcs_unit = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    shiire_unit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_tanka = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shiire_master", x => new { x.shiire_saki_code, x.shiire_prd_code, x.shohin_code });
                    table.ForeignKey(
                        name: "FK_shiire_master_shiire_saki_master_shiire_saki_code",
                        column: x => x.shiire_saki_code,
                        principalTable: "shiire_saki_master",
                        principalColumn: "shiire_saki_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shiire_master_shohin_master_shohin_code",
                        column: x => x.shohin_code,
                        principalTable: "shohin_master",
                        principalColumn: "shohin_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chumon_jisseki_meisai",
                columns: table => new
                {
                    chumon_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_prd_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shohin_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    chumon_su = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    chumon_zan = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chumon_jisseki_meisai", x => new { x.chumon_code, x.shiire_saki_code, x.shiire_prd_code, x.shohin_code });
                    table.ForeignKey(
                        name: "FK_chumon_jisseki_meisai_chumon_jisseki_chumon_code_shiire_sak~",
                        columns: x => new { x.chumon_code, x.shiire_saki_code },
                        principalTable: "chumon_jisseki",
                        principalColumns: new[] { "chumon_code", "shiire_saki_code" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chumon_jisseki_meisai_shiire_master_shiire_saki_code_shiire~",
                        columns: x => new { x.shiire_saki_code, x.shiire_prd_code, x.shohin_code },
                        principalTable: "shiire_master",
                        principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "soko_zaiko",
                columns: table => new
                {
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_prd_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shohin_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    soko_zaiko_case_su = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    soko_zaiko_su = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    last_shiire_date = table.Column<DateOnly>(type: "date", nullable: false),
                    last_delivery_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soko_zaiko", x => new { x.shiire_saki_code, x.shiire_prd_code, x.shohin_code });
                    table.ForeignKey(
                        name: "FK_soko_zaiko_shiire_master_shiire_saki_code_shiire_prd_code_s~",
                        columns: x => new { x.shiire_saki_code, x.shiire_prd_code, x.shohin_code },
                        principalTable: "shiire_master",
                        principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shiire_jisseki",
                columns: table => new
                {
                    chumon_code = table.Column<string>(type: "character varying(20)", nullable: false),
                    shiire_date = table.Column<DateOnly>(type: "date", nullable: false),
                    seq_by_shiiredate = table.Column<long>(type: "bigint", nullable: false),
                    shiire_saki_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_prd_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    shiire_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    shohin_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nonyu_su = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shiire_jisseki", x => new { x.chumon_code, x.shiire_date, x.seq_by_shiiredate, x.shiire_saki_code, x.shiire_prd_code });
                    table.ForeignKey(
                        name: "FK_shiire_jisseki_chumon_jisseki_meisai_chumon_code_shiire_sak~",
                        columns: x => new { x.chumon_code, x.shiire_saki_code, x.shiire_prd_code, x.shohin_code },
                        principalTable: "chumon_jisseki_meisai",
                        principalColumns: new[] { "chumon_code", "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chumon_jisseki_shiire_saki_code",
                table: "chumon_jisseki",
                column: "shiire_saki_code");

            migrationBuilder.CreateIndex(
                name: "IX_chumon_jisseki_meisai_shiire_saki_code_shiire_prd_code_shoh~",
                table: "chumon_jisseki_meisai",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" });

            migrationBuilder.CreateIndex(
                name: "IX_shiire_jisseki_chumon_code_shiire_saki_code_shiire_prd_code~",
                table: "shiire_jisseki",
                columns: new[] { "chumon_code", "shiire_saki_code", "shiire_prd_code", "shohin_code" });

            migrationBuilder.CreateIndex(
                name: "IX_shiire_master_shohin_code",
                table: "shiire_master",
                column: "shohin_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shiire_jisseki");

            migrationBuilder.DropTable(
                name: "soko_zaiko");

            migrationBuilder.DropTable(
                name: "chumon_jisseki_meisai");

            migrationBuilder.DropTable(
                name: "chumon_jisseki");

            migrationBuilder.DropTable(
                name: "shiire_master");

            migrationBuilder.DropTable(
                name: "shiire_saki_master");

            migrationBuilder.DropTable(
                name: "shohin_master");
        }
    }
}
