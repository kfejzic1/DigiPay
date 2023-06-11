using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcessingServer.DAL.Migrations
{
    public partial class AppDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserBankAccounts",
                columns: table => new
                {
                    UserBankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Credit = table.Column<double>(type: "double", nullable: false),
                    Debit = table.Column<double>(type: "double", nullable: false),
                    Total = table.Column<double>(type: "double", nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBankAccounts", x => x.UserBankAccountId);
                    table.ForeignKey(
                        name: "FK_UserBankAccounts_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorBankAccounts",
                columns: table => new
                {
                    VendorBankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Credit = table.Column<double>(type: "double", nullable: false),
                    Debit = table.Column<double>(type: "double", nullable: false),
                    Total = table.Column<double>(type: "double", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBankAccounts", x => x.VendorBankAccountId);
                    table.ForeignKey(
                        name: "FK_VendorBankAccounts_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionPurpose = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RecipientAccountId = table.Column<int>(type: "int", nullable: false),
                    SenderAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_UserBankAccounts_RecipientAccountId",
                        column: x => x.RecipientAccountId,
                        principalTable: "UserBankAccounts",
                        principalColumn: "UserBankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_UserBankAccounts_SenderAccountId",
                        column: x => x.SenderAccountId,
                        principalTable: "UserBankAccounts",
                        principalColumn: "UserBankAccountId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    VoucherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    UserBankAccountId = table.Column<int>(type: "int", nullable: false),
                    ActivatorId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.VoucherId);
                    table.ForeignKey(
                        name: "FK_Vouchers_UserBankAccounts_UserBankAccountId",
                        column: x => x.UserBankAccountId,
                        principalTable: "UserBankAccounts",
                        principalColumn: "UserBankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vouchers_Users_ActivatorId",
                        column: x => x.ActivatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EInvoicePaymentHistory",
                columns: table => new
                {
                    EInvoicePaymentHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayerBankAccountId = table.Column<int>(type: "int", nullable: false),
                    PayeeBankAccountId = table.Column<int>(type: "int", nullable: false),
                    PayerAddress = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reference = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EInvoicePaymentHistory", x => x.EInvoicePaymentHistoryId);
                    table.ForeignKey(
                        name: "FK_EInvoicePaymentHistory_UserBankAccounts_PayerBankAccountId",
                        column: x => x.PayerBankAccountId,
                        principalTable: "UserBankAccounts",
                        principalColumn: "UserBankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EInvoicePaymentHistory_VendorBankAccounts_PayeeBankAccountId",
                        column: x => x.PayeeBankAccountId,
                        principalTable: "VendorBankAccounts",
                        principalColumn: "VendorBankAccountId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoicePaymentHistory_PayeeBankAccountId",
                table: "EInvoicePaymentHistory",
                column: "PayeeBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoicePaymentHistory_PayerBankAccountId",
                table: "EInvoicePaymentHistory",
                column: "PayerBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecipientAccountId",
                table: "Transactions",
                column: "RecipientAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBankAccounts_OwnerId",
                table: "UserBankAccounts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBankAccounts_CreatorId",
                table: "VendorBankAccounts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_ActivatorId",
                table: "Vouchers",
                column: "ActivatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_UserBankAccountId",
                table: "Vouchers",
                column: "UserBankAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EInvoicePaymentHistory");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "VendorBankAccounts");

            migrationBuilder.DropTable(
                name: "UserBankAccounts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
