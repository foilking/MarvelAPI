using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarvelAPI;
using System.Configuration;
using MarvelAPI.Sample.Models;

namespace MarvelAPI.Sample.Controllers
{
    public class HomeController : Controller
    {
        private string _MarvelPublicKey { get; set; }
        private string _MarvelPrivateKey { get; set; }

        public HomeController()
        {
            _MarvelPublicKey = ConfigurationManager.AppSettings["MarvelPublicApiKey"];
            _MarvelPrivateKey = ConfigurationManager.AppSettings["MarvelPrivateApiKey"];
        }

        public ActionResult Index()
        {
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

        [HttpGet]
        public ActionResult MarvelTesting()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetComic(int comicId)
        {
            var marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey);

            Comic comic = marvel.GetComic(comicId);
            ViewBag.Comic = comic;
            return View("MarvelTesting");
        }
    }
}