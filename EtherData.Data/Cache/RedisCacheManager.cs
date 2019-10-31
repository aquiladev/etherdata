using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;

namespace EtherData.Data.Cache
{
    public class RedisCacheManager
    {
        private readonly IDatabase _db;
        private readonly int _liveTime;

        public RedisCacheManager(string connectionString, int liveTiem)
        {
            _db = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
            _liveTime = liveTiem;
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


        public T Get<T>(string key)
        {
            return Get<T>(key, null);
        }

        public T Get<T>(string key, Func<T> get)
        {
            var rValue = _db.StringGet(key);
            if (rValue.HasValue)
            {
                return Deserialize<T>(rValue);
            }

            if (get != null)
            {
                var result = get();
                Set(key, result);
                return result;
            }

            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            var entryBytes = Serialize(value);
            var expiresIn = TimeSpan.FromMinutes(_liveTime);
            _db.StringSet(key, entryBytes, expiresIn);
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
