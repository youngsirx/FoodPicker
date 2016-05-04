using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class Category
    {
        public int CategoryID { get; set; } //PK

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string CategoryName { get; set; }

        public virtual ICollection<Food> Foods { get; set; }
    }
}