﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkflowEngineV1._0.Data;

#nullable disable

namespace WorkflowEngineV1._0.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Connection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EndTaskId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "endTaskId");

                    b.Property<string>("NextConnId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartTaskId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "startTaskId");

                    b.Property<int>("WorkflowId")
                        .HasColumnType("int");

                    b.Property<float>("XLoc")
                        .HasColumnType("real")
                        .HasAnnotation("Relational:JsonPropertyName", "xLoc");

                    b.Property<float>("YLoc")
                        .HasColumnType("real")
                        .HasAnnotation("Relational:JsonPropertyName", "yLoc");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowId");

                    b.ToTable("Connections");

                    b.HasAnnotation("Relational:JsonPropertyName", "connections");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkflowId")
                        .HasColumnType("int");

                    b.Property<bool>("isPublished")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowId")
                        .IsUnique();

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.TaskItem", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("StateDTO")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int?>("WorkflowId")
                        .HasColumnType("int");

                    b.Property<int>("X")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "x");

                    b.Property<int>("Y")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "y");

                    b.Property<string>("iconHTML")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "iconHtml");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowId");

                    b.ToTable("TaskItems");

                    b.HasAnnotation("Relational:JsonPropertyName", "tasks");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Workflow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "caption");

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasProblem")
                        .HasColumnType("bit");

                    b.Property<string>("ProblemTaskId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "workflowName");

                    b.HasKey("Id");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Connection", b =>
                {
                    b.HasOne("WorkflowEngineV1._0.Models.Workflow", "Workflow")
                        .WithMany("Connections")
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Document", b =>
                {
                    b.HasOne("WorkflowEngineV1._0.Models.Workflow", "Workflow")
                        .WithOne("Document")
                        .HasForeignKey("WorkflowEngineV1._0.Models.Document", "WorkflowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.TaskItem", b =>
                {
                    b.HasOne("WorkflowEngineV1._0.Models.Workflow", "Workflow")
                        .WithMany("Tasks")
                        .HasForeignKey("WorkflowId");

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("WorkflowEngineV1._0.Models.Workflow", b =>
                {
                    b.Navigation("Connections");

                    b.Navigation("Document");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
