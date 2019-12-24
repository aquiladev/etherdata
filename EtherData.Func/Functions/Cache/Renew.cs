using System;
using System.Collections.Generic;
using EtherData.Data.Cache;
using EtherData.Data.Models;
using EtherData.Data.Queries;
using EtherData.Data.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Cache
{
    public static class Renew
    {
        [FunctionName("Cache_Renew")]
        public static void Run(
            [TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo timer,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"Renew function executed at: {DateTime.Now}");

            var cache = new RedisCacheManager(config["REDIS:CONNECTION_STRING"], int.Parse(config["REDIS:LIVE_TIME"]));
            var mapping = GetMapping(config);

            foreach (var k in mapping.Keys)
            {
                log.LogInformation($"Start renew cache for {k}");
                cache.Set(k, mapping[k]());
            }
        }

        private static Dictionary<string, Func<object>> GetMapping(IConfigurationRoot config)
        {
            var client = BigQueryFactory.Create(config["GOOGLE_APPLICATION:CREDENTIALS"], config["GOOGLE_APPLICATION:PROJECT_ID"]);

            return new Dictionary<string, Func<object>> {
                {
                    BlockStatFilter.Default.ToKey(CacheKey.BLOCK_STAT),
                    () => {
                        var q = new BlockStatQuery(client);
                        return q.Get(BlockStatFilter.Default);
                    }
                },
                {
                    BlockStatFilter.Month.ToKey(CacheKey.BLOCK_STAT),
                    () => {
                        var q = new BlockStatQuery(client);
                        return q.Get(BlockStatFilter.Month);
                    }
                },
                {
                    CacheKey.CONTRACT_DEPLOYMENT_STAT.ToString(),
                    () => {
                        var q = new DeploymentStatQuery(client);
                        return q.Get();
                    }
                },
                {
                    CacheKey.CONTRACT_OBSOLESCENCE.ToString(),
                    () => {
                        var q = new ContractObsolescenceQuery(client);
                        return q.Get();
                    }
                },
                {
                    MinerStatFilter.Default.ToKey(CacheKey.MINER_STAT),
                    () => {
                        var q = new MinerStatQuery(client);
                        return q.Get(MinerStatFilter.Default);
                    }
                },
                {
                    MinerStatFilter.Month.ToKey(CacheKey.MINER_STAT),
                    () => {
                        var q = new MinerStatQuery(client);
                        return q.Get(MinerStatFilter.Month);
                    }
                },
                {
                    MinerStatFilter.Year.ToKey(CacheKey.MINER_STAT),
                    () => {
                        var q = new MinerStatQuery(client);
                        return q.Get(MinerStatFilter.Year);
                    }
                },
                {
                    TokenStatFilter.Default.ToKey(CacheKey.TOKEN_USAGE),
                    () => {
                        var q = new TokenUsageQuery(client);
                        return q.Get(TokenStatFilter.Default);
                    }
                },
                {
                    TokenStatFilter.Month.ToKey(CacheKey.TOKEN_USAGE),
                    () => {
                        var q = new TokenUsageQuery(client);
                        return q.Get(TokenStatFilter.Month);
                    }
                },
                {
                    TokenStatFilter.Year.ToKey(CacheKey.TOKEN_USAGE),
                    () => {
                        var q = new TokenUsageQuery(client);
                        return q.Get(TokenStatFilter.Year);
                    }
                },
                {
                    CacheKey.ERC721_STAT,
                    () => {
                        var q = new Erc721StatQuery(client);
                        return q.Get();
                    }
                },
                {
                    CacheKey.ERC721_USAGE,
                    () => {
                        var q = new Erc721UsageQuery(client);
                        return q.Get();
                    }
                },
            };
        }
    }
}
