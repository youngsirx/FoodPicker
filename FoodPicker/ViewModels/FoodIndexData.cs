using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodPicker.Models;

namespace FoodPicker.ViewModels
{
    public class FoodIndexData
    {
        public int FoodID { get; set; }

        public string Name { get; set; }
        public string ImageName {get; set;}
        public float? AverageRating { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; }

    }
}