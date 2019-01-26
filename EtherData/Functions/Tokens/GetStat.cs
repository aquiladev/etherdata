using System.Net;
using System.Net.Http;
using EtherData.Cache;
using EtherData.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Tokens
{
    public static class GetStat
    {
        [FunctionName("GetTokenStat")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tokens/stat")]HttpRequestMessage req,
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

            var result = cache.Get(CacheKey.TOKEN_STAT, query.GetAll);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
