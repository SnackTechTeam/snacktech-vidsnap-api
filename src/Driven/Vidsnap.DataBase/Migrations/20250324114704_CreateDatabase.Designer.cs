﻿// <auto-generated />
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vidsnap.DataBase.Context;

#nullable disable

namespace Vidsnap.DataBase.Migrations
{
    [ExcludeFromCodeCoverage]
    [DbContext(typeof(AppDbContext))]
    [Migration("20250324114704_CreateDatabase")]
    partial class CreateDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Vidsnap.Domain.Entities.Video", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DataInclusao")
                        .HasColumnType("datetime");

                    b.Property<int>("Duracao")
                        .HasColumnType("int");

                    b.Property<string>("EmailUsuario")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("varchar");

                    b.Property<string>("Extensao")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("varchar");

                    b.Property<Guid>("IdUsuario")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NomeVideo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar");

                    b.Property<int>("StatusAtual")
                        .HasColumnType("int");

                    b.Property<int>("Tamanho")
                        .HasColumnType("int");

                    b.Property<string>("URLImagem")
                        .HasMaxLength(500)
                        .HasColumnType("varchar");

                    b.Property<string>("URLZip")
                        .HasMaxLength(500)
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("Video", (string)null);
                });

            modelBuilder.Entity("Vidsnap.Domain.Entities.VideoStatus", b =>
                {
                    b.Property<Guid>("IdVideo")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataInclusao")
                        .HasColumnType("datetime");

                    b.HasKey("IdVideo", "Status");

                    b.ToTable("VideoStatus", (string)null);
                });

            modelBuilder.Entity("Vidsnap.Domain.Entities.VideoStatus", b =>
                {
                    b.HasOne("Vidsnap.Domain.Entities.Video", "Video")
                        .WithMany("VideoStatuses")
                        .HasForeignKey("IdVideo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Video");
                });

            modelBuilder.Entity("Vidsnap.Domain.Entities.Video", b =>
                {
                    b.Navigation("VideoStatuses");
                });
#pragma warning restore 612, 618
        }
    }
}
