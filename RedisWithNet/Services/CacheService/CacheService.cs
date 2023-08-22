using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisWithNet.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private IDatabase db;

        public CacheService()
        {
            ConfigureRedis();
        }

        private void ConfigureRedis()
        {
            db = ConnectionHelper.Connection.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public object RemoveData(string key)
        {
            bool isKeyExist = db.KeyExists(key);
            if (isKeyExist == true)
            {
                return db.KeyDelete(key);
            }
            return false;
        }
    }
}
