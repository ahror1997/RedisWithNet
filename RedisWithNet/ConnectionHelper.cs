using StackExchange.Redis;

namespace RedisWithNet
{
    public class ConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        static ConnectionHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisUrl"]);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get { return lazyConnection.Value; }
        }
    }
}