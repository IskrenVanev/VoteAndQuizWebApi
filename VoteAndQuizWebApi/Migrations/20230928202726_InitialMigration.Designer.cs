﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VoteAndQuizWebApi.Data;

#nullable disable

namespace VoteAndQuizWebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230928202726_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Quiz", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CorrectOptionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("CreatorId")
                        .IsRequired()
                        .HasColumnType("varbinary(900)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("QuizEndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ShowQuiz")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("quizVotes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.QuizOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuizId")
                        .IsUnique();

                    b.ToTable("QuizOptions");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.User", b =>
                {
                    b.Property<byte[]>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varbinary(900)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AuthId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Loses")
                        .HasColumnType("int");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.UserQuizAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<string>("UserAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("varbinary(900)");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("UserQuizAnswers");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.UserVoteAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Option")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("UserId")
                        .HasColumnType("varbinary(900)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserVoteAnswers");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Vote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("CreatorId")
                        .IsRequired()
                        .HasColumnType("varbinary(900)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ShowVote")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("VoteEndDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("voteVotes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.VoteOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Option")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("VoteCount")
                        .HasColumnType("bigint");

                    b.Property<int?>("VoteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VoteId");

                    b.ToTable("VoteOptions");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Quiz", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.QuizOption", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.Quiz", "Quiz")
                        .WithOne("CorrectOption")
                        .HasForeignKey("VoteAndQuizWebApi.Models.QuizOption", "QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.UserQuizAnswer", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.Quiz", "Quiz")
                        .WithMany("Options")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VoteAndQuizWebApi.Models.User", null)
                        .WithMany("UserQuizAnswers")
                        .HasForeignKey("UserId");

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.UserVoteAnswer", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.User", null)
                        .WithMany("UserVoteAnswers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Vote", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.VoteOption", b =>
                {
                    b.HasOne("VoteAndQuizWebApi.Models.Vote", null)
                        .WithMany("Options")
                        .HasForeignKey("VoteId");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Quiz", b =>
                {
                    b.Navigation("CorrectOption")
                        .IsRequired();

                    b.Navigation("Options");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.User", b =>
                {
                    b.Navigation("UserQuizAnswers");

                    b.Navigation("UserVoteAnswers");
                });

            modelBuilder.Entity("VoteAndQuizWebApi.Models.Vote", b =>
                {
                    b.Navigation("Options");
                });
#pragma warning restore 612, 618
        }
    }
}