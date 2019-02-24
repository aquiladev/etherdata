using EtherData.Models;
using EtherData.Utils;
using System.Collections.Generic;

namespace EtherData.Cache
{
    public class CacheKey
    {
        public const string BLOCK_STAT = "BLOCK_STAT_{0}";
        public const string BLOCK_NONCE_STAT = "BLOCK_NONCE_STAT";
        public const string TOKEN_USAGE = "TOKEN_USAGE_{0}";
        public const string MINER_STAT = "MINER_STAT_{0}";
        public const string CONTRACT_OBSOLESCENCE = "CONTRACT_OBSOLESCENCE";
        public const string CONTRACT_DEPLOYMENT_STAT = "CONTRACT_DEPLOYMENT_STAT";

        public static IEnumerable<string> GetAll()
        {
            return new List<string>
            {
                BlockStatFilter.Default.ToKey(BLOCK_STAT),
                BlockStatFilter.Month.ToKey(BLOCK_STAT),
                BLOCK_NONCE_STAT,
                TokenStatFilter.Default.ToKey(TOKEN_USAGE),
                TokenStatFilter.Month.ToKey(TOKEN_USAGE),
                TokenStatFilter.Year.ToKey(TOKEN_USAGE),
                MinerStatFilter.Default.ToKey(MINER_STAT),
                MinerStatFilter.Month.ToKey(MINER_STAT),
                MinerStatFilter.Year.ToKey(MINER_STAT),
                CONTRACT_OBSOLESCENCE,
                CONTRACT_DEPLOYMENT_STAT
            };
        }
    }
}
