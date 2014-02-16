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
            return View();
        }

        //
        // GET: /Marvel/GetComics
        public ActionResult GetComics()
        {
            return View(new GetComicsViewModel());
        }

        //
        // POST: /Marvel/GetComics
        [HttpPost]
        public ActionResult GetComics(GetComicsViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComics(Format: model.Format, FormatType: model.FormatType, NoVariants: model.NoVariants, DateDescript: model.Descriptor, HasDigitalIssue: model.HasDigitalIssue, Order: model.Order, Limit: model.Limit, Offset: model.Offset);
            model.ResultComics = comics;
            return View(model);
        }

        //
        // GET: /Marvel/GetComic
        public ActionResult GetComic(int id)
        {
            var model = new GetComicViewModel();
            model.Comic = _Marvel.GetComic(id);
            return View(model);
        }


        //
        // GET: /Marvel/GetComicsForCharacter
        public ActionResult GetComicsForCharacter(int id)
        {
            IEnumerable<Comic> comics = _Marvel.GetComicsForCharacter(id);
            var model = new GetComicsForCharacterViewModel();
            model.CharacterId = id;
            model.ResultComics = comics;
            return View(model);
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
        // GET: /Marvel/GetComicsForCreator
        public ActionResult GetComicsForCreator(int id)
        {
            IEnumerable<Comic> comics = _Marvel.GetComicsForCreator(id);
            var model = new GetComicsForCreatorViewModel();
            model.CreatorId = id;
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