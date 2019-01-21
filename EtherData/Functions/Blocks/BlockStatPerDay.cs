using System.Net;
using System.Net.Http;
using EtherData.Cache;
using EtherData.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.BlocksPerDay
{
    public static class BlockStatPerDay
    {
        [FunctionName("BlockStatPerDayGet")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "blocks/statPerDay")]HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var query = new BlockStatPerDayQuery(config);
            var cache = new RedisCacheManager(config);

            var result = cache.Get(CacheKey.BLOCK_STAT_PER_DAY, query.Get);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
