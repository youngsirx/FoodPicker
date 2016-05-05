namespace FoodPicker.Migrations.FoodMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestaurantHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "MondayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "TuesdayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "WednesdayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "ThursdayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "FridayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "SaturdayHours", c => c.String(maxLength: 15));
            AddColumn("dbo.Restaurant", "SundayHours", c => c.String(maxLength: 15));
            DropColumn("dbo.Restaurant", "Hours");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurant", "Hours", c => c.String());
            DropColumn("dbo.Restaurant", "SundayHours");
            DropColumn("dbo.Restaurant", "SaturdayHours");
            DropColumn("dbo.Restaurant", "FridayHours");
            DropColumn("dbo.Restaurant", "ThursdayHours");
            DropColumn("dbo.Restaurant", "WednesdayHours");
            DropColumn("dbo.Restaurant", "TuesdayHours");
            DropColumn("dbo.Restaurant", "MondayHours");
        }
    }
}
