using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight.WEBAPP.Services.TOOLS
{
    public class CachingHelper : ICachingHelper
    {
        // Injection de dependences
        private readonly IMemoryCache _memoryCache;
        public CachingHelper(IMemoryCache memoryCache) =>
            _memoryCache = memoryCache;


        public async Task<T> GetSetAsyncList<T>(string pKey, T pClass)
        {
            //GET CACHE IF EXIST
            var output = _memoryCache.Get<T>(pKey);
            if (output is not null) return output;
            if (output is null && pClass is null) return pClass;

            await Task.Delay(3000);

            _memoryCache.Set(pKey, pClass, TimeSpan.FromMinutes(10));
            return pClass;
        }
    }
}
