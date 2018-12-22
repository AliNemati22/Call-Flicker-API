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

    public class MockCache : IDistributedCache
    {
        Dictionary<string, byte[]> mockCache = new Dictionary<string, byte[]>();
        public byte[] Get(string key)
        {
            return mockCache.ContainsKey(key) ? mockCache[key] : null;
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (mockCache.ContainsKey(key))
                mockCache.Remove(key);

            mockCache.Add(key, value);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
