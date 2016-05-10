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

namespace FoodPicker.Controllers
{
    public class FoodController : Controller
    {
        private FoodContext db = new FoodContext();

        // GET: Food
        public ActionResult Index()
        {
            var foods = db.Foods.Include(f => f.Restaurant);
            return View(foods.ToList());
        }

        // GET: Food/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // GET: Food/Create
        public ActionResult Create()
        {
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name");
            return View();
        }

        // POST: Food/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FoodID,Name,Description,RestaurantID,Price")] Food food, HttpPostedFileBase ImageName)
        {

            ViewBag.ResturantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);

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
                        //"image/gif",
                        //"image/jpg",
                        //"image/jpeg",
                        "image/png"
                   };
                    if (!validImageTypes.Contains(ImageName.ContentType))
                    {
                        //file being uploaded is not a png -display error
                        ModelState.AddModelError("", "Please use a PNG image only.");
                        return View(food);
                    }

                    food.DateAdded = DateTime.Today;
                    
                    
                    //save new food to database
                    db.Foods.Add(food);
                    await db.SaveChangesAsync();

                    //retrieve the IDENTITY (new name for image) FROM sql sERVER
                    string pictureName = food.FoodID.ToString();


                    //next rename, scale an upload the image.
                    ImageUpload imageUpload = new ImageUpload { Width = 200, Height = 275 };
                    ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

                    food.ImageName = pictureName + ".png";
                    db.Entry(food).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = food.FoodID});
                }
                else
                {
                    ModelState.AddModelError("", "You have not selected an image file to upload.");
                    return View(food);
                }
               
            }//end of modelstate

            
            return View(food);
        }

        // GET: Food/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
            return View(food);
        }

        // POST: Food/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Description,Price")] Food food, HttpPostedFileBase ImageName)
        {

            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
            if (ModelState.IsValid)
            {
                //check if you have anything to upload
                if (ImageName != null && ImageName.ContentLength > 0)
                {

                    var validImageTypes = new string[]
                   {
                        //"image/gif",
                        //"image/jpg",
                        //"image/jpeg",
                        "image/png"
                   };
                    if (!validImageTypes.Contains(ImageName.ContentType))
                    {
                        //file being uploaded is not a png -display error
                        ModelState.AddModelError("", "Please use a PNG image only.");
                        return View(food);
                    }
                                       
                    //save new food to database
                    db.Foods.Add(food);
                    await db.SaveChangesAsync();

                    //retrieve the IDENTITY (new name for image) FROM sql sERVER
                    string pictureName = food.ImageName.ToString();
                    
                    //next rename, scale an upload the image.
                    ImageUpload imageUpload = new ImageUpload { Width = 200, Height = 275 };
                    ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

                    
                    return RedirectToAction("Details", new { id = food.FoodID });
                }
              
  
            }
          
            return View(food);
        }
        //public async Task<ActionResult> EditPost(int? id, HttpPostedFileBase ImageName)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var foodToUpdate = db.Foods.Find(id);
        //    if (TryUpdateModel(foodToUpdate, "",
        //        new string[] { "Name", "Description", "Price" }))
        //    {

        //        if (ImageName != null && ImageName.ContentLength > 0)
        //        {
        //            //do we have anything to upload - yes
        //            var validImageTypes = new string[]
        //            {
        //                //"image/gif",
        //                //"image/jpg",
        //                //"image/jpeg",
        //                "image/png"
        //            };
        //            if (!validImageTypes.Contains(ImageName.ContentType))
        //            {
        //                //file being uploaded is not a png -display error
        //                ModelState.AddModelError("", "Please choose a PNG image.");
        //                return View(foodToUpdate);

        //            }

        //            try
        //            {
        //                await db.SaveChangesAsync();
        
        //                string pictureName = foodToUpdate.ToString();

        //                //next rename, scale an upload the image.
        //                ImageUpload imageUpload = new ImageUpload { Width = 200, Height = 275 };
        //                ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

        //                return RedirectToAction("Details", new {id = foodToUpdate });
        //            }
        //          
        //        }

        //    }
        //    return View("Details", new { id = foodToUpdate });
        //}




        // GET: Food/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = db.Foods.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        // POST: Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            

            Food food = db.Foods.Find(id);

            string image = food.ImageName.ToString();
           

            db.Foods.Remove(food);
            db.SaveChanges();
            return RedirectToAction("Index");
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
