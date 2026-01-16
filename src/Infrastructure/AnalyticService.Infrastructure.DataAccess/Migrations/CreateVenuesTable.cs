using FluentMigrator;

namespace AnalyticService.Infrastructure.DataAccess.Migrations;

[Migration(3, "create venues table")]
public class CreateVenuesTable : Migration
{
    public override void Up()
    {
        Create.Table("venues")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("total_seats").AsInt64().NotNullable()
            .WithColumn("address").AsString(400).NotNullable()
            .WithColumn("hall_scheme_id").AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("venues");
    }
}