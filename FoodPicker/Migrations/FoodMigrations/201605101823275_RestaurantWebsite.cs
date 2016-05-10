namespace FoodPicker.Migrations.FoodMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestaurantWebsite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "Url", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restaurant", "Url");
        }
    }
}
