using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseBackend.Migrations
{
    public partial class num1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrGroup",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(nullable: false),
                    vahed = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrGroup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prgroupid = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.id);
                    table.ForeignKey(
                        name: "FK_Field_PrGroup_prgroupid",
                        column: x => x.prgroupid,
                        principalTable: "PrGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prgroupid = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: false),
                    price = table.Column<int>(nullable: false),
                    imagename = table.Column<string>(nullable: true),
                    count = table.Column<int>(nullable: false),
                    readyday = table.Column<int>(nullable: false),
                    sendday = table.Column<int>(nullable: false),
                    text = table.Column<string>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.id);
                    table.ForeignKey(
                        name: "FK_Product_PrGroup_prgroupid",
                        column: x => x.prgroupid,
                        principalTable: "PrGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleid = table.Column<int>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    username = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    lastlogin = table.Column<DateTime>(nullable: false),
                    activecode = table.Column<int>(nullable: false),
                    isactive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                    table.ForeignKey(
                        name: "FK_User_Role_roleid",
                        column: x => x.roleid,
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FillField",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fieldid = table.Column<int>(nullable: false),
                    productid = table.Column<int>(nullable: false),
                    text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FillField", x => x.id);
                    table.ForeignKey(
                        name: "FK_FillField_Field_fieldid",
                        column: x => x.fieldid,
                        principalTable: "Field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FillField_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Gallery",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productid = table.Column<int>(nullable: false),
                    imagename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gallery", x => x.id);
                    table.ForeignKey(
                        name: "FK_Gallery_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productid = table.Column<int>(nullable: false),
                    text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tag_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Takhfif",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productid = table.Column<int>(nullable: false),
                    starttime = table.Column<DateTime>(nullable: false),
                    endtime = table.Column<DateTime>(nullable: false),
                    darsad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Takhfif", x => x.id);
                    table.ForeignKey(
                        name: "FK_Takhfif_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productid = table.Column<int>(nullable: false),
                    userid = table.Column<int>(nullable: false),
                    text = table.Column<string>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comment_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<int>(nullable: false),
                    user2id = table.Column<int>(nullable: true),
                    text = table.Column<string>(nullable: false),
                    isseen = table.Column<bool>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false),
                    sendername = table.Column<string>(nullable: true),
                    senderlastname = table.Column<string>(nullable: true),
                    senderemail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.id);
                    table.ForeignKey(
                        name: "FK_Message_User_user2id",
                        column: x => x.user2id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    lastname = table.Column<string>(nullable: false),
                    phonenumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.id);
                    table.ForeignKey(
                        name: "FK_Person_User_id",
                        column: x => x.id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentReplay",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    commentid = table.Column<int>(nullable: false),
                    text = table.Column<string>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReplay", x => x.id);
                    table.ForeignKey(
                        name: "FK_CommentReplay_Comment_commentid",
                        column: x => x.commentid,
                        principalTable: "Comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personid = table.Column<int>(nullable: false),
                    ostan = table.Column<string>(nullable: false),
                    shahr = table.Column<string>(nullable: false),
                    codeposti = table.Column<string>(nullable: false),
                    text = table.Column<string>(nullable: false),
                    girandename = table.Column<string>(nullable: false),
                    girandelastname = table.Column<string>(nullable: false),
                    girandephonenumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id);
                    table.ForeignKey(
                        name: "FK_Address_Person_personid",
                        column: x => x.personid,
                        principalTable: "Person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<int>(nullable: false),
                    addressid = table.Column<int>(nullable: false),
                    total = table.Column<int>(nullable: false),
                    ispay = table.Column<bool>(nullable: false),
                    isready = table.Column<bool>(nullable: false),
                    issend = table.Column<bool>(nullable: false),
                    iscansel = table.Column<bool>(nullable: false),
                    createdate = table.Column<DateTime>(nullable: false),
                    paydate = table.Column<DateTime>(nullable: false),
                    senddate = table.Column<DateTime>(nullable: false),
                    paymentid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.id);
                    table.ForeignKey(
                        name: "FK_Basket_Address_addressid",
                        column: x => x.addressid,
                        principalTable: "Address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Basket_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "BasketItem",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    basketid = table.Column<int>(nullable: false),
                    productid = table.Column<int>(nullable: false),
                    count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItem", x => x.id);
                    table.ForeignKey(
                        name: "FK_BasketItem_Basket_basketid",
                        column: x => x.basketid,
                        principalTable: "Basket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItem_Product_productid",
                        column: x => x.productid,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_personid",
                table: "Address",
                column: "personid");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_addressid",
                table: "Basket",
                column: "addressid");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_userid",
                table: "Basket",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_basketid",
                table: "BasketItem",
                column: "basketid");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_productid",
                table: "BasketItem",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_productid",
                table: "Comment",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_userid",
                table: "Comment",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReplay_commentid",
                table: "CommentReplay",
                column: "commentid");

            migrationBuilder.CreateIndex(
                name: "IX_Field_prgroupid",
                table: "Field",
                column: "prgroupid");

            migrationBuilder.CreateIndex(
                name: "IX_FillField_fieldid",
                table: "FillField",
                column: "fieldid");

            migrationBuilder.CreateIndex(
                name: "IX_FillField_productid",
                table: "FillField",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_Gallery_productid",
                table: "Gallery",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_Message_user2id",
                table: "Message",
                column: "user2id");

            migrationBuilder.CreateIndex(
                name: "IX_Message_userid",
                table: "Message",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_prgroupid",
                table: "Product",
                column: "prgroupid");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_productid",
                table: "Tag",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_Takhfif_productid",
                table: "Takhfif",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_User_roleid",
                table: "User",
                column: "roleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketItem");

            migrationBuilder.DropTable(
                name: "CommentReplay");

            migrationBuilder.DropTable(
                name: "FillField");

            migrationBuilder.DropTable(
                name: "Gallery");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Takhfif");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "PrGroup");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
