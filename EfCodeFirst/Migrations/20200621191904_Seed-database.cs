using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCodeFirst.Migrations
{
    public partial class Seeddatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Dose",
                table: "PrescriptionMedicament",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Doctor",
                columns: new[] { "IdDoctor", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "as@gmail.com", "Albert", "Sum" },
                    { 2, "ab@gmail.com", "Adam", "Brown" },
                    { 3, "as@gmail.com", "Catherine", "Smith" }
                });

            migrationBuilder.InsertData(
                table: "Medicament",
                columns: new[] { "IdMedicament", "Description", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "Short-acting benzodiazepine", "Xanax", "Pills" },
                    { 2, "Drug containing salts of amphetamine", "Adderall", "Pills" },
                    { 3, "Effective pain killer", "Vicodin", "Pills" }
                });

            migrationBuilder.InsertData(
                table: "Patient",
                columns: new[] { "IdPatient", "Birthdate", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 6, 21, 21, 19, 4, 282, DateTimeKind.Local).AddTicks(3855), "Martin", "Gooseman" },
                    { 2, new DateTime(2020, 6, 21, 21, 19, 4, 287, DateTimeKind.Local).AddTicks(6174), "Luke", "Krause" },
                    { 3, new DateTime(2020, 6, 21, 21, 19, 4, 287, DateTimeKind.Local).AddTicks(6227), "Kyron", "Harwood" }
                });

            migrationBuilder.InsertData(
                table: "Prescription",
                columns: new[] { "IdPrescription", "Date", "DueDate", "IdDoctor", "IdPatient" },
                values: new object[] { 1, new DateTime(2020, 6, 21, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(1175), new DateTime(2020, 6, 22, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(1698), 1, 1 });

            migrationBuilder.InsertData(
                table: "Prescription",
                columns: new[] { "IdPrescription", "Date", "DueDate", "IdDoctor", "IdPatient" },
                values: new object[] { 2, new DateTime(2020, 6, 21, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(2280), new DateTime(2020, 6, 22, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(2303), 2, 2 });

            migrationBuilder.InsertData(
                table: "Prescription",
                columns: new[] { "IdPrescription", "Date", "DueDate", "IdDoctor", "IdPatient" },
                values: new object[] { 3, new DateTime(2020, 6, 21, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(2312), new DateTime(2020, 6, 22, 21, 19, 4, 301, DateTimeKind.Local).AddTicks(2315), 3, 3 });

            migrationBuilder.InsertData(
                table: "PrescriptionMedicament",
                columns: new[] { "IdMedicament", "IdPrescription", "Details", "Dose" },
                values: new object[,]
                {
                    { 1, 1, "Lorem ipsum dolor", 15 },
                    { 3, 1, "Lorem ipsum dolor", 10 },
                    { 2, 2, "Lorem ipsum dolor", 8 },
                    { 3, 2, "Lorem ipsum dolor", 6 },
                    { 1, 3, "Lorem ipsum dolor", 3 },
                    { 2, 3, "Lorem ipsum dolor", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "PrescriptionMedicament",
                keyColumns: new[] { "IdMedicament", "IdPrescription" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "Medicament",
                keyColumn: "IdMedicament",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medicament",
                keyColumn: "IdMedicament",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medicament",
                keyColumn: "IdMedicament",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Prescription",
                keyColumn: "IdPrescription",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Prescription",
                keyColumn: "IdPrescription",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Prescription",
                keyColumn: "IdPrescription",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctor",
                keyColumn: "IdDoctor",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctor",
                keyColumn: "IdDoctor",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctor",
                keyColumn: "IdDoctor",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patient",
                keyColumn: "IdPatient",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patient",
                keyColumn: "IdPatient",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patient",
                keyColumn: "IdPatient",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "Dose",
                table: "PrescriptionMedicament",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
