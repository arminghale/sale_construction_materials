﻿// <auto-generated />
using System;
using DataBaseBackend;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataBaseBackend.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20210307071845_num2")]
    partial class num2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataBaseBackend.Address", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("codeposti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("girandelastname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("girandename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("girandephonenumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ostan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("personid")
                        .HasColumnType("int");

                    b.Property<string>("shahr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("personid");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("DataBaseBackend.Basket", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("addressid")
                        .HasColumnType("int");

                    b.Property<DateTime>("createdate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("iscansel")
                        .HasColumnType("bit");

                    b.Property<bool>("ispay")
                        .HasColumnType("bit");

                    b.Property<bool>("isready")
                        .HasColumnType("bit");

                    b.Property<bool>("issend")
                        .HasColumnType("bit");

                    b.Property<DateTime>("paydate")
                        .HasColumnType("datetime2");

                    b.Property<int>("paymentid")
                        .HasColumnType("int");

                    b.Property<DateTime>("senddate")
                        .HasColumnType("datetime2");

                    b.Property<int>("total")
                        .HasColumnType("int");

                    b.Property<int>("userid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("addressid");

                    b.HasIndex("userid");

                    b.ToTable("Basket");
                });

            modelBuilder.Entity("DataBaseBackend.BasketItem", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("basketid")
                        .HasColumnType("int");

                    b.Property<int>("count")
                        .HasColumnType("int");

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("basketid");

                    b.HasIndex("productid");

                    b.ToTable("BasketItem");
                });

            modelBuilder.Entity("DataBaseBackend.Comment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdate")
                        .HasColumnType("datetime2");

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("productid");

                    b.HasIndex("userid");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("DataBaseBackend.CommentReplay", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("commentid")
                        .HasColumnType("int");

                    b.Property<DateTime>("createdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("commentid");

                    b.ToTable("CommentReplay");
                });

            modelBuilder.Entity("DataBaseBackend.Field", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("prgroupid")
                        .HasColumnType("int");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("prgroupid");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("DataBaseBackend.FillField", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("fieldid")
                        .HasColumnType("int");

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.Property<string>("text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("fieldid");

                    b.HasIndex("productid");

                    b.ToTable("FillField");
                });

            modelBuilder.Entity("DataBaseBackend.Gallery", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("imagename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("productid");

                    b.ToTable("Gallery");
                });

            modelBuilder.Entity("DataBaseBackend.Message", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("createdate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isseen")
                        .HasColumnType("bit");

                    b.Property<string>("senderemail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("senderlastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sendername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("user2id")
                        .HasColumnType("int");

                    b.Property<int>("userid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("user2id");

                    b.HasIndex("userid");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("DataBaseBackend.Person", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<string>("lastname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phonenumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("DataBaseBackend.PrGroup", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("vahed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("PrGroup");
                });

            modelBuilder.Entity("DataBaseBackend.Product", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("count")
                        .HasColumnType("int");

                    b.Property<DateTime>("createdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("imagename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("prgroupid")
                        .HasColumnType("int");

                    b.Property<int>("price")
                        .HasColumnType("int");

                    b.Property<int>("readyday")
                        .HasColumnType("int");

                    b.Property<int>("sendday")
                        .HasColumnType("int");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("prgroupid");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("DataBaseBackend.Role", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("DataBaseBackend.Tag", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.Property<string>("text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("productid");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("DataBaseBackend.Takhfif", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("darsad")
                        .HasColumnType("int");

                    b.Property<DateTime>("endtime")
                        .HasColumnType("datetime2");

                    b.Property<int>("productid")
                        .HasColumnType("int");

                    b.Property<DateTime>("starttime")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.HasIndex("productid");

                    b.ToTable("Takhfif");
                });

            modelBuilder.Entity("DataBaseBackend.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("activecode")
                        .HasColumnType("int");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isactive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("lastlogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("roleid")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("roleid");

                    b.ToTable("User");
                });

            modelBuilder.Entity("DataBaseBackend.Address", b =>
                {
                    b.HasOne("DataBaseBackend.Person", "Person")
                        .WithMany("Addresses")
                        .HasForeignKey("personid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Basket", b =>
                {
                    b.HasOne("DataBaseBackend.Address", "Address")
                        .WithMany()
                        .HasForeignKey("addressid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataBaseBackend.User", "User")
                        .WithMany("Baskets")
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.BasketItem", b =>
                {
                    b.HasOne("DataBaseBackend.Basket", "Basket")
                        .WithMany("BasketItems")
                        .HasForeignKey("basketid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("BasketItems")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Comment", b =>
                {
                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("Comments")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataBaseBackend.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.CommentReplay", b =>
                {
                    b.HasOne("DataBaseBackend.Comment", "Comment")
                        .WithMany("CommentReplays")
                        .HasForeignKey("commentid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Field", b =>
                {
                    b.HasOne("DataBaseBackend.PrGroup", "PrGroup")
                        .WithMany("Fields")
                        .HasForeignKey("prgroupid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.FillField", b =>
                {
                    b.HasOne("DataBaseBackend.Field", "Field")
                        .WithMany("FillFields")
                        .HasForeignKey("fieldid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("FillFields")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Gallery", b =>
                {
                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("Galleries")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Message", b =>
                {
                    b.HasOne("DataBaseBackend.User", "User2")
                        .WithMany("Messages")
                        .HasForeignKey("user2id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataBaseBackend.User", "User")
                        .WithMany("Messages2")
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Person", b =>
                {
                    b.HasOne("DataBaseBackend.User", "User")
                        .WithOne("Person")
                        .HasForeignKey("DataBaseBackend.Person", "id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Product", b =>
                {
                    b.HasOne("DataBaseBackend.PrGroup", "PrGroup")
                        .WithMany("Products")
                        .HasForeignKey("prgroupid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Tag", b =>
                {
                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("Tags")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.Takhfif", b =>
                {
                    b.HasOne("DataBaseBackend.Product", "Product")
                        .WithMany("Takhfifs")
                        .HasForeignKey("productid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseBackend.User", b =>
                {
                    b.HasOne("DataBaseBackend.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("roleid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
