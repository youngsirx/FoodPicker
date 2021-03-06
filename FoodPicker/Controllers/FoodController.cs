﻿using System;
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
using FoodPicker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FoodPicker.Controllers
{
    public class FoodController : Controller
    {
        private FoodContext db = new FoodContext();

        // GET: Food
        [AllowAnonymous]
        public ActionResult Index()
        {
            var foods = db.Foods.Include(f => f.Restaurant);
            return View(foods.ToList());
        }

        // GET: Food/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            //var currentUser = manager.FindById(User.Identity.GetUserId());
            //var viewModel = new FavoriteData();
            //viewModel.Users = db.Users.Include(i => i.Foods).Where(i => i.Email == currentUser.Email);

            //var user = viewModel.Users.Where(i => i.Email == currentUser.Email).Single();

            //viewModel.Foods = viewModel.Users.Where(i => i.UserID == user.UserID).Single().Foods;
           
            
            
         

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Food food = db.Foods
               .Include(i => i.Categories)
               .Where(i => i.FoodID == id).Single();


            var categories = db.Categories.Include(i => i.Foods).Where(i => i.CategoryID == food.FoodID).ToList();

            ViewBag.categoryName = categories;

            if (food == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("owner")) {
                ViewBag.isOwner = (food.Restaurant.UserID == currentUserID());
            }
            if(User.IsInRole("user")){
                int userID = (int) currentUserID();
                User user = db.Users.Include(i => i.Foods).Where(i => i.UserID == userID ).SingleOrDefault();
                ViewBag.isFavorite = user.Foods.Contains(food); 
            }
            

            return View(food);
        }

        // Post: Food/Details/5
        [HttpPost, ActionName("Details")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddDetails(int? id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            User userToUpdate = db.Users.Include(i => i.Foods).Where(i => i.Email == currentUser.Email).SingleOrDefault();
            Food foodToUpdate = db.Foods.Where(i => i.FoodID == id).SingleOrDefault();

            //jhkalack: change of logic
            if (userToUpdate.Foods.Contains(foodToUpdate))
            { userToUpdate.Foods.Remove(foodToUpdate); }
            else { userToUpdate.Foods.Add(foodToUpdate); }

            //var user = db.Users.Where(i => i.Email == currentUser.Email).Single();
            ////var fav = db.Foods.Find(id);
            ////var removals = fav.Users.Single();

            //var fav = db.Foods.Find(id);

            //var removals = fav.Users.Where(i => i.UserID == user.UserID).SingleOrDefault();
            


            //if (foodToUpdate != null)
            //{
            //    //if (removals != null)
            //    //{
                    
            //    //}
            //    //else
            //    //{
            //        userToUpdate.Foods.Add(foodToUpdate);                    
            //    //}
                
            //}
     
            db.SaveChanges();


            return RedirectToAction("Details", new { id = id});
                  

        }


        //jyoung added check box
        // GET: Food/Create
        [Authorize(Roles ="admin, owner")]
        public ActionResult Create()
        {
            if (User.IsInRole("owner"))
            {
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
                    if (restaurant == null) { return View("NoRestaurant"); }
                    ViewBag.RestoID = restaurant.RestaurantID;
                }
                
            }
            else
            {
                ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name");
            }

          
            var food = new Food();
            food.Categories = new List<Category>();
            PopulateAssignedCategories(food);

            return View();
        }

        // POST: Food/Create
        //jyoung added check box
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin, owner")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,RestaurantID,Price")] Food food, HttpPostedFileBase ImageName, string[] selectedCategory,string categoryname)
        {
            food.Categories = new List<Category>();

            if (selectedCategory != null)
            {
                //food.Categories = new List<Category>();


                foreach (var cat in selectedCategory)
                {
                    var categoryToAdd = db.Categories.Find(int.Parse(cat));
                    food.Categories.Add(categoryToAdd);
                 
                }
            }
            Category category = new Category();
                    
                
                if (!string.IsNullOrEmpty(categoryname))
                {
                if(!(db.Categories.Any(i=>i.CategoryName == categoryname)))
                {
                    //Category category = new Category();
                    category.CategoryName = categoryname;
                    db.Categories.Add(category);
                    await db.SaveChangesAsync();
                    food.Categories.Add(category);
                }
                else
                {
                    category = db.Categories.Where(i => i.CategoryName == categoryname).SingleOrDefault();
                    food.Categories.Add(category);
                }
    
            }
               
      
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
                        //file being uploaded is not a png -display error
                        ModelState.AddModelError("", "Please use a PNG image only.");

                        //jkhalack: we need to provide this list in a view bag to display a dropdown list for the restaurants
                        if (User.IsInRole("owner")) { ViewBag.RestoID = food.RestaurantID; }
                        else
                        {
                            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
                        }
                        PopulateAssignedCategories(food);
                        //jkhalack: end view bag

                        return View(food);
                    }

                    food.DateAdded = DateTime.Today;

                    //save new food to database
                    db.Foods.Add(food);
                    await db.SaveChangesAsync();

                    //retrieve the IDENTITY (new name for image) FROM sql sERVER
                    string pictureName = food.FoodID.ToString();

                    //next rename, scale an upload the image.
                    ImageUpload imageUpload = new ImageUpload { Width = 300, Height = 200 };
                    ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

                    food.ImageName = pictureName + ".png";
                    db.Entry(food).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                  

                    return RedirectToAction("Details", new { id = food.FoodID });
                }
                else
                {
                    ModelState.AddModelError("", "You have not selected an image file to upload.");

                    //jkhalack: we need to provide this list in a view bag to display a dropdown list for the restaurants
                    if (User.IsInRole("owner")) { ViewBag.RestoID = food.RestaurantID; }
                    else { 
                    ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
                    }
                    PopulateAssignedCategories(food);
                    //jkhalack: end view bag

                    return View(food);
                }

            }//end of modelstate


            if (User.IsInRole("owner")) { ViewBag.RestoID = food.RestaurantID; }
            else
            {
                ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
            }

            PopulateAssignedCategories(food);

            return View(food);
        }

        //Jyoung added controller to receive the categroies
        private void PopulateAssignedCategories(Food food)
        {
            var allCategories = db.Categories;
            HashSet<int> foodCategories;
            if (food.Categories !=null)
            {
                foodCategories = new HashSet<int>(food.Categories.Select(c => c.CategoryID));
            }
            else
            {
                foodCategories = new HashSet<int>();
            }

            

            var viewModel = new List<CategoryData>();

            foreach (var category in allCategories)
            {
                viewModel.Add(new CategoryData
                {
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName,
                    Assigned = foodCategories.Contains(category.CategoryID)
                });
            }
            ViewBag.Catagories = viewModel;

        }

        // GET: Food/Edit/5
        //jyoung added check box
        [Authorize(Roles = "admin, owner")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //replaced scffolded code to include office assignements
            //Food food = db.Foods.Find(id);
            Food food = db.Foods
                .Include(i => i.Categories)
                .Where(i => i.FoodID == id).Single();

            PopulateAssignedCategories(food);
            if (food == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("owner"))
            {
                ViewBag.isOwner = (food.Restaurant.UserID == currentUserID());
            }else
            {
                ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);            
            }
            return View(food);
        }

        // POST: Food/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "admin, owner")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id,  string[] selectedCategory, HttpPostedFileBase ImageName, string categoryname)
        {
            
            var foodToUpdate = db.Foods
               .Include(i => i.Categories)
               .Where(i => i.FoodID == id).Single();
            if (User.IsInRole("owner") && (foodToUpdate.Restaurant.UserID != currentUserID()))
            {
               return RedirectToAction("Details", new { id = id });
            }

            if (ModelState.IsValid)
            {
                if (TryUpdateModel(foodToUpdate, "",
                 new string[] { "Name", "Description", "Price" }))
                {
                    try
                    {
                        UpdateFoodCategory(selectedCategory, foodToUpdate);
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(categoryname))
                        {
                            Category category = new Category();
                            if(!(db.Categories.Any(i=>i.CategoryName == categoryname)))
                            {
                                category.CategoryName = categoryname;
                                db.Categories.Add(category);
                                db.SaveChanges();
                                foodToUpdate.Categories.Add(category);
                                db.SaveChanges();
                            }
                            else
                            {
                                category = db.Categories.Where(i => i.CategoryName == categoryname).SingleOrDefault();
                                foodToUpdate.Categories.Add(category);
                                db.SaveChanges();
                            }
  
                        }

                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again later!");
                    }
                }

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
                        ModelState.AddModelError("", "Please use a PNG image only.");

                        //jkhalack: populate ViewBag to retry the edit
                        PopulateAssignedCategories(foodToUpdate);
                        ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", foodToUpdate.RestaurantID);
                        //jkhalack: end populate ViewBag to retry the edit

                        return View(foodToUpdate);
                    }

                    //retrieve the IDENTITY (new name for image) FROM sql sERVER
                    string pictureName = foodToUpdate.FoodID.ToString();

                    //next rename, scale an upload the image.
                    ImageUpload imageUpload = new ImageUpload { Width = 300, Height = 200 };
                    ImageResult imageResult = imageUpload.RenameUploadFile(ImageName, pictureName);

                    foodToUpdate.ImageName = pictureName + ".png";
                   // db.Entry(food).State = EntityState.Modified;
                    db.SaveChanges();


                }
                PopulateAssignedCategories(foodToUpdate);
                return RedirectToAction("Details", new { id = id });
            }

            PopulateAssignedCategories(foodToUpdate);
            return View(foodToUpdate);
        }




        private void UpdateFoodCategory(string[] selectedCategorey, Food foodToUpdate)
        {
            if (selectedCategorey == null)
            {
                foodToUpdate.Categories = new List<Category>();
                return;
            }

            var selectedCategoryHS = new HashSet<string>(selectedCategorey);
            var foodCategory = new HashSet<int>(foodToUpdate.Categories.Select(i => i.CategoryID));

            //Loop all categories in database
            foreach (var cat in db.Categories)
            {
                //Add a new category to food 
                if (selectedCategoryHS.Contains(cat.CategoryID.ToString()))
                {
                    if (!foodCategory.Contains(cat.CategoryID))
                    {
                        foodToUpdate.Categories.Add(cat);
                    }
                }
                else
                {

                    //Remove existing category to food
                    if (foodCategory.Contains(cat.CategoryID))
                    {
                        foodToUpdate.Categories.Remove(cat);
                    }
                }//end of if 


            }//end of foreach


        }


        // GET: Food/Delete/5
        [Authorize(Roles = "admin, owner")]
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
            if (User.IsInRole("owner") && (food.Restaurant.UserID != currentUserID()))
            {
                return RedirectToAction("Details", new { id = id });
            }
            return View(food);
        }

        // POST: Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin, owner")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Food food = db.Foods.Find(id);
            if (User.IsInRole("owner") && (food.Restaurant.UserID != currentUserID()))
            {
                return RedirectToAction("Details", new { id = id });
            }

            db.Foods.Remove(food);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

      [Authorize]
        public ActionResult Favorite(int? id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var viewModel = new FavoriteData();
            viewModel.Users = db.Users.Include(i => i.Foods).Where(i => i.Email == currentUser.Email);

            var user = viewModel.Users.Where(i => i.Email == currentUser.Email).Single();

            viewModel.Foods = viewModel.Users.Where(i => i.UserID == user.UserID).Single().Foods;


            //return View(viewModel);
            //jkhalack: the view expects a list of Foods - let's provide it
            return View(viewModel.Foods);
        }

        [HttpPost, ActionName("Favorite")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFavorite(int? FoodID)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var viewModel = new FavoriteData();
            viewModel.Users = db.Users.Include(i => i.Foods).Where(i => i.Email == currentUser.Email);

            var user = viewModel.Users.Where(i => i.Email == currentUser.Email).Single();

            viewModel.Foods = viewModel.Users.Where(i => i.UserID == user.UserID).Single().Foods;
            

            var fav = db.Foods.Find(FoodID);

            var removals = fav.Users.Where(i => i.UserID == user.UserID).SingleOrDefault();

            //var removals = fav.Users.SingleOrDefault();

            fav.Users.Remove(removals);

            db.SaveChanges();

            return RedirectToAction("Favorite");
        }



        [AllowAnonymous]
        public ActionResult FoodsByCategory(string name)
        {
            Category category = db.Categories.Where(i => i.CategoryName == name).SingleOrDefault();
            //var foods = db.Foods.Where(i => i.FoodID == category.CategoryID);

            //jkhalack: eager loading of restaurant information
            db.Entry(category).Collection(x => x.Foods).Load();
            foreach (Food food in category.Foods)
            {
                db.Entry(food).Reference(x => x.Restaurant).Load();
            }
            var foods = category.Foods;



            //var categories = db.Categories.Include(f => f.Foods).Where(f => f.CategoryName == name).SingleOrDefault();
            //var foods = categories.Foods;


            ViewBag.name = name;

            return View(foods.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResults(string searchstring)
        {
            //first find the categories that contain that string
            var categories = db.Categories.Where(i => i.CategoryName.Contains(searchstring));
            //var foods = db.Foods.Where(i => i.FoodID == category.CategoryID);




            var foodsByCategory = new List<Food>();
            foreach (var category in categories)
            {
                db.Entry(category).Collection(x => x.Foods).Load();
                foreach (Food food in category.Foods)
                {
                    db.Entry(food).Reference(x => x.Restaurant).Load();
                }
                foodsByCategory = foodsByCategory.Concat(category.Foods.ToList()).ToList();
            }
                 



            ViewBag.name = searchstring;



            //jkhalack: now find the foods that contain the string in the Food nam or in description

            var foodsByName = db.Foods.Where(i => i.Name.Contains(searchstring) || i.Description.Contains(searchstring))
                .OrderBy(i=>i.AverageRating).ToList();

            var foods = new List<Food>(foodsByName.Count + foodsByCategory.Count);
            foods.AddRange(foodsByName);
            foods.AddRange(foodsByCategory);



            return View(foods);
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
                if (user != null)
                {
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