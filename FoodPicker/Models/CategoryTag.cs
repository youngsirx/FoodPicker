using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class CategoryTag
    {
        public int CategoryID { get; set; }
        public int FoodID { get; set; }


        public virtual Category Category { get; set; }

        public virtual Food Food { get; set; }
    }
}