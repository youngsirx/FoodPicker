using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodPicker.DAL;
using FoodPicker.Models;
using System.Threading.Tasks;
using FoodPicker.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodPicker.Controllers
{
    public class RestaurantController : Controller
    {
        private FoodContext db = new FoodContext();

        // GET: Restaurant
        public ActionResult Index()
        {
            var restaurants = db.Restaurants.Include(r => r.Owner);
            return View(restaurants.ToList());
        }

        // GET: Restaurant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Restaurant restaurant = db.Restaurants.Find(id);
            var restaurant = db.Restaurants.Include(r => r.Foods).Where(r => r.RestaurantID == id).SingleOrDefault();
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // GET: Restaurant/Create
        [Authorize(Roles = "owner, admin")]
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "owner, admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RestaurantID,Name,StreetAddress,City,Province,PostalCode,Country,Phone,MondayHours,TuesdayHours,WednesdayHours,ThursdayHours,FridayHours,SaturdayHours,SundayHours,Url,Description,UserID")] Restaurant restaurant, HttpPostedFileBase ImageName)
        {
            //jkhalack: modified the food image upload method to be used for restaurant pictures
            //jyoung: added image upload
            //added HttpPostedFileBase ImageName args in method
            //Also the form needs an enctype="multipart/form-data"
            if (ModelState.IsValid)
            {

                //check if you have anything to upload
                if (ImageName != null && ImageName.ContentLength > 0)
                {

                    var validImageTypes = new string[]
                   {
                        "image/gif",
                        "image/jpg",
                        "image/jpeg",
                        "image/png"
                   };
                    if (!validImageTypes.Contains(ImageName.ContentType))
                    {
                        //file being uploaded is not a jpg -display error
                        ModelState.AddModelError("", "Please use a JPG image only.");
                        ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", restaurant.UserID);

                        return View(restaurant);
                    }


                    //save new restaurant to database
                    db.Restaurants.Add(restaurant);
                    await db.SaveChangesAsync();

                    //retrieve the IDENTITY (new name for image) FROM sql sERVER
                    string pictureName = restaurant.RestaurantID.ToString();
                    
                    //next rename, scale an upload the image.
                    RestoImageUpload imageUpload = new RestoImageUpload { Width = 300, Height = 200 };
                    ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

                    //ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", restaurant.UserID);

                    return RedirectToAction("Details", new { id = restaurant.RestaurantID });
                }
                else
                {
                    ModelState.AddModelError("", "You have not selected an image file to upload.");
                    ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", restaurant.UserID);

                    return View(restaurant);
                }




                //db.Restaurants.Add(restaurant);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }//end ModelState

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", restaurant.UserID);
            return View(restaurant);
        }

        // GET: Restaurant/Edit/5
        [Authorize(Roles = "owner, admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", restaurant.UserID);
            return View(restaurant);
        }

        // POST: Restaurant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "owner, admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, HttpPostedFileBase ImageName)
        {
            //[Bind(Include = "RestaurantID,Name,StreetAddress,City,Province,
            //PostalCode,Country,Phone,
            //MondayHours,TuesdayHours,WednesdayHours,ThursdayHours,FridayHours,SaturdayHours,SundayHours,
            //Description,Url,UserID")]
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Restaurant restaurantToUpdate = db.Restaurants.Find(id);
            
            if ( !(
                User.IsInRole("admin") ||
                ( User.IsInRole("owner") &&
                currentUserID() == restaurantToUpdate.UserID )
                ) )
            { return View("NotYourRestaurant"); }

            if (TryUpdateModel(restaurantToUpdate, "",
               new string[] { "Name", "StreetAddress", "City", "Province", "PostalCode", "Country", "Phone",
               "MondayHours", "TuesdayHours", "WednesdayHours", "ThursdayHours", "FridayHours",
                   "SaturdayHours", "SundayHours", "Description", "Url", "UserID"}))
            {
                try
                {
                    db.SaveChanges();
                    if (ImageName != null && ImageName.ContentLength > 0)
                    {
                        var validImageTypes = new string[]
                     {
                        "image/gif",
                        "image/jpg",
                        "image/jpeg",
                         "image/png"
                     };
                        if (!validImageTypes.Contains(ImageName.ContentType))
                        {
                            //file being uploaded is not a jpg -display error
                            ModelState.AddModelError("", "Please use a JPG image only.");
                            return View(restaurantToUpdate);
                        }
                        //retrieve the IDENTITY (new name for image) FROM sql sERVER
                        string pictureName = id.ToString();
                        //next rename, scale an upload the image.
                        RestoImageUpload imageUpload = new RestoImageUpload { Width = 300, Height = 200 };
                        if (imageUpload.DeleteImage(restaurantToUpdate.ImageName))
                        {
                            ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);
                        }
                    }
                    return RedirectToAction("Details", new { id = restaurantToUpdate.RestaurantID });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again later!");
                }
            }
            return View(restaurantToUpdate);

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "RestaurantID,Name,StreetAddress,City,Province,PostalCode,Country,Phone,MondayHours,TuesdayHours,WednesdayHours,ThursdayHours,FridayHours,SaturdayHours,SundayHours,Description,Url,UserID")] Restaurant restaurant, HttpPostedFileBase ImageName)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (ImageName != null && ImageName.ContentLength > 0)
        //        {
        //            var validImageTypes = new string[]
        //         {
        //                //"image/gif",
        //                "image/jpg",
        //                "image/jpeg"
        //             //,
        //             //"image/png"
        //         };
        //            if (!validImageTypes.Contains(ImageName.ContentType))
        //            {
        //                //file being uploaded is not a jpg -display error
        //                ModelState.AddModelError("", "Please use a JPG image only.");                        
        //                return View(restaurant);
        //            }
        //            //retrieve the IDENTITY (new name for image) FROM sql sERVER
        //            string pictureName = restaurant.RestaurantID.ToString();
        //            //next rename, scale an upload the image.
        //            RestoImageUpload imageUpload = new RestoImageUpload { Width = 300, Height = 200 };
        //            if(imageUpload.DeleteImage(restaurant.ImageName))
        //            {
        //                ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);
        //            }
        //        }
        //        db.Entry(restaurant).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Details",new { id = restaurant.RestaurantID });
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", restaurant.UserID);
        //    return View(restaurant);
        //}

        // GET: Restaurant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            db.Restaurants.Remove(restaurant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //jkhalack: management view for restaurant owner
        [Authorize(Roles ="owner")]
        public ActionResult MyRestaurant()
        {
            //find the owner that is currently logged in
            string userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var manager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(ApplicationDbContext.Create())
                    );
                var currentUser = manager.FindByEmail(User.Identity.GetUserName());
                //get the restaurant entity for this logged in user
                User owner = db.Users.Where(i => i.Email == currentUser.Email).Single();
                Restaurant restaurant = db.Restaurants
                    .Include(u => u.Owner)
                    .Where(i => i.UserID == owner.UserID).SingleOrDefault();

                if(restaurant!=null)
                {
                    return View("Details",restaurant);
                }else
                {
                    ViewBag.OwnerID = owner.UserID;
                    return View("Create");
                }                
            }
            else
            {
                return HttpNotFound();
            }
                
        }
        private int? currentUserID()
        {
            string userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var manager = new UserManager<ApplicationUser>(
                   new UserStore<ApplicationUser>(ApplicationDbContext.Create())
                   );
                var currentUser = manager.FindByEmail(User.Identity.GetUserName());
                //get the restaurant entity for this logged in user
                User user = db.Users.Where(i => i.Email == currentUser.Email).SingleOrDefault();
                if ( user != null ) {
                    return user.UserID;
                }
            }
             return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
