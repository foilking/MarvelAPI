using MarvelAPI.Sample.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarvelAPI.Sample.Controllers
{
    public class MarvelController : Controller
    {
        private string _MarvelPublicKey { get; set; }
        private string _MarvelPrivateKey { get; set; }
        private Marvel _Marvel { get; set; }

        public MarvelController()
        {
            _MarvelPublicKey = ConfigurationManager.AppSettings["MarvelPublicApiKey"];
            _MarvelPrivateKey = ConfigurationManager.AppSettings["MarvelPrivateApiKey"];

            _Marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey, true);
        }

        //
        // GET: /Marvel/
        public ActionResult Index()
        {
            var model = new MarvelViewModel();
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

        //
        // POST: /Marvel/GetComics
        [HttpPost]
        public JsonResult GetComics(GetComicsViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComics(Format: model.Format, FormatType: model.FormatType, NoVariants: model.NoVariants, DateDescript: model.Descriptor, HasDigitalIssue: model.HasDigitalIssue, Order: model.Order, Limit: model.Limit, Offset: model.Offset);
            return Json(comics);
        }

        //
        // POST: /Marvel/GetComic
        [HttpPost]
        public JsonResult GetComic(int id)
        {
            var comic = _Marvel.GetComic(id);
            return Json(comic);
        }

        //
        // POST: /Marvel/GetComicsForCharacter
        [HttpPost]
        public ActionResult GetComicsForCharacter(GetComicsForCharacterViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComicsForCharacter(CharacterId: model.CharacterId, Format: model.Format, FormatType: model.FormatType, NoVariants: model.NoVariants, DateDescript: model.Descriptor, HasDigitalIssue: model.HasDigitalIssue, Order: model.Order, Limit: model.Limit, Offset: model.Offset);
            model.ResultComics = comics;
            return View(model);
        }

        //
        // POST: /Marvel/GetComicsForCreator
        [HttpPost]
        public ActionResult GetComicsForCreator(GetComicsForCreatorViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComicsForCreator(CreatorId: model.CreatorId, Format: model.Format, FormatType: model.FormatType, NoVariants: model.NoVariants, DateDescript: model.Descriptor, HasDigitalIssue: model.HasDigitalIssue, Order: model.Order, Limit: model.Limit, Offset: model.Offset);
            model.ResultComics = comics;
            return View(model);
        }
	}
}