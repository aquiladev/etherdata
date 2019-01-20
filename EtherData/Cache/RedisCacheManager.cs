using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;

namespace EtherData.Cache
{
    public class RedisCacheManager
    {
        private readonly IDatabase _db;
        private readonly int _liveTime;

        public RedisCacheManager(IConfigurationRoot config)
        {
            _db = ConnectionMultiplexer.Connect(config["REDIS:CONNECTION_STRING"]).GetDatabase();
            _liveTime = int.Parse(config["REDIS:LIVE_TIME"]);
        }

        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public T Get<T>(string key, Func<T> getData)
        {
            var rValue = _db.StringGet(key);
            if (rValue.HasValue)
            {
                return Deserialize<T>(rValue);
            }

            var result = getData();
            var entryBytes = Serialize(result);
            var expiresIn = TimeSpan.FromMinutes(_liveTime);
            _db.StringSet(key, entryBytes, expiresIn);

            return result;
        }

        public void CleanUp()
        {
            foreach (var key in CacheKey.GetAll())
            {
                if (_db.KeyExists(key))
                {
                    _db.KeyDelete(key);
                }
            }
        }
    }
}
