using MarvelAPI.Sample.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace MarvelAPI.Sample.Controllers
{
    public class APIController : Controller
    {
        private string _MarvelPublicKey { get; set; }
        private string _MarvelPrivateKey { get; set; }
        private Marvel _Marvel { get; set; }

        public APIController()
        {
            _MarvelPublicKey = ConfigurationManager.AppSettings["MarvelPublicApiKey"];
            _MarvelPrivateKey = ConfigurationManager.AppSettings["MarvelPrivateApiKey"];

            _Marvel = new Marvel(_MarvelPublicKey, _MarvelPrivateKey, true);
        }

        //[HttpGet]
        //public object Comics()
        //{
        //    IEnumerable<Comic> comics = _Marvel.GetComics();
        //    IEnumerable<SlimComic> slimComics = comics.Select(comic => new SlimComic { Id = comic.Id, Description = comic.Description, DiamondCode = comic.DiamondCode, DigitalId = comic.DigitalId });
        //    return new { comics: JsonConvert.SerializeObject(slimComics, ) };
        //}

        [HttpPost]
        public JsonResult Comics(GetComicsViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComics(format: model.Format, formatType: model.FormatType, noVariants: model.NoVariants, dateDescript: model.Descriptor, hasDigitalIssue: model.HasDigitalIssue, order: model.Order, limit: model.Limit, offset: model.Offset);
            return Json(comics);
        }

        [HttpPost]
        public JsonResult Comic(int id)
        {
            var comic = _Marvel.GetComic(id);
            return Json(comic);
        }

        //
        // POST: /Marvel/GetComics
        [HttpPost]
        public JsonResult GetComics(GetComicsViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComics(format: model.Format, formatType: model.FormatType, noVariants: model.NoVariants, dateDescript: model.Descriptor, hasDigitalIssue: model.HasDigitalIssue, order: model.Order, limit: model.Limit, offset: model.Offset);
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
            IEnumerable<Comic> comics = _Marvel.GetComicsForCharacter(characterId: model.CharacterId, format: model.Format, formatType: model.FormatType, noVariants: model.NoVariants, dateDescript: model.Descriptor, hasDigitalIssue: model.HasDigitalIssue, order: model.Order, limit: model.Limit, offset: model.Offset);
            model.ResultComics = comics;
            return View(model);
        }

        //
        // POST: /Marvel/GetComicsForCreator
        [HttpPost]
        public ActionResult GetComicsForCreator(GetComicsForCreatorViewModel model)
        {
            IEnumerable<Comic> comics = _Marvel.GetComicsForCreator(creatorId: model.CreatorId, format: model.Format, formatType: model.FormatType, noVariants: model.NoVariants, dateDescript: model.Descriptor, hasDigitalIssue: model.HasDigitalIssue, order: model.Order, limit: model.Limit, offset: model.Offset);
            model.ResultComics = comics;
            return View(model);
        }
    }
}