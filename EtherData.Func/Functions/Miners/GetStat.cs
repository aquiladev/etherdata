using EtherData.Data.Cache;
using EtherData.Data.Models;
using EtherData.Data.Queries;
using EtherData.Data.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EtherData.Functions.Miners
{
    public static class GetStat
    {
        [FunctionName("Miners_GetMinerStat")]
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

            var cache = new RedisCacheManager(config);
            var filter = Enum<MinerStatFilter>.Parse(req.Query["filter"]);
            var result = cache.Get<IEnumerable<MinerStat>>(filter.ToKey(CacheKey.MINER_STAT));
            return new OkObjectResult(result);
        }
    }
}
