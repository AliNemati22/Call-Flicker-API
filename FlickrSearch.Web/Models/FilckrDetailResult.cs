using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{
    public class FlickrDetailResponse
    {
        public FlickrDetailResult photo { get; set; }

        public string stat { get; set; }

        public int? code { get; set; }

        public string message { get; set; }
    }

    public class FlickrDetailResult
    {
        public string id { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public string farm { get; set; }
        public ImageDateData dates { get; set; }
        public ImageTitleData title { get; set; }
        public ImageTagsData tags { get; set; }
        public ImageUrlsData urls { get; set; }
        public ImageOwnerData owner { get; set; }


        public string thumbnailUrl
        {
            get
            {
                return $"https://farm{farm}.staticflickr.com/{server}/{id}_{secret}_q.jpg";
            }
        }
    }

    public class ImageDateData
    {
        public string taken { get; set; }

    }

    public class ImageTitleData
    {
        public string _content { get; set; }

    }

    public class ImageTagsData
    {
        public List<ImageTagDetail> tag { get; set; }
        public ImageTagsData()
        {
            tag = new List<ImageTagDetail>();
        }
    }

    public class ImageTagDetail
    {
        public string _content { get; set; }
    }

    public class ImageOwnerData
    {
        public string username { get; set; }
    }

    public class ImageUrlsData
    {
        public List<ImageUrlDetail> url { get; set; }
        public ImageUrlsData()
        {
            url = new List<ImageUrlDetail>();
        }
    }

    public class ImageUrlDetail
    {
        public string _content { get; set; }
        public string type { get; set; }
        
    }
}
