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

            string popular = "SELECT TOP 10 FoodID, Name, ImageName, AverageRating From Food WHERE AverageRating > 3";
            IEnumerable<ViewModels.FoodIndexData> dataP =
                    db.Database.SqlQuery<ViewModels.FoodIndexData>(popular);
            ViewBag.popular = dataP.ToList();


            string recentlyadded = "SELECT TOP 10 FoodID, Name, ImageName, AverageRating From Food WHERE DATEPART(m, DateAdded) = DATEPART(m, DATEADD(m, -1, getdate())) AND DATEPART(yyyy, DateAdded) = DATEPART(yyyy, DATEADD(m, -1, getdate()))";
            IEnumerable<ViewModels.FoodIndexData> dataA =
                    db.Database.SqlQuery<ViewModels.FoodIndexData>(recentlyadded);
            ViewBag.recentlyadded = dataA.ToList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}