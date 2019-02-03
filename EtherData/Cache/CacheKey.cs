using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCK_STAT = "BLOCK_STAT";
        public const string BLOCK_STAT_30 = "BLOCK_STAT_30";
        public const string BLOCK_NONCE_STAT = "BLOCK_NONCE_STAT";
        public const string TOKEN_USAGE = "TOKEN_USAGE";
        public const string TOKEN_USAGE_30 = "TOKEN_USAGE_30";
        public const string TOKEN_USAGE_365 = "TOKEN_USAGE_365";
        public const string MINER_STAT = "MINER_STAT";
        public const string MINER_STAT_30 = "MINER_STAT_30";
        public const string MINER_STAT_365 = "MINER_STAT_365";
        public const string CONTRACT_OBSOLESCENCE = "CONTRACT_OBSOLESCENCE";
        public const string CONTRACT_DEPLOYMENT_STAT = "CONTRACT_DEPLOYMENT_STAT";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BLOCK_STAT,
                BLOCK_STAT_30,
                BLOCK_NONCE_STAT,
                TOKEN_USAGE,
                TOKEN_USAGE_30,
                TOKEN_USAGE_365,
                MINER_STAT,
                MINER_STAT_30,
                MINER_STAT_365,
                CONTRACT_OBSOLESCENCE,
                CONTRACT_DEPLOYMENT_STAT
            };
        }
    }
}
