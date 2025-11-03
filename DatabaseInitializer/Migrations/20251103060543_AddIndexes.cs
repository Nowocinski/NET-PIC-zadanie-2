using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseInitializer.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_CompanyId",
                table: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Person_BirthDate",
                table: "Person",
                column: "BirthDate");

            migrationBuilder.CreateIndex(
                name: "IX_Person_FirstName_LastName",
                table: "Person",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_Gender",
                table: "Person",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyId_ContractType",
                table: "Employee",
                columns: new[] { "CompanyId", "ContractType" });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ContractType",
                table: "Employee",
                column: "ContractType");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Salary",
                table: "Employee",
                column: "Salary");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Person_BirthDate",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_FirstName_LastName",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_Gender",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Employee_CompanyId_ContractType",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ContractType",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Salary",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Company_Name",
                table: "Company");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyId",
                table: "Employee",
                column: "CompanyId");
        }
    }
}
