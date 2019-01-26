using System.Net;
using System.Net.Http;
using EtherData.Cache;
using EtherData.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Blocks
{
    public static class GetStat30
    {
        [FunctionName("GetBlockStat30")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/blocks/stat30")]HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var query = new BlockStatQuery(BigQueryFactory.Create(config));
            var cache = new RedisCacheManager(config);

            var result = cache.Get(CacheKey.BLOCK_STAT_30, query.Get30);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
