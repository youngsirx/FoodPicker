using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        public int FoodID { get; set; }
        public int UserID { get; set; }

        [Required]
        [Range(0, 5)]
        public int UserRating { get; set; }

        public virtual Food Food { get; set; }
        public virtual User User { get; set; }
    }
}