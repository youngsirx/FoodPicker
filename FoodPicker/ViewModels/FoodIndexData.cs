using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodPicker.Models;

namespace FoodPicker.ViewModels
{
    public class FoodIndexData
    {
        public IEnumerable<Food> Foods { get; set; }
    }
}