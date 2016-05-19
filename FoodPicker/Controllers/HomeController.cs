using FoodPicker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodPicker.Controllers
{
    public class HomeController : Controller
    {
        private FoodContext db = new FoodContext();

        public ActionResult Index()
        {

            //string popular = "SELECT TOP 10 FoodID, Name, ImageName, AverageRating From Food WHERE AverageRating > 3";
            string popular = "SELECT TOP 4 FoodID, Name, ImageName, AverageRating From Food ORDER BY AverageRating DESC";
            IEnumerable<ViewModels.FoodIndexData> dataP =
                    db.Database.SqlQuery<ViewModels.FoodIndexData>(popular);
            ViewBag.popular = dataP.ToList();


            //string recentlyadded = "SELECT TOP 10 FoodID, Name, ImageName, AverageRating From Food WHERE DATEPART(m, DateAdded) = DATEPART(m, DATEADD(m, -1, getdate())) AND DATEPART(yyyy, DateAdded) = DATEPART(yyyy, DATEADD(m, -1, getdate()))";
            string recentlyadded = "SELECT TOP 4 FoodID, Name, ImageName, AverageRating From Food ORDER BY DateAdded DESC";
            IEnumerable<ViewModels.FoodIndexData> dataA =
                    db.Database.SqlQuery<ViewModels.FoodIndexData>(recentlyadded);
            ViewBag.recentlyadded = dataA.ToList();

            ViewBag.restaurants = db.Restaurants.ToList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "FoodPicker Description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //jkhalack: Ajax search methods
        private List<Models.Category> GetCategory(string searchString)
        {
            return db.Categories
                .Where(i => i.CategoryName.Contains(searchString)).ToList();
        }

        public ActionResult CategorySearch(string q)
        {
            //get search results
            var category = GetCategory(q);
            return PartialView("_CategorySearch", category);
        }

        //this is the AJAX - autocomplete
        public ActionResult QuickSearch(string term)
        {
            var category = GetCategory(term)
                .Select(a => new { value = a.CategoryName });
            return Json(category, JsonRequestBehavior.AllowGet);
        }


        //jkhalack: end Ajax search methods
    }
}