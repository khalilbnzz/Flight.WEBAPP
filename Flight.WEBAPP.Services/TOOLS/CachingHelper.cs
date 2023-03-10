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
            if (output is null && pClass is not null) return pClass;

            _memoryCache.Set(pKey, pClass, TimeSpan.FromMinutes(10));
            return pClass;
        }

        public async Task<T> SetAsyncList<T>(string pKey, T pClass)
        {
            _memoryCache.Set(pKey, pClass, TimeSpan.FromMinutes(10));
            return pClass;
        }
        public async Task<T> GetRemoveSetAsyncList<T>(string pKey, T pClass)
        {
            _memoryCache.Remove(pKey);
            _memoryCache.Set(pKey, pClass, TimeSpan.FromMinutes(10));
            return pClass;
        }

        public async Task<T> GetCache<T>(string pKey)
        {
            _memoryCache.TryGetValue(pKey, out var output);
            return (T)output;
        }

        public async Task<string> GetSetAsyncListString<String>(string pKey, string pJson)
        {
            //GET CACHE IF EXIST
            var output = _memoryCache.Get<string>(pKey);
            if (output is not null) return output;
            if (output is null && pJson is null) return pJson;

            _memoryCache.Set(pKey, pJson, TimeSpan.FromMinutes(10));
            return pJson;
        }

        public async Task<Dictionary<int, T>> GetSetAsyncDictionnary<T>(string pKey, Dictionary<int, T> keyValues)
        {
            //GET CACHE IF EXIST
            var output = _memoryCache.Get<Dictionary<int, T>>(pKey);
            if (output is not null) return output;
            

            _memoryCache.Set(pKey, keyValues, TimeSpan.FromMinutes(10));
            return keyValues;
        }

        public T GetValue<T>(string pKey)
        {
            _memoryCache.TryGetValue(pKey, out T Value);
            return Value;
        }

        public T GetValueLike<T>(string pValue)
        {
            return default(T);
        }

        public void Remove(string pKey)
        {
            _memoryCache.Remove(pKey);
        }

        public void SetValue<T>(string pKey, T pValue)
        {
            _memoryCache.Set(pKey, pValue, TimeSpan.FromMinutes(20));
        }

      
    }
}
