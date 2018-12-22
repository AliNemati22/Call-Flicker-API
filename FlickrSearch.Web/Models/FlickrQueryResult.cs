using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{

    public class FlickrSearchResult
    {
        public FlickrSearchPhotos photos { get; set; }

        public string stat { get; set; }

        public int? code { get; set; }

        public string message { get; set; }

    }

    public class FlickrSearchPhotos
    {
        /*"photos": { "page": 1, "pages": "7604", "perpage": 100, "total": "760321", photo:[] }*/
        public decimal page { get; set; }
        public decimal pages { get; set; }
        public int perpage { get; set; }
        public decimal total { get; set; }
        public List<FlickrSearchPhotoItem> photo { get; set; }

        public FlickrSearchPhotos()
        {
            photo = new List<FlickrSearchPhotoItem>();
        }
    }

    public class FlickrSearchPhotoItem
    {
        /*
        { "id": "45477771255", "owner": "127273037@N02", "secret": "a13c97980b", "server": "4823", "farm": 5, "title": "UNICEF Livery", "ispublic": 1, "isfriend": 0, "isfamily": 0 },
        */
        public string id { get; set; }
        public string owner { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public string farm { get; set; }
        public string title { get; set; }

        public string OrginalUrl
        {
            get
            {
                return $"https://farm{farm}.staticflickr.com/{server}/{id}_{secret}_b.jpg";
            }
        }

        public string ThumbnailUrl
        {
            get
            {
                return $"https://farm{farm}.staticflickr.com/{server}/{id}_{secret}_q.jpg";
            }
        }
    }
}
