using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCK_STAT = "BLOCK_STAT";
        public const string TOKEN_STAT= "TOKEN_STAT";
        public const string TOKEN_STAT_30 = "TOKEN_STAT_30";
        public const string TOKEN_STAT_365 = "TOKEN_STAT_365";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BLOCK_STAT,
                TOKEN_STAT,
                TOKEN_STAT_30,
                TOKEN_STAT_365
            };
        }
    }
}
