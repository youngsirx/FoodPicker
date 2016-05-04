using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class Favourite
    {
        public int FoodID { get; set; }

        public int UserID { get; set; }

        public virtual Food Food { get; set; }
        public virtual User User { get; set; }

    }
}