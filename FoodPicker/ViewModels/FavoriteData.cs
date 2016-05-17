using FoodPicker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPicker.ViewModels
{
    public class FavoriteData
    {
        public IEnumerable<Food> Foods { get; set; }
        public IEnumerable<User> Users { get; set; }

    }
}