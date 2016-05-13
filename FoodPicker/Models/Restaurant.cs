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
        [Display(Name ="Restaurant Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
                
        [Required]
        [StringLength(80, MinimumLength = 3)]
        [Display(Name = "Address")]
        public string StreetAddress { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string City { get; set; }
        [StringLength(50)]
        public string Province { get; set; }
        [StringLength(10)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [StringLength(60)]
        public string Country { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        //hours
        [Display(Name = "Monday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string MondayHours { get; set; }
        [Display(Name = "Tuesday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string TuesdayHours { get; set; }
        [Display(Name = "Wednesday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string WednesdayHours { get; set; }
        [Display(Name = "Thursday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string ThursdayHours { get; set; }
        [Display(Name = "Friyday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string FridayHours { get; set; }
        [Display(Name = "Saturday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string SaturdayHours { get; set; }
        [Display(Name = "Sunday")]
        [DisplayFormat(NullDisplayText = "Closed")]
        [StringLength(15)]
        public string SundayHours { get; set; }

        public string Description { get; set; }

        [Display(Name ="Website")]
        [DisplayFormat(NullDisplayText = "No website provided")]
        [DataType(DataType.Url)]
        [StringLength(100)]
        public string Url { get; set; }

        [Display(Name = "Owner")]
        public int UserID { get; set; }


        public virtual ICollection<Food> Foods { get; set; }

        public virtual  User Owner { get; set; }

        public string Address
        {
            get
            {
                return StreetAddress + ", " + City + ", " + Province + ", " + PostalCode + " " + Country;
            }
        }

        public string ImageName
        {
            get
            {
                return RestaurantID + ".jpg";
            }
        }
    }
}