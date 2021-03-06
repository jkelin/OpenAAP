﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using OpenAAP.Context;
using System;

namespace OpenAAP.Migrations
{
    [DbContext(typeof(OpenAAPContext))]
    [Migration("20180616090150_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("OpenAAP.Context.Identity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasMaxLength(255);

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasMaxLength(64);

                    b.Property<string>("UserName")
                        .HasColumnName("user_name")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("identity");
                });

            modelBuilder.Entity("OpenAAP.Context.PasswordAuthentication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DisabledAt")
                        .HasColumnName("disabled_at");

                    b.Property<byte[]>("EncodedStoredPassword")
                        .IsRequired()
                        .HasColumnName("encoded_stored_password");

                    b.Property<Guid>("IdentityId")
                        .HasColumnName("identity_id");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId")
                        .HasName("ix_password_authentication_identity_id");

                    b.ToTable("password_authentication");
                });

            modelBuilder.Entity("OpenAAP.Context.PasswordAuthentication", b =>
                {
                    b.HasOne("OpenAAP.Context.Identity", "Identity")
                        .WithMany("PasswordAuthentications")
                        .HasForeignKey("IdentityId")
                        .HasConstraintName("fk_password_authentication_identity_identity_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
