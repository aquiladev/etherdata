using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCK_STAT = "BLOCK_STAT";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BLOCK_STAT
            };
        }
    }
}
