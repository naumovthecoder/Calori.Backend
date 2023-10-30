// using System;
// using Microsoft.EntityFrameworkCore.Migrations;
//
// namespace Calori.WebApi.Migrations.CaloriDb
// {
//     public partial class InitialCreate : Migration
//     {
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.CreateTable(
//                 name: "ApplicationBodyParameters",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "INTEGER", nullable: false)
//                         .Annotation("Sqlite:Autoincrement", true),
//                     MinWeight = table.Column<int>(type: "INTEGER", nullable: true),
//                     MaxWeight = table.Column<int>(type: "INTEGER", nullable: true),
//                     BMI = table.Column<decimal>(type: "TEXT", nullable: true),
//                     BMR = table.Column<decimal>(type: "TEXT", nullable: true)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_ApplicationBodyParameters", x => x.Id);
//                 });
//
//             migrationBuilder.CreateTable(
//                 name: "CaloriSlimmingPlan",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "INTEGER", nullable: false)
//                         .Annotation("Sqlite:Autoincrement", true),
//                     Calories = table.Column<int>(type: "INTEGER", nullable: true),
//                     Protein = table.Column<decimal>(type: "TEXT", nullable: true),
//                     Fats = table.Column<decimal>(type: "TEXT", nullable: true),
//                     Carbohydrates = table.Column<decimal>(type: "TEXT", nullable: true)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_CaloriSlimmingPlan", x => x.Id);
//                 });
//
//             migrationBuilder.CreateTable(
//                 name: "PersonalSlimmingPlan",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "INTEGER", nullable: false)
//                         .Annotation("Sqlite:Autoincrement", true),
//                     StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
//                     FinishDate = table.Column<DateTime>(type: "TEXT", nullable: true),
//                     WeekNumber = table.Column<int>(type: "INTEGER", nullable: true),
//                     CurrentWeight = table.Column<decimal>(type: "TEXT", nullable: true),
//                     CaloricNeeds = table.Column<int>(type: "INTEGER", nullable: true),
//                     Goal = table.Column<int>(type: "INTEGER", nullable: true),
//                     CaloriSlimmingPlanId = table.Column<int>(type: "INTEGER", nullable: true),
//                     CurrentWeekDeficit = table.Column<int>(type: "INTEGER", nullable: true),
//                     BurnedThisWeek = table.Column<int>(type: "INTEGER", nullable: true),
//                     TotalBurned = table.Column<int>(type: "INTEGER", nullable: true),
//                     WeeksToTarget = table.Column<int>(type: "INTEGER", nullable: true),
//                     CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
//                     UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_PersonalSlimmingPlan", x => x.Id);
//                     table.ForeignKey(
//                         name: "FK_PersonalSlimmingPlan_CaloriSlimmingPlan_CaloriSlimmingPlanId",
//                         column: x => x.CaloriSlimmingPlanId,
//                         principalTable: "CaloriSlimmingPlan",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Restrict);
//                 });
//
//             migrationBuilder.CreateTable(
//                 name: "CaloriApplications",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "INTEGER", nullable: false)
//                         .Annotation("Sqlite:Autoincrement", true),
//                     GenderId = table.Column<int>(type: "INTEGER", nullable: true),
//                     Weight = table.Column<decimal>(type: "TEXT", nullable: true),
//                     Height = table.Column<int>(type: "INTEGER", nullable: true),
//                     Goal = table.Column<int>(type: "INTEGER", nullable: true),
//                     Age = table.Column<int>(type: "INTEGER", nullable: true),
//                     Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
//                     ApplicationBodyParametersId = table.Column<int>(type: "INTEGER", nullable: true),
//                     ActivityLevelId = table.Column<int>(type: "INTEGER", nullable: true),
//                     CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
//                     AnotherAllergy = table.Column<string>(type: "TEXT", nullable: true),
//                     DailyCalories = table.Column<int>(type: "INTEGER", nullable: true),
//                     Ration = table.Column<int>(type: "INTEGER", nullable: true),
//                     PersonalSlimmingPlanId = table.Column<int>(type: "INTEGER", nullable: true),
//                     UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
//                     UserId = table.Column<string>(type: "TEXT", nullable: true)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_CaloriApplications", x => x.Id);
//                     table.ForeignKey(
//                         name: "FK_CaloriApplications_ApplicationBodyParameters_ApplicationBodyParametersId",
//                         column: x => x.ApplicationBodyParametersId,
//                         principalTable: "ApplicationBodyParameters",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Restrict);
//                     table.ForeignKey(
//                         name: "FK_CaloriApplications_PersonalSlimmingPlan_PersonalSlimmingPlanId",
//                         column: x => x.PersonalSlimmingPlanId,
//                         principalTable: "PersonalSlimmingPlan",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Restrict);
//                 });
//
//             migrationBuilder.CreateTable(
//                 name: "ApplicationAllergy",
//                 columns: table => new
//                 {
//                     Id = table.Column<int>(type: "INTEGER", nullable: false)
//                         .Annotation("Sqlite:Autoincrement", true),
//                     ApplicationId = table.Column<int>(type: "INTEGER", nullable: true),
//                     Allergy = table.Column<int>(type: "INTEGER", nullable: false)
//                 },
//                 constraints: table =>
//                 {
//                     table.PrimaryKey("PK_ApplicationAllergy", x => x.Id);
//                     table.ForeignKey(
//                         name: "FK_ApplicationAllergy_CaloriApplications_ApplicationId",
//                         column: x => x.ApplicationId,
//                         principalTable: "CaloriApplications",
//                         principalColumn: "Id",
//                         onDelete: ReferentialAction.Restrict);
//                 });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 1, 1250, null, null, null });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 2, 1500, null, null, null });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 3, 1750, null, null, null });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 4, 2000, null, null, null });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 5, 2250, null, null, null });
//
//             migrationBuilder.InsertData(
//                 table: "CaloriSlimmingPlan",
//                 columns: new[] { "Id", "Calories", "Carbohydrates", "Fats", "Protein" },
//                 values: new object[] { 6, 2500, null, null, null });
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_ApplicationAllergy_ApplicationId",
//                 table: "ApplicationAllergy",
//                 column: "ApplicationId");
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_ApplicationBodyParameters_Id",
//                 table: "ApplicationBodyParameters",
//                 column: "Id",
//                 unique: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_CaloriApplications_ApplicationBodyParametersId",
//                 table: "CaloriApplications",
//                 column: "ApplicationBodyParametersId",
//                 unique: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_CaloriApplications_Id",
//                 table: "CaloriApplications",
//                 column: "Id",
//                 unique: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_CaloriApplications_PersonalSlimmingPlanId",
//                 table: "CaloriApplications",
//                 column: "PersonalSlimmingPlanId");
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_CaloriSlimmingPlan_Id",
//                 table: "CaloriSlimmingPlan",
//                 column: "Id",
//                 unique: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_PersonalSlimmingPlan_CaloriSlimmingPlanId",
//                 table: "PersonalSlimmingPlan",
//                 column: "CaloriSlimmingPlanId",
//                 unique: true);
//
//             migrationBuilder.CreateIndex(
//                 name: "IX_PersonalSlimmingPlan_Id",
//                 table: "PersonalSlimmingPlan",
//                 column: "Id",
//                 unique: true);
//         }
//
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropTable(
//                 name: "ApplicationAllergy");
//
//             migrationBuilder.DropTable(
//                 name: "CaloriApplications");
//
//             migrationBuilder.DropTable(
//                 name: "ApplicationBodyParameters");
//
//             migrationBuilder.DropTable(
//                 name: "PersonalSlimmingPlan");
//
//             migrationBuilder.DropTable(
//                 name: "CaloriSlimmingPlan");
//         }
//     }
// }
