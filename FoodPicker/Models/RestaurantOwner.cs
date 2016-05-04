using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class RestaurantOwner:User
    {
       // public int RestaurantID { get; set; }

        public virtual Restaurant Restaurant { get; set; }
    }
}