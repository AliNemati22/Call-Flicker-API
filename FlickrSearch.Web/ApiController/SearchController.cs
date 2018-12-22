using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FlickrSearch.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;

namespace FlickrSearch.Web.ApiController
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        public readonly int pageSize = 21;
        private readonly string api_key = "4ccde5e7b4278b8f47d3d429109ab0e1";
        private readonly IDistributedCache _distributedCache;
        private readonly bool EnableCaching = true;
        private readonly int CacheSeconds = 120;

        public SearchController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpPost("getImageDetail")]
        public IActionResult getImageDetail([FromBody]ImageDetailRequest meta)
        {
            
            var imageDetailResponse = new ImageDetailResponse();
            if (string.IsNullOrWhiteSpace(meta.ImageId))
            {
                imageDetailResponse.HasError = true;
                imageDetailResponse.ErrorMessage = "imageId must be supplied.";
                var errorResponse = new ObjectResult(imageDetailResponse);
                errorResponse.StatusCode = 200;
                return errorResponse;
            }
            
            var cacheKey = GetMD5Hash(meta.ImageId);

            string cacheResult = "";
            if (EnableCaching)
                cacheResult = _distributedCache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheResult))
            {
                ProceesDetailResponse(cacheResult, imageDetailResponse);
            }
            else
            {

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.flickr.com/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var querySuffix = makeDetailQuerySuffic(meta);

                    HttpResponseMessage flickrResponse = client.GetAsync(querySuffix).Result;
                    if (flickrResponse.IsSuccessStatusCode)
                    {

                        string jsonResult = flickrResponse.Content.ReadAsStringAsync().Result;

                        ProceesDetailResponse(jsonResult, imageDetailResponse);
                        if (!imageDetailResponse.HasError)
                        {

                            if (EnableCaching)
                            {
                                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(CacheSeconds));
                                _distributedCache.SetString(cacheKey, jsonResult, options);
                            }
                        }

                    }
                    else
                    {
                        imageDetailResponse.HasError = true;
                        imageDetailResponse.ErrorMessage = $"request Failed with statusCode: { flickrResponse.StatusCode.ToString() }, reason: { flickrResponse.ReasonPhrase }";
                    }
                }
            }

            var response = new ObjectResult(imageDetailResponse);
            response.StatusCode = 200;
            return response;

        }

        private void ProceesDetailResponse(string jsonResult, ImageDetailResponse imageDetailResponse)
        {
            var resultJObject = JObject.Parse(jsonResult);
            var result = resultJObject.ToObject<FlickrDetailResponse>();
            if (result.stat == "ok")
            {
                imageDetailResponse.HasError = false;
                imageDetailResponse.ErrorMessage = null;
                imageDetailResponse.Owner = result.photo.owner.username;
                imageDetailResponse.Title = result.photo.title._content;
                imageDetailResponse.Tags = result.photo.tags.tag.Select(a=>a._content).ToArray();
                imageDetailResponse.Urls = result.photo.urls.url.Select(a=>a._content).ToArray();
                imageDetailResponse.ThumbnailUrl = result.photo.thumbnailUrl;

            }
            else
            {
                imageDetailResponse.HasError = true;
                imageDetailResponse.ErrorMessage = $"request Failed with code: { (result.code.HasValue ? result.code.Value.ToString() : "") }, message: { result.message }";
            }
            
        }

        private string makeDetailQuerySuffic(ImageDetailRequest meta)
        {
            var baseQuery = $"services/rest/?method=flickr.photos.getInfo&api_key={this.api_key}";
            baseQuery += "&format=json&nojsoncallback=1";
            baseQuery += $"&photo_id={meta.ImageId}";
                
            return baseQuery;
        }


        [HttpPost("findImages")]
        public IActionResult findImages([FromBody]SearchRequestData meta)
        {
            if (!meta.Page.HasValue)
                meta.Page = 1;


            var searchResponse = new SearchResultResponse();
            if (string.IsNullOrWhiteSpace(meta.Keywords))
            {
                if (!meta.Lat.HasValue || !meta.Lon.HasValue)
                {
                    searchResponse.HasError = true;
                    searchResponse.ErrorMessage = "at Least on of (Keywords/Gelocation info) must be supplied.";
                    var errorResponse = new ObjectResult(searchResponse);
                    errorResponse.StatusCode = 200;
                    return errorResponse;
                }
            }


            var querySuffix = makeQuerySuffic(meta);
            var cacheKey = GetMD5Hash(querySuffix);

            string cacheResult = "";
            if (EnableCaching)
                cacheResult = _distributedCache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cacheResult))
            {
                ProceesQueryResponse(cacheResult, searchResponse);
            }
            else
            {

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.flickr.com/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage flickrResponse = client.GetAsync(querySuffix).Result;
                    if (flickrResponse.IsSuccessStatusCode)
                    {

                        string jsonResult = flickrResponse.Content.ReadAsStringAsync().Result;

                        ProceesQueryResponse(jsonResult, searchResponse);
                        if (!searchResponse.HasError)
                        {

                            if (EnableCaching)
                            {
                                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(CacheSeconds));
                                _distributedCache.SetString(cacheKey, jsonResult, options);
                            }
                        }

                    }
                    else
                    {
                        searchResponse.HasError = true;
                        searchResponse.ErrorMessage = $"request Failed with statusCode: { flickrResponse.StatusCode.ToString() }, reason: { flickrResponse.ReasonPhrase }";
                    }
                }
            }

            var response = new ObjectResult(searchResponse);
            response.StatusCode = 200;
            return response;

        }

        private void ProceesQueryResponse(string jsonResult, SearchResultResponse searchResponse)
        {
            var resultJObject = JObject.Parse(jsonResult);
            FlickrSearchResult result = resultJObject.ToObject<FlickrSearchResult>();
            if (result.stat == "ok")
            {
                searchResponse.HasError = false;
                searchResponse.ErrorMessage = null;
                searchResponse.Photos = result.photos.photo;
                searchResponse.Pages = result.photos.pages;
                searchResponse.PerPage = result.photos.perpage;
                searchResponse.Total = result.photos.total;


            }
            else
            {
                searchResponse.HasError = true;
                searchResponse.ErrorMessage = $"request Failed with code: { (result.code.HasValue ? result.code.Value.ToString() : "") }, message: { result.message }";

            }
        }

        private string makeQuerySuffic(SearchRequestData meta)
        {
            var baseQuery = $"services/rest/?method=flickr.photos.search&api_key={this.api_key}";
            baseQuery += $"&per_page={this.pageSize.ToString()}";
            baseQuery += "&format=json&nojsoncallback=1";

            if (meta.Page.HasValue)
                baseQuery += $"&page={meta.Page.ToString()}";

            if (!string.IsNullOrWhiteSpace(meta.Keywords))
                baseQuery += $"&tags={meta.Keywords}";

            if (meta.Lat.HasValue)
                baseQuery += $"&lat={meta.Lat.Value.ToString()}";

            if (meta.Lon.HasValue)
                baseQuery += $"&lon={meta.Lon.Value.ToString()}";

            return baseQuery;
        }

        private string GetMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

}