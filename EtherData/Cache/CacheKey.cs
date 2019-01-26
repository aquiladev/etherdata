using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCK_STAT = "BLOCK_STAT";
        public const string BLOCK_STAT_30 = "BLOCK_STAT_30";
        public const string TOKEN_STAT= "TOKEN_STAT";
        public const string TOKEN_STAT_30 = "TOKEN_STAT_30";
        public const string TOKEN_STAT_365 = "TOKEN_STAT_365";
        public const string TOKEN_USAGE = "TOKEN_USAGE";
        public const string TOKEN_USAGE_30 = "TOKEN_USAGE_30";
        public const string TOKEN_USAGE_365 = "TOKEN_USAGE_365";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BLOCK_STAT,
                BLOCK_STAT_30,
                TOKEN_STAT,
                TOKEN_STAT_30,
                TOKEN_STAT_365,
                TOKEN_USAGE,
                TOKEN_USAGE_30,
                TOKEN_USAGE_365
            };
        }
    }
}
