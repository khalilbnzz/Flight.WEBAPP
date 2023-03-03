using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight.WEBAPP.Services.TOOLS
{
    public interface ICachingHelper
    {
        Task<T> SetAsyncList<T>(string pKey, T pClass);
        Task<T> GetSetAsyncList<T>(string pKey, T pClass);
        Task<T> GetRemoveSetAsyncList<T>(string pKey, T pClass);
        Task<string> GetSetAsyncListString<String>(string pKey, string pJson);
        Task<T> GetCache<T>(string pKey);

        Task<Dictionary<int, T>> GetSetAsyncDictionnary<T>(string pKey, Dictionary<int, T> keyValues);

        void SetValue<T>(string pKey, T pValue);

        T GetValue<T>(string pKey);
        T GetValueLike<T>(string pValue);
        void Remove(string pKey);
    }
}
