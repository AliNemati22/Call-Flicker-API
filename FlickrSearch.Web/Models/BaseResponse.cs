using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrSearch.Web.Models
{
    public class BaseResponse
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
