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
using FoodPicker.ViewModels;

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

            ViewBag.cat = new SelectList(db.Categories, "CategoryName");

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

        //jyoung added check box
        // GET: Food/Create
        public ActionResult Create()
        {
             ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name");

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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FoodID,Name,Description,RestaurantID,Price")] Food food, HttpPostedFileBase ImageName, string[] selectedCategory)
        {

            if(selectedCategory != null)
            {
                food.Categories = new List<Category>();
                foreach(var cat in selectedCategory)
                {
                    var categoryToAdd = db.Categories.Find(int.Parse(cat));
                    food.Categories.Add(categoryToAdd);
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
            ViewBag.ResturantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);

            PopulateAssignedCategories(food);
            
            return View(food);
        }

        //Jyoung added controller to receive the categroies
        private void PopulateAssignedCategories(Food food)
        {
            var allCategories = db.Categories;

            var foodCategories = new HashSet<int>(food.Categories.Select(c => c.CategoryID));

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
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", food.RestaurantID);
            return View(food);
        }

        // POST: Food/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCategory)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var foodToUpdate = db.Foods
                .Include(i => i.Categories)
                .Where(i => i.FoodID == id).Single();

            if (TryUpdateModel(foodToUpdate, "",
                new string[] { "Name", "Description", "Price" }))
            {
                try
                {
                    UpdateFoodCategory(selectedCategory, foodToUpdate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again later!");
                }
            }
            //Irregardless of the outcome (success or fail) we return the student model
            //with edit view or Index view
            PopulateAssignedCategories(foodToUpdate);
            return View("Details", "Food", new {id = id });
        }

        private void UpdateFoodCategory(string[] selectedCategorey, Food foodToUpdate)
        {
            if(selectedCategorey ==null)
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
