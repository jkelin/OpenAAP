﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using OpenAAP.Context;
using OpenAAP.Services.PasswordHashing;
using System;

namespace OpenAAP.Migrations
{
    [DbContext(typeof(OpenAAPContext))]
    [Migration("20180509191221_CreateDatabase")]
    partial class CreateDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("OpenAAP.Context.Identity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<string>("Email")
                        .HasMaxLength(64);

                    b.Property<string>("UserName")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Identity");
                });

            modelBuilder.Entity("OpenAAP.Context.PasswordAuthentication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Algorithm");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DisabledAt");

                    b.Property<byte[]>("Hash")
                        .IsRequired();

                    b.Property<Guid>("IdentityId");

                    b.Property<byte[]>("Salt")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("IdentityId");

                    b.ToTable("PasswordAuthentication");
                });

            modelBuilder.Entity("OpenAAP.Context.PasswordAuthentication", b =>
                {
                    b.HasOne("OpenAAP.Context.Identity", "Identity")
                        .WithMany("PasswordAuthentication")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
