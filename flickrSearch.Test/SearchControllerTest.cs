using FlickrSearch.Web.ApiController;
using FlickrSearch.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace flickrSearch.Test
{
    [TestFixture]
    public class SearchControllerTest
    {
        private readonly SearchController _searchController;
        public SearchControllerTest()
        {
            var mockCache = new MockCache();
            _searchController = new SearchController(mockCache);

        }


        [Test]
        public void findImages_Test1()
        {
            var searchRequest = new SearchRequestData();
            searchRequest.Keywords = "Flower";
            searchRequest.Page = 1;
            var result = _searchController.findImages(searchRequest) as ObjectResult;
            var responseData = result.Value as SearchResultResponse;

            Assert.AreEqual(responseData.HasError, false);

           
        }

        [Test]
        public void findImages_Test2()
        {
            var searchRequest = new SearchRequestData();
            searchRequest.Keywords = "Car";
            searchRequest.Page = 1;
            var result = _searchController.findImages(searchRequest) as ObjectResult;
            var responseData = result.Value as SearchResultResponse;
            
            Assert.AreEqual(responseData.PerPage, _searchController.pageSize);

        }

        [Test]
        public void getImageDetail_Test1()
        {
            var imageDetailRequest = new ImageDetailRequest();
            imageDetailRequest.ImageId = "32524464838";
            var result = _searchController.getImageDetail(imageDetailRequest) as ObjectResult;
            var responseData = result.Value as ImageDetailResponse;

            Assert.AreEqual(responseData.Owner.ToLower(), "swanson.matt");

        }
    }
    
}
