using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client.Services
{
    public class CacheService
    {
        public const string NickNames = "NickNames";
        public const string LocalNickNames = "LocalNickNames";
        public const string UpdateLogs = "UpdateLogs";
        public int? LastUnitPromotionLevel { get; set; }

        private Dictionary<string, object> cache = new Dictionary<string, object>();

        public T Get<T>(string key, T defaultValue = default)
        {
            if (cache.TryGetValue(key, out var obj) && obj is T value)
                return value;
            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
            if (cache.ContainsKey(key))
                cache[key] = value;
            else
                cache.Add(key, value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (cache.TryGetValue(key, out var obj) && obj is T v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }
    }
}
