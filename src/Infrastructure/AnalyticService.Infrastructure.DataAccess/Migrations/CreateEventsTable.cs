using FluentMigrator;

namespace AnalyticService.Infrastructure.DataAccess.Migrations;

[Migration(2, "create events table")]
public class CreateEventsTable : Migration
{
    public override void Up()
    {
        Create.Table("events")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("start_date").AsDateTimeOffset().NotNullable()
            .WithColumn("venue_id").AsInt64().NotNullable()
            .WithColumn("artist_id").AsInt64().NotNullable()
            .WithColumn("total_seats").AsInt64().NotNullable()
            .WithColumn("occupied_seats").AsInt64().NotNullable().WithDefaultValue(0)
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefaultValue(DateTimeOffset.UtcNow);
    }

    public override void Down()
    {
        Delete.Table("events");
    }
}