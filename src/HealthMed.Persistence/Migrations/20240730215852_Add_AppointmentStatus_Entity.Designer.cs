﻿// <auto-generated />
using System;
using HealthMed.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HealthMed.Persistence.Migrations
{
    [DbContext(typeof(EFContext))]
    [Migration("20240730215852_Add_AppointmentStatus_Entity")]
    partial class Add_AppointmentStatus_Entity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HealthMed.Domain.Entities.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdAppointment");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CanceledAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasBeenNotified")
                        .HasColumnType("bit");

                    b.Property<byte>("IdAppointmentStatus")
                        .HasColumnType("tinyint");

                    b.Property<int>("IdDoctor")
                        .HasColumnType("int");

                    b.Property<int?>("IdPatient")
                        .IsConcurrencyToken()
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("IdAppointmentStatus");

                    b.HasIndex("IdDoctor");

                    b.HasIndex("IdPatient");

                    b.ToTable("appointments", (string)null);
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.AppointmentStatus", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint")
                        .HasColumnName("IdAppointmentStatus");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("appointmentstatus", (string)null);

                    b.HasData(
                        new
                        {
                            Id = (byte)1,
                            Name = "Disponível"
                        },
                        new
                        {
                            Id = (byte)2,
                            Name = "Ocupado"
                        },
                        new
                        {
                            Id = (byte)3,
                            Name = "Cancelado"
                        });
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.Role", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint")
                        .HasColumnName("IdRole");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = (byte)1,
                            Name = "Médico"
                        },
                        new
                        {
                            Id = (byte)2,
                            Name = "Paciente"
                        });
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdSchedule");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdDoctor")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("IdDoctor");

                    b.ToTable("schedules", (string)null);
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdUser");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("CRM")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte>("IdRole")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(180)
                        .HasColumnType("nvarchar(180)");

                    b.Property<string>("_passwordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PasswordHash");

                    b.HasKey("Id");

                    b.HasIndex("IdRole");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 10000,
                            CPF = "16058582008",
                            CRM = "1605858-BR",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)1,
                            Name = "Dr. Doctor 1",
                            _passwordHash = "MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1"
                        },
                        new
                        {
                            Id = 10001,
                            CPF = "80084320044",
                            CRM = "8008432-BR",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)1,
                            Name = "Dr. Doctor 2",
                            _passwordHash = "MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1"
                        },
                        new
                        {
                            Id = 10002,
                            CPF = "04953908015",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)2,
                            Name = "Ailton",
                            _passwordHash = "LFhLAgFT8oinF3iXkk63ccZhEllpvGtr/OHG28On+hqniGeX+AIYe8UhNnqztEIm"
                        },
                        new
                        {
                            Id = 10003,
                            CPF = "30050470086",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)2,
                            Name = "Bruno",
                            _passwordHash = "yobUq3aH9/R2x//xYdfaxqX2+FVBBLKzLipbFZILjsTo2sJ9cU/f2F4q6vvwIRzs"
                        },
                        new
                        {
                            Id = 10004,
                            CPF = "80738939080",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)2,
                            Name = "Cecília",
                            _passwordHash = "LSHTSlFvEBDMS0tjoK2po682H7rLfgL2sXssgm/djzWWouzW4lIydGie7PbmX/1P"
                        },
                        new
                        {
                            Id = 10005,
                            CPF = "53900451060",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)2,
                            Name = "Cesar",
                            _passwordHash = "q1EyG7yB1S6Cwm7DGuDo3P8ZraEvVHTdBbKHZ1QW3TMG5JWVCnb3EO3UslYiiGeL"
                        },
                        new
                        {
                            Id = 10006,
                            CPF = "41739149033",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IdRole = (byte)2,
                            Name = "Paulo",
                            _passwordHash = "XAro1VAlABuvkw5sxcSPEUdCeuTZRcM+9qLOumd79674Ro2V0bvvnlgb3zIkA7Yt"
                        });
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.Appointment", b =>
                {
                    b.HasOne("HealthMed.Domain.Entities.AppointmentStatus", null)
                        .WithMany()
                        .HasForeignKey("IdAppointmentStatus")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("HealthMed.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("IdDoctor")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("HealthMed.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("IdPatient")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.Schedule", b =>
                {
                    b.HasOne("HealthMed.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("IdDoctor")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("HealthMed.Domain.Entities.User", b =>
                {
                    b.HasOne("HealthMed.Domain.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("IdRole")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("HealthMed.Domain.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)")
                                .HasColumnName("Email");

                            b1.HasKey("UserId");

                            b1.ToTable("users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");

                            b1.HasData(
                                new
                                {
                                    UserId = 10000,
                                    Value = "dr.doctor1@healthmed.app"
                                },
                                new
                                {
                                    UserId = 10001,
                                    Value = "dr.doctor2@healthmed.app"
                                },
                                new
                                {
                                    UserId = 10002,
                                    Value = "ailton@techchallenge.app"
                                },
                                new
                                {
                                    UserId = 10003,
                                    Value = "bruno@techchallenge.app"
                                },
                                new
                                {
                                    UserId = 10004,
                                    Value = "cecilia@techchallenge.app"
                                },
                                new
                                {
                                    UserId = 10005,
                                    Value = "cesar@techchallenge.app"
                                },
                                new
                                {
                                    UserId = 10006,
                                    Value = "paulo@techchallenge.app"
                                });
                        });

                    b.Navigation("Email");
                });
#pragma warning restore 612, 618
        }
    }
}
