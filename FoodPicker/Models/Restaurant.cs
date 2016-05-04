using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodPicker.Models
{
    public class Restaurant
    {
        public int RestaurantID { get; set; }

        [Required]
        [Display(Name ="Restaurant")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
                
        [Required]
        [StringLength(80, MinimumLength = 3)]
        public string StreetAddress { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string City { get; set; }
        [StringLength(50)]
        public string Province { get; set; }
        [StringLength(10)]
        public string PostalCode { get; set; }
        [StringLength(60)]
        public string Country { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }        
        public string Hours { get; set; }        

        public string Description { get; set; }

        public int UserID { get; set; }


        public virtual ICollection<Food> Foods { get; set; }

        public virtual  User Owner { get; set; }

    }
}