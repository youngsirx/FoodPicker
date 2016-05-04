namespace FoodPicker.Migrations.FoodMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Food",
                c => new
                    {
                        FoodID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false),
                        ImageName = c.String(),
                        RestaurantID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        DateAdded = c.DateTime(nullable: false),
                        AverageRating = c.Single(),
                    })
                .PrimaryKey(t => t.FoodID)
                .ForeignKey("dbo.Restaurant", t => t.RestaurantID)
                .Index(t => t.RestaurantID);
            
            CreateTable(
                "dbo.Rating",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        FoodID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        UserRating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RatingID)
                .ForeignKey("dbo.Food", t => t.FoodID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.FoodID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 65),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Restaurant",
                c => new
                    {
                        RestaurantID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        StreetAddress = c.String(nullable: false, maxLength: 80),
                        City = c.String(nullable: false, maxLength: 50),
                        Province = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 10),
                        Country = c.String(maxLength: 60),
                        Phone = c.String(maxLength: 15),
                        Hours = c.String(),
                        Description = c.String(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RestaurantID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.FavoriteFood",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        FoodID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.FoodID })
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Food", t => t.FoodID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.FoodID);
            
            CreateTable(
                "dbo.FoodCategory",
                c => new
                    {
                        CategoryID = c.Int(nullable: false),
                        FoodID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CategoryID, t.FoodID })
                .ForeignKey("dbo.Category", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Food", t => t.FoodID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.FoodID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FoodCategory", "FoodID", "dbo.Food");
            DropForeignKey("dbo.FoodCategory", "CategoryID", "dbo.Category");
            DropForeignKey("dbo.Restaurant", "UserID", "dbo.User");
            DropForeignKey("dbo.Food", "RestaurantID", "dbo.Restaurant");
            DropForeignKey("dbo.Rating", "UserID", "dbo.User");
            DropForeignKey("dbo.FavoriteFood", "FoodID", "dbo.Food");
            DropForeignKey("dbo.FavoriteFood", "UserID", "dbo.User");
            DropForeignKey("dbo.Rating", "FoodID", "dbo.Food");
            DropIndex("dbo.FoodCategory", new[] { "FoodID" });
            DropIndex("dbo.FoodCategory", new[] { "CategoryID" });
            DropIndex("dbo.FavoriteFood", new[] { "FoodID" });
            DropIndex("dbo.FavoriteFood", new[] { "UserID" });
            DropIndex("dbo.Restaurant", new[] { "UserID" });
            DropIndex("dbo.Rating", new[] { "UserID" });
            DropIndex("dbo.Rating", new[] { "FoodID" });
            DropIndex("dbo.Food", new[] { "RestaurantID" });
            DropTable("dbo.FoodCategory");
            DropTable("dbo.FavoriteFood");
            DropTable("dbo.Restaurant");
            DropTable("dbo.User");
            DropTable("dbo.Rating");
            DropTable("dbo.Food");
            DropTable("dbo.Category");
        }
    }
}
