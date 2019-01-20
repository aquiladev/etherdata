using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCKS_PER_DAY = "BLOCKS_PER_DAY";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BLOCKS_PER_DAY
            };
        }
    }
}
