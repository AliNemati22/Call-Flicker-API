using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{
    public class SearchResultResponse: BaseResponse
    {

        public List<FlickrSearchPhotoItem> Photos { get; set; }

        public decimal Pages { get; set; }
        public int PerPage { get; set; }
        public decimal Total { get; set; }
    }
}
