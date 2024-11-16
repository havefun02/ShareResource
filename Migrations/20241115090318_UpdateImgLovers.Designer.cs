﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShareResource.Database;

#nullable disable

namespace ShareResource.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241115090318_UpdateImgLovers")]
    partial class UpdateImgLovers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ShareResource.Models.Entities.Img", b =>
                {
                    b.Property<string>("ImgId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<long>("FileSize")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("ImgId");

                    b.HasIndex("UserId");

                    b.ToTable("Imgs");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.ImgLovers", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ImgId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "ImgId");

                    b.HasIndex("ImgId");

                    b.ToTable("ImgLovers");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.ImgTags", b =>
                {
                    b.Property<string>("TagId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ImgId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("TagId", "ImgId");

                    b.HasIndex("ImgId");

                    b.ToTable("ImgTags");
                });

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

                    b.HasIndex("RoleName")
                        .IsUnique();

                    b.ToTable("Roles");
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
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Tag", b =>
                {
                    b.Property<string>("TagId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
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

                    b.Property<byte[]>("UserIcon")
                        .HasColumnType("longblob");

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
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Img", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.User", "User")
                        .WithMany("UserImgs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.ImgLovers", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.Img", "Img")
                        .WithMany("ImgLovers")
                        .HasForeignKey("ImgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShareResource.Models.Entities.User", "User")
                        .WithMany("ImgLovers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Img");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.ImgTags", b =>
                {
                    b.HasOne("ShareResource.Models.Entities.Img", "Imgs")
                        .WithMany("ImgTags")
                        .HasForeignKey("ImgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShareResource.Models.Entities.Tag", "Tags")
                        .WithMany("ImgTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Imgs");

                    b.Navigation("Tags");
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
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.Img", b =>
                {
                    b.Navigation("ImgLovers");

                    b.Navigation("ImgTags");
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

            modelBuilder.Entity("ShareResource.Models.Entities.Tag", b =>
                {
                    b.Navigation("ImgTags");
                });

            modelBuilder.Entity("ShareResource.Models.Entities.User", b =>
                {
                    b.Navigation("ImgLovers");

                    b.Navigation("UserImgs");

                    b.Navigation("UserToken");
                });
#pragma warning restore 612, 618
        }
    }
}
