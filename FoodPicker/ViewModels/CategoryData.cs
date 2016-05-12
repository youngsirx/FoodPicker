using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPicker.ViewModels
{
    public class CategoryData
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        public bool Assigned { get; set; }

    }
}