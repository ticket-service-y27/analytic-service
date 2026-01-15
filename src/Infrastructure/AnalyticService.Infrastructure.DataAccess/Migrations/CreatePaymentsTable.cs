using FluentMigrator;

namespace AnalyticService.Infrastructure.DataAccess.Migrations;

[Migration(1, "create payments table")]
public class CreatePaymentsTable : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE TYPE payment_status AS ENUM('pending', 'succeeded', 'failed', 'refunded');");

        Create.Table("payments")
            .WithColumn("payment_id").AsInt64().PrimaryKey()
            .WithColumn("wallet_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("status").AsCustom("payment_status").NotNullable()
            .WithColumn("amount").AsInt64().NotNullable()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);
    }

    public override void Down()
    {
        Delete.Table("payments");
        Execute.Sql("DROP TYPE IF EXISTS payment_status;");
    }
}