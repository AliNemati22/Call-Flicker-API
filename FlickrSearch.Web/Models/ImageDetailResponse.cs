using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{
    public class ImageDetailResponse: BaseResponse
    {
        public string[] Dates { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public string[] Urls { get; set; }
        public string Owner { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
