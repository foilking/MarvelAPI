using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace MarvelAPI.Sample.Models
{
    public class MarvelViewModel
    {
        [DisplayName("Public API Key")]
        public string MarvelPublicAPIKey { get; set; }
        [DisplayName("Private API Key")]
        public string MarvelPrivateAPIKey { get; set; }

        public List<SelectListItem> MarvelMethods { get; set; }
        public List<SelectListItem> BooleanSelectList { get; set; }
        public List<SelectListItem> ComicFormatSelectList { get; set; }
        public List<SelectListItem> ComicFormatTypeSelectList { get; set; }
        public List<SelectListItem> DateDescriptorsSelectList { get; set; }
        public List<SelectListItem> OrderByOptionsSelectList { get; set; }

        public GetComicsViewModel GetComics { get; set; }
    }
}