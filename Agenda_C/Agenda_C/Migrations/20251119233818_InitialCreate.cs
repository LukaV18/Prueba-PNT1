using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda_C.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coberturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCredencial = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: true),
                    Prestadora = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coberturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Provincia = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ProfesionalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prestaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Duracion = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonaId = table.Column<int>(type: "int", nullable: false),
                    NameUser = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EmailUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DNI = table.Column<int>(type: "int", nullable: false),
                    PersonaType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolPrincipal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoberturaId = table.Column<int>(type: "int", nullable: true),
                    MatriculaId = table.Column<int>(type: "int", nullable: true),
                    PrestacionId = table.Column<int>(type: "int", nullable: true),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Coberturas_CoberturaId",
                        column: x => x.CoberturaId,
                        principalTable: "Coberturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Prestaciones_PrestacionId",
                        column: x => x.PrestacionId,
                        principalTable: "Prestaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Direcciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Altura = table.Column<int>(type: "int", nullable: false),
                    Piso = table.Column<int>(type: "int", nullable: true),
                    Departamento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PersonaId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Direcciones_AspNetUsers_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DNI = table.Column<int>(type: "int", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CoberturaId = table.Column<int>(type: "int", nullable: true),
                    Prestadora = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formularios_AspNetUsers_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Formularios_Coberturas_CoberturaId",
                        column: x => x.CoberturaId,
                        principalTable: "Coberturas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Telefonos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefijo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoDeTelefono = table.Column<int>(type: "int", nullable: false),
                    PersonaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdministradorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefonos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telefonos_AspNetUsers_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Telefonos_AspNetUsers_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDeAlta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PacienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfesionalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Confirmado = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    DescripcionCancelacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsuarioId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turnos_AspNetUsers_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turnos_AspNetUsers_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turnos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Turnos_AspNetUsers_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CoberturaId",
                table: "AspNetUsers",
                column: "CoberturaId",
                unique: true,
                filter: "[CoberturaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MatriculaId",
                table: "AspNetUsers",
                column: "MatriculaId",
                unique: true,
                filter: "[MatriculaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PrestacionId",
                table: "AspNetUsers",
                column: "PrestacionId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Coberturas_NumeroCredencial",
                table: "Coberturas",
                column: "NumeroCredencial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_PersonaId",
                table: "Direcciones",
                column: "PersonaId",
                unique: true,
                filter: "[PersonaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_CoberturaId",
                table: "Formularios",
                column: "CoberturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_PacienteId",
                table: "Formularios",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_Numero",
                table: "Matriculas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestaciones_Nombre",
                table: "Prestaciones",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Telefonos_AdministradorId",
                table: "Telefonos",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Telefonos_PersonaId",
                table: "Telefonos",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_PacienteId",
                table: "Turnos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ProfesionalId",
                table: "Turnos",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_UsuarioId",
                table: "Turnos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_UsuarioId1",
                table: "Turnos",
                column: "UsuarioId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Direcciones");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropTable(
                name: "Telefonos");

            migrationBuilder.DropTable(
                name: "Turnos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Coberturas");

            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Prestaciones");
        }
    }
}
