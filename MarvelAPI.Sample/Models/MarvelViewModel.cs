using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarvelAPI.Sample.Models
{
    public class MarvelViewModel
    {
        public List<SelectListItem> ComicFormatSelectList { get; set; }

        public List<SelectListItem> ComicFormatTypeSelectList { get; set; }

        public List<SelectListItem> DateDescriptorsSelectList { get; set; }

        public List<SelectListItem> OrderByOptionsSelectList { get; set; }
    }
}