﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShareResource.Database;

#nullable disable

namespace ShareResource.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ShareResource.Models.Entities.Permission", b =>
                {
                    b.Property<string>("PermissionId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionId = "Read",
                            PermissionName = "Read"
                        },
                        new
                        {
                            PermissionId = "Write",
                            PermissionName = "Write"
                        });
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleDescription")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = "Admin",
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = "Guest",
                            RoleName = "Guest"
                        });
                });

            modelBuilder.Entity("ShareResource.Models.Entities.RolePermission", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PermissionId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions");

                    b.HasData(
                        new
                        {
                            RoleId = "Admin",
                            PermissionId = "Read"
                        },
                        new
                        {
                            RoleId = "Admin",
                            PermissionId = "Write"
                        },
                        new
                        {
                            RoleId = "Guest",
                            PermissionId = "Read"
                        });
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Token", b =>
                {
                    b.Property<int>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsRevoked")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("RefreshToken")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("TokenId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("UserPassword")
                        .HasColumnType("longtext");

                    b.Property<string>("UserPhone")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.Property<string>("UserRoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = "b42b1327-3b2e-4584-b99b-ad7662ac0fe2",
                            UserEmail = "Admin@gmail.com",
                            UserName = "Lapphan",
                            UserPassword = "AQAAAAIAAYagAAAAENOYkz9xEvjuv+6BDnPBcJxnW1Wi1hZyrJ+O8zdXXqpY0h1iAn3Ur+omOlIaNwL0Rw==",
                            UserPhone = "123456789",
                            UserRoleId = "Admin"
                        });
                });

            modelBuilder.Entity("ShareResource.Models.Entities.RolePermission", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShareResource.Models.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Token", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.User", "User")
                        .WithOne("UserToken")
                        .HasForeignKey("ShareResource.Models.Entities.Token", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.User", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.Role", "UserRole")
                        .WithMany("RoleUsers")
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("RoleUsers");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.User", b =>
                {
                    b.Navigation("UserToken");
                });
#pragma warning restore 612, 618
        }
    }
}
