using System;
using Microsoft.EntityFrameworkCore.Migrations;
using CarritoCompras_NT1.Extensions;

namespace CarritoCompras_NT1.Migrations
{
    public partial class primeraVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 30, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FechaAlta = table.Column<DateTime>(nullable: false),
                    Password = table.Column<byte[]>(nullable: true),
                    Direccion = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    Apellido = table.Column<string>(maxLength: 30, nullable: false),
                    Telefono = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Nombre", "Email", "FechaAlta", "Password", "UserName", "Apellido", "Telefono", "Direccion" },
                values: new object[] {Guid.NewGuid(), "Carlos", "cSantana@hotmail.com", DateTime.Now, "CSantana77".Encriptar()
                ,"CSantana77", "Santana","4747-2323", "San Lorenzo 1969" }
                );

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 30, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FechaAlta = table.Column<DateTime>(nullable: false),
                    Password = table.Column<byte[]>(nullable: true),
                    Direccion = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    Apellido = table.Column<string>(maxLength: 30, nullable: false),
                    Telefono = table.Column<string>(nullable: false),
                    DNI = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 30, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FechaAlta = table.Column<DateTime>(nullable: false),
                    Password = table.Column<byte[]>(nullable: true),
                    Direccion = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    Apellido = table.Column<string>(maxLength: 30, nullable: false),
                    Telefono = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 30, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Telefono = table.Column<string>(nullable: false),
                    Direccion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 30, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    PrecioVigente = table.Column<float>(nullable: false),
                    CategoriaID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaID",
                        column: x => x.CategoriaID,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carritos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    ClienteID = table.Column<Guid>(nullable: false),
                    Subtotal = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carritos_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SucursalID = table.Column<Guid>(nullable: false),
                    ProductoID = table.Column<Guid>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockItems_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockItems_Sucursales_SucursalID",
                        column: x => x.SucursalID,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarritoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CarritoID = table.Column<Guid>(nullable: false),
                    ProductoID = table.Column<Guid>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    ValorUnitario = table.Column<float>(nullable: false),
                    Subtotal = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Carritos_CarritoID",
                        column: x => x.CarritoID,
                        principalTable: "Carritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClienteID = table.Column<Guid>(nullable: false),
                    CarritoID = table.Column<Guid>(nullable: false),
                    Total = table.Column<float>(nullable: false),
                    FechaCompra = table.Column<DateTime>(nullable: false),
                    SucursalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Carritos_CarritoID",
                        column: x => x.CarritoID,
                        principalTable: "Carritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compras_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compras_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_CarritoID",
                table: "CarritoItems",
                column: "CarritoID");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_ProductoID",
                table: "CarritoItems",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_Carritos_ClienteID",
                table: "Carritos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_CarritoID",
                table: "Compras",
                column: "CarritoID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteID",
                table: "Compras",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_SucursalId",
                table: "Compras",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaID",
                table: "Productos",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_ProductoID",
                table: "StockItems",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_SucursalID",
                table: "StockItems",
                column: "SucursalID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "CarritoItems");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
