﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyCandidate.DataAccess;

#nullable disable

namespace MyCandidate.DataAccess.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("MyCandidate.Common.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModificationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<int>("LocationId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateOnVacancy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CandidateId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModificationDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("SelectionStatusId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VacancyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("SelectionStatusId");

                    b.HasIndex("VacancyId");

                    b.ToTable("CandidateOnVacancies");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CandidateId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("CandidateResources");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateSkill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CandidateId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeniorityId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SkillId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("SeniorityId");

                    b.HasIndex("SkillId");

                    b.ToTable("CandidateSkills");
                });

            modelBuilder.Entity("MyCandidate.Common.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CountryId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("MyCandidate.Common.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CandidateOnVacancyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModificationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CandidateOnVacancyId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MyCandidate.Common.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("MyCandidate.Common.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("MyCandidate.Common.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<int>("CityId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("MyCandidate.Common.Office", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CompanyId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LocationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("LocationId");

                    b.ToTable("Offices");
                });

            modelBuilder.Entity("MyCandidate.Common.ResourceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ResourceTypes");
                });

            modelBuilder.Entity("MyCandidate.Common.SelectionStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SelectionStatuses");
                });

            modelBuilder.Entity("MyCandidate.Common.Seniority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Seniorities");
                });

            modelBuilder.Entity("MyCandidate.Common.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<int>("SkillCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SkillCategoryId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("MyCandidate.Common.SkillCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SkillCategories");
                });

            modelBuilder.Entity("MyCandidate.Common.Vacancy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModificationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<int>("OfficeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VacancyStatusId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.HasIndex("VacancyStatusId");

                    b.ToTable("Vacancies");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancyResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VacancyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ResourceTypeId");

                    b.HasIndex("VacancyId");

                    b.ToTable("VacancyResources");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancySkill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeniorityId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SkillId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VacancyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SeniorityId");

                    b.HasIndex("SkillId");

                    b.HasIndex("VacancyId");

                    b.ToTable("VacancySkills");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancyStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VacancyStatuses");
                });

            modelBuilder.Entity("MyCandidate.Common.Candidate", b =>
                {
                    b.HasOne("MyCandidate.Common.Location", "Location")
                        .WithMany("Candidates")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateOnVacancy", b =>
                {
                    b.HasOne("MyCandidate.Common.Candidate", "Candidate")
                        .WithMany("CandidateOnVacancies")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.SelectionStatus", "SelectionStatus")
                        .WithMany()
                        .HasForeignKey("SelectionStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Vacancy", "Vacancy")
                        .WithMany("CandidateOnVacancies")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("SelectionStatus");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateResource", b =>
                {
                    b.HasOne("MyCandidate.Common.Candidate", "Candidate")
                        .WithMany("CandidateResources")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.ResourceType", "ResourceType")
                        .WithMany("CandidateResources")
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("ResourceType");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateSkill", b =>
                {
                    b.HasOne("MyCandidate.Common.Candidate", "Candidate")
                        .WithMany("CandidateSkills")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Seniority", "Seniority")
                        .WithMany()
                        .HasForeignKey("SeniorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("Seniority");

                    b.Navigation("Skill");
                });

            modelBuilder.Entity("MyCandidate.Common.City", b =>
                {
                    b.HasOne("MyCandidate.Common.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("MyCandidate.Common.Comment", b =>
                {
                    b.HasOne("MyCandidate.Common.CandidateOnVacancy", "CandidateOnVacancy")
                        .WithMany("Comments")
                        .HasForeignKey("CandidateOnVacancyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CandidateOnVacancy");
                });

            modelBuilder.Entity("MyCandidate.Common.Location", b =>
                {
                    b.HasOne("MyCandidate.Common.City", "City")
                        .WithMany("Locations")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("MyCandidate.Common.Office", b =>
                {
                    b.HasOne("MyCandidate.Common.Company", "Company")
                        .WithMany("Officies")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Location", "Location")
                        .WithMany("Officies")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("MyCandidate.Common.Skill", b =>
                {
                    b.HasOne("MyCandidate.Common.SkillCategory", "SkillCategory")
                        .WithMany("Skills")
                        .HasForeignKey("SkillCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SkillCategory");
                });

            modelBuilder.Entity("MyCandidate.Common.Vacancy", b =>
                {
                    b.HasOne("MyCandidate.Common.Office", "Office")
                        .WithMany("Vacancies")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.VacancyStatus", "VacancyStatus")
                        .WithMany("Vacancies")
                        .HasForeignKey("VacancyStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");

                    b.Navigation("VacancyStatus");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancyResource", b =>
                {
                    b.HasOne("MyCandidate.Common.ResourceType", "ResourceType")
                        .WithMany("VacancyResources")
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Vacancy", "Vacancy")
                        .WithMany("VacancyResources")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceType");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancySkill", b =>
                {
                    b.HasOne("MyCandidate.Common.Seniority", "Seniority")
                        .WithMany()
                        .HasForeignKey("SeniorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Skill", "Skill")
                        .WithMany("VacancySkills")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyCandidate.Common.Vacancy", "Vacancy")
                        .WithMany("VacancySkills")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Seniority");

                    b.Navigation("Skill");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("MyCandidate.Common.Candidate", b =>
                {
                    b.Navigation("CandidateOnVacancies");

                    b.Navigation("CandidateResources");

                    b.Navigation("CandidateSkills");
                });

            modelBuilder.Entity("MyCandidate.Common.CandidateOnVacancy", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("MyCandidate.Common.City", b =>
                {
                    b.Navigation("Locations");
                });

            modelBuilder.Entity("MyCandidate.Common.Company", b =>
                {
                    b.Navigation("Officies");
                });

            modelBuilder.Entity("MyCandidate.Common.Country", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("MyCandidate.Common.Location", b =>
                {
                    b.Navigation("Candidates");

                    b.Navigation("Officies");
                });

            modelBuilder.Entity("MyCandidate.Common.Office", b =>
                {
                    b.Navigation("Vacancies");
                });

            modelBuilder.Entity("MyCandidate.Common.ResourceType", b =>
                {
                    b.Navigation("CandidateResources");

                    b.Navigation("VacancyResources");
                });

            modelBuilder.Entity("MyCandidate.Common.Skill", b =>
                {
                    b.Navigation("VacancySkills");
                });

            modelBuilder.Entity("MyCandidate.Common.SkillCategory", b =>
                {
                    b.Navigation("Skills");
                });

            modelBuilder.Entity("MyCandidate.Common.Vacancy", b =>
                {
                    b.Navigation("CandidateOnVacancies");

                    b.Navigation("VacancyResources");

                    b.Navigation("VacancySkills");
                });

            modelBuilder.Entity("MyCandidate.Common.VacancyStatus", b =>
                {
                    b.Navigation("Vacancies");
                });
#pragma warning restore 612, 618
        }
    }
}
