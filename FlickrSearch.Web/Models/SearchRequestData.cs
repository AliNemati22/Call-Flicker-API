using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{
    public class SearchRequestData
    {
        public int? Page { get; set; }
        public string Keywords { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
    }
}
