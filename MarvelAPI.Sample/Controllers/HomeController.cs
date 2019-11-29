using MarvelAPI.Sample.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace MarvelAPI.Sample.Controllers
{
    public class HomeController : Controller
    {
        private string _MarvelPublicKey { get; set; }
        private string _MarvelPrivateKey { get; set; }
        private Marvel _Marvel { get; set; }

        public HomeController()
        {
            _MarvelPublicKey = ConfigurationManager.AppSettings["MarvelPublicApiKey"];
            _MarvelPrivateKey = ConfigurationManager.AppSettings["MarvelPrivateApiKey"];

            _Marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey, true);
        }

        public ActionResult Index()
        {
            var model = new MarvelViewModel();

            model.MarvelMethods = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "(none)" },
                new SelectListItem { Text = "GetComics", Value = Url.Action("GetComics") },
                new SelectListItem { Text = "GetComic", Value = Url.Action("GetComic") }
            };

            model.BooleanSelectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "(none)" },
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };

            IEnumerable<ComicFormat> comicFormats = Enum.GetValues(typeof(ComicFormat)).Cast<ComicFormat>();
            model.ComicFormatSelectList = comicFormats.Select(format => new SelectListItem { Text = format.ToParameter(), Value = format.ToString(), Selected = false }).ToList();
            model.ComicFormatSelectList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = true });

            IEnumerable<ComicFormatType> comicFormatTypes = Enum.GetValues(typeof(ComicFormatType)).Cast<ComicFormatType>();
            model.ComicFormatTypeSelectList = comicFormatTypes.Select(format => new SelectListItem { Text = format.ToParameter(), Value = format.ToString(), Selected = false }).ToList();
            model.ComicFormatTypeSelectList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = true });

            IEnumerable<DateDescriptor> dateDescriptors = Enum.GetValues(typeof(DateDescriptor)).Cast<DateDescriptor>();
            model.DateDescriptorsSelectList = dateDescriptors.Select(format => new SelectListItem { Text = format.ToParameter(), Value = format.ToString(), Selected = false }).ToList();
            model.DateDescriptorsSelectList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = true });

            IEnumerable<OrderBy> orderByOptions = Enum.GetValues(typeof(OrderBy)).Cast<OrderBy>();
            model.OrderByOptionsSelectList = orderByOptions.Select(format => new SelectListItem { Text = format.ToParameter(), Value = format.ToString(), Selected = false }).ToList();
            model.OrderByOptionsSelectList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = true });

            return View(model);
        }

        public ActionResult TestComics()
        {
            var characters = new List<int>();
            characters.Add(1011334);
            characters.Add(1017100);

            var comics = _Marvel.GetComics(sharedAppearances: characters);

            return View();
        }
    }
}