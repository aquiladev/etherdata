using EtherData.Cache;
using EtherData.Data;
using EtherData.Models;
using EtherData.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Miners
{
    public static class GetStat
    {
        [FunctionName("GetMinerStat")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/miners/stat")]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var query = new MinerStatQuery(BigQueryFactory.Create(config));
            var cache = new RedisCacheManager(config);

            var filter = Enum<MinerStatFilter>.Parse(req.Query["filter"]);
            var result = cache.Get(filter.ToKey(CacheKey.MINER_STAT), () => query.Get(filter));
            return new OkObjectResult(result);
        }
    }
}
