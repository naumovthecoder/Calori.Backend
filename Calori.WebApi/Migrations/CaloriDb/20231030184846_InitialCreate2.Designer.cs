﻿// // <auto-generated />
// using System;
// using Calori.WebApi;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure;
// using Microsoft.EntityFrameworkCore.Migrations;
// using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//
// namespace Calori.WebApi.Migrations.CaloriDb
// {
//     [DbContext(typeof(CaloriDbContext))]
//     [Migration("20231030184846_InitialCreate2")]
//     partial class InitialCreate2
//     {
//         protected override void BuildTargetModel(ModelBuilder modelBuilder)
//         {
// #pragma warning disable 612, 618
//             modelBuilder
//                 .HasAnnotation("ProductVersion", "5.0.17");
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.ApplicationAllergy", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int>("Allergy")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("ApplicationId")
//                         .HasColumnType("INTEGER");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("ApplicationId");
//
//                     b.ToTable("ApplicationAllergy");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.ApplicationBodyParameters", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("INTEGER");
//
//                     b.Property<decimal?>("BMI")
//                         .HasColumnType("TEXT");
//
//                     b.Property<decimal?>("BMR")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("MaxWeight")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("MinWeight")
//                         .HasColumnType("INTEGER");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("Id")
//                         .IsUnique();
//
//                     b.ToTable("ApplicationBodyParameters");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.CaloriApplication", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("ActivityLevelId")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("Age")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<string>("AnotherAllergy")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("ApplicationBodyParametersId")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<DateTime?>("CreatedAt")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("DailyCalories")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<string>("Email")
//                         .HasMaxLength(256)
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("GenderId")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("Goal")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("Height")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("PersonalSlimmingPlanId")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("Ration")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<DateTime?>("UpdatedAt")
//                         .HasColumnType("TEXT");
//
//                     b.Property<string>("UserId")
//                         .HasColumnType("TEXT");
//
//                     b.Property<decimal?>("Weight")
//                         .HasColumnType("TEXT");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("ApplicationBodyParametersId")
//                         .IsUnique();
//
//                     b.HasIndex("Id")
//                         .IsUnique();
//
//                     b.HasIndex("PersonalSlimmingPlanId");
//
//                     b.ToTable("CaloriApplications");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.CaloriAccount.CaloriSlimmingPlan", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("Calories")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<decimal?>("Carbohydrates")
//                         .HasColumnType("TEXT");
//
//                     b.Property<decimal?>("Fats")
//                         .HasColumnType("TEXT");
//
//                     b.Property<decimal?>("Protein")
//                         .HasColumnType("TEXT");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("Id")
//                         .IsUnique();
//
//                     b.ToTable("CaloriSlimmingPlan");
//
//                     b.HasData(
//                         new
//                         {
//                             Id = 1,
//                             Calories = 1250
//                         },
//                         new
//                         {
//                             Id = 2,
//                             Calories = 1500
//                         },
//                         new
//                         {
//                             Id = 3,
//                             Calories = 1750
//                         },
//                         new
//                         {
//                             Id = 4,
//                             Calories = 2000
//                         },
//                         new
//                         {
//                             Id = 5,
//                             Calories = 2250
//                         },
//                         new
//                         {
//                             Id = 6,
//                             Calories = 2500
//                         });
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.CaloriAccount.PersonalSlimmingPlan", b =>
//                 {
//                     b.Property<int>("Id")
//                         .ValueGeneratedOnAdd()
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("BurnedThisWeek")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("CaloriSlimmingPlanId")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("CaloricNeeds")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<DateTime?>("CreatedAt")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("CurrentWeekDeficit")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<decimal?>("CurrentWeight")
//                         .HasColumnType("TEXT");
//
//                     b.Property<DateTime?>("FinishDate")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("Goal")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<DateTime?>("StartDate")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("TotalBurned")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<DateTime?>("UpdatedAt")
//                         .HasColumnType("TEXT");
//
//                     b.Property<int?>("WeekNumber")
//                         .HasColumnType("INTEGER");
//
//                     b.Property<int?>("WeeksToTarget")
//                         .HasColumnType("INTEGER");
//
//                     b.HasKey("Id");
//
//                     b.HasIndex("CaloriSlimmingPlanId")
//                         .IsUnique();
//
//                     b.HasIndex("Id")
//                         .IsUnique();
//
//                     b.ToTable("PersonalSlimmingPlan");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.ApplicationAllergy", b =>
//                 {
//                     b.HasOne("Calori.Domain.Models.ApplicationModels.CaloriApplication", null)
//                         .WithMany("ApplicationAllergies")
//                         .HasForeignKey("ApplicationId");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.CaloriApplication", b =>
//                 {
//                     b.HasOne("Calori.Domain.Models.ApplicationModels.ApplicationBodyParameters", "ApplicationBodyParameters")
//                         .WithOne()
//                         .HasForeignKey("Calori.Domain.Models.ApplicationModels.CaloriApplication", "ApplicationBodyParametersId");
//
//                     b.HasOne("Calori.Domain.Models.CaloriAccount.PersonalSlimmingPlan", "PersonalSlimmingPlan")
//                         .WithMany()
//                         .HasForeignKey("PersonalSlimmingPlanId");
//
//                     b.Navigation("ApplicationBodyParameters");
//
//                     b.Navigation("PersonalSlimmingPlan");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.CaloriAccount.PersonalSlimmingPlan", b =>
//                 {
//                     b.HasOne("Calori.Domain.Models.CaloriAccount.CaloriSlimmingPlan", "CaloriSlimmingPlan")
//                         .WithOne()
//                         .HasForeignKey("Calori.Domain.Models.CaloriAccount.PersonalSlimmingPlan", "CaloriSlimmingPlanId");
//
//                     b.Navigation("CaloriSlimmingPlan");
//                 });
//
//             modelBuilder.Entity("Calori.Domain.Models.ApplicationModels.CaloriApplication", b =>
//                 {
//                     b.Navigation("ApplicationAllergies");
//                 });
// #pragma warning restore 612, 618
//         }
//     }
// }
