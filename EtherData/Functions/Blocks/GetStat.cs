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
using System.Collections.Generic;

namespace EtherData.Functions.Blocks
{
    public static class GetStat
    {
        [FunctionName("GetBlockStat")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/blocks/stat")]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cache = new RedisCacheManager(config);
            var filter = Enum<BlockStatFilter>.Parse(req.Query["filter"]);
            var result = cache.Get<IEnumerable<BlockStat>>(filter.ToKey(CacheKey.BLOCK_STAT));
            return new OkObjectResult(result);
        }
    }
}
