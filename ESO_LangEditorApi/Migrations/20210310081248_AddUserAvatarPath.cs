﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESO_LangEditor.API.Migrations
{
    public partial class AddUserAvatarPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAvatarPath",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("08d42f95-66ff-4bcb-adc7-b395f436086c"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(3755), new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(3742) });

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("ba652528-900f-437b-8832-eb6d387ad010"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(3789), new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(3787) });

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("d05c0e82-025e-4e31-97ae-b8858ab0a784"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(3025), new DateTime(2021, 3, 10, 16, 12, 48, 123, DateTimeKind.Local).AddTicks(2381) });

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("5a1a90b8-183f-4d78-a76b-a00ed7889b84"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 122, DateTimeKind.Local).AddTicks(9948));

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("9a04bae0-b14a-4015-9b51-f0f04137000a"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 122, DateTimeKind.Local).AddTicks(9180));

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("c9b3970d-e7d5-4290-939a-563466f13203"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 122, DateTimeKind.Local).AddTicks(9983));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("0762d175-5a6f-43b4-9dea-b9c28d5d4b0e"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2164));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("3d284691-b117-44ea-bdfa-52e19b7e8612"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2068));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("4ca5183a-f0dd-4f6f-b664-58a647df535c"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2127));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("6cfaec5e-be72-4537-890b-696e5cd33b09"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2147));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("7639d924-b155-4f58-b45e-ff87bf0dba9b"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(1197));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("7b0a678d-04d1-4442-86ab-e7c3ffb207e0"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2109));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("cd017b93-0d0d-4c35-8bfc-9e82665ec817"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2182));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("d7480da5-602f-4b5e-8a03-5a95fbd34c1d"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2215));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("de09f1b2-7a2c-4bbc-8f16-b5fde4317d84"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 10, 16, 12, 48, 121, DateTimeKind.Local).AddTicks(2199));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvatarPath",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("08d42f95-66ff-4bcb-adc7-b395f436086c"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(4791), new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(4779) });

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("ba652528-900f-437b-8832-eb6d387ad010"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(4826), new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(4824) });

            migrationBuilder.UpdateData(
                table: "LangtextArchive",
                keyColumn: "Id",
                keyValue: new Guid("d05c0e82-025e-4e31-97ae-b8858ab0a784"),
                columns: new[] { "ArchiveTimestamp", "ReviewTimestamp" },
                values: new object[] { new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(4055), new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(3305) });

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("5a1a90b8-183f-4d78-a76b-a00ed7889b84"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(612));

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("9a04bae0-b14a-4015-9b51-f0f04137000a"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 890, DateTimeKind.Local).AddTicks(9848));

            migrationBuilder.UpdateData(
                table: "LangtextReview",
                keyColumn: "Id",
                keyValue: new Guid("c9b3970d-e7d5-4290-939a-563466f13203"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 891, DateTimeKind.Local).AddTicks(646));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("0762d175-5a6f-43b4-9dea-b9c28d5d4b0e"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3363));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("3d284691-b117-44ea-bdfa-52e19b7e8612"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3266));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("4ca5183a-f0dd-4f6f-b664-58a647df535c"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3324));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("6cfaec5e-be72-4537-890b-696e5cd33b09"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3343));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("7639d924-b155-4f58-b45e-ff87bf0dba9b"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(2397));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("7b0a678d-04d1-4442-86ab-e7c3ffb207e0"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3304));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("cd017b93-0d0d-4c35-8bfc-9e82665ec817"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3380));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("d7480da5-602f-4b5e-8a03-5a95fbd34c1d"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3413));

            migrationBuilder.UpdateData(
                table: "Langtexts",
                keyColumn: "Id",
                keyValue: new Guid("de09f1b2-7a2c-4bbc-8f16-b5fde4317d84"),
                column: "ReviewTimestamp",
                value: new DateTime(2021, 3, 4, 0, 0, 12, 889, DateTimeKind.Local).AddTicks(3397));
        }
    }
}