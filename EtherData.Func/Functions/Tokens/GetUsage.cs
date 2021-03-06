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

namespace EtherData.Functions.Tokens
{
    public static class GetUsage
    {
        [FunctionName("Tokens_GetTokenUsage")]
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

            var cache = new RedisCacheManager(config["REDIS:CONNECTION_STRING"], int.Parse(config["REDIS:LIVE_TIME"]));
            var filter = Enum<TokenStatFilter>.Parse(req.Query["filter"]);
            var result = cache.Get<IEnumerable<TokenUsage>>(filter.ToKey(CacheKey.TOKEN_USAGE));
            return new OkObjectResult(result);
        }
    }
}
