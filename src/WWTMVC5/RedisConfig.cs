using System.Configuration;

namespace WWTMVC5
{
    /// <summary>
    /// Delegates the configuration of the Redis Cache to use another app setting
    /// This centralizes our redis connection string and allows use in session offloading
    /// </summary>
    public class RedisConfig
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["RedisConnectionString"];
        }
    }
}