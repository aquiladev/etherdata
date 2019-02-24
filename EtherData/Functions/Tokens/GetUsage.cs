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

namespace EtherData.Functions.Tokens
{
    public static class GetUsage
    {
        [FunctionName("GetTokenUsage")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/tokens/usage")]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var query = new TokenUsageQuery(BigQueryFactory.Create(config));
            var cache = new RedisCacheManager(config);

            var filter = Enum<TokenStatFilter>.Parse(req.Query["filter"]);
            var result = cache.Get(filter.ToKey(CacheKey.TOKEN_USAGE), () => query.Get(filter));
            return new OkObjectResult(result);
        }
    }
}
