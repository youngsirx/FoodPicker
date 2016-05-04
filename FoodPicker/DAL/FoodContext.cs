using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FoodPicker.Models;

namespace FoodPicker.DAL
{
    public class FoodContext: DbContext
    {
        public FoodContext() : base("DefaultConnection")
        {

        }

        public DbSet<Food> Foods { get; set; }

        public DbSet<Category> Categories { get; set; }       
       
        public DbSet<Rating> Ratings { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Category>()
               .HasMany(f => f.Foods).WithMany(i => i.Categories)
               .Map(t => t.MapLeftKey("CategoryID")
                    .MapRightKey("FoodID")
                    .ToTable("FoodCategory"));

            modelBuilder.Entity<User>()
               .HasMany(f => f.Foods).WithMany(i => i.Users)
               .Map(t => t.MapLeftKey("UserID")
                    .MapRightKey("FoodID")
                    .ToTable("FavoriteFood"));
        }



   }
}