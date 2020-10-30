using System.Configuration;

namespace WWTMVC5
{
    /// <summary>
    /// Delegates the configuration of the Redis Cache to use another app setting
    /// This centralizes our redis connection string and allows use in session offloading
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// This method is called by the Microsoft Redis Session provider, 
        /// and is only referenced by name in the Web.Config sessionState block,
        /// do not remove as long as it is referenced therein
        /// </summary>
        /// <returns>The shared Redis connection string</returns>
        public static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["RedisConnectionString"];
        }
    }
}