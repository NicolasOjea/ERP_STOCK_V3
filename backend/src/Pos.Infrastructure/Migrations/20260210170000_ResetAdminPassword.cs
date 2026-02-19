using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Pos.Infrastructure.Persistence;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[DbContext(typeof(PosDbContext))]
[Migration("20260210170000_ResetAdminPassword")]
public partial class ResetAdminPassword : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE usuarios SET \"PasswordHash\" = 'sha256:8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918' WHERE \"Username\" = 'admin';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE usuarios SET \"PasswordHash\" = 'sha256:3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121' WHERE \"Username\" = 'admin';");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        // No model changes; data-only migration.
    }
}
