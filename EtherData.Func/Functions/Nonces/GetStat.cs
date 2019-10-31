using System.Net;
using System.Net.Http;
using EtherData.Data.Cache;
using EtherData.Data.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Nonces
{
    public static class GetStat
    {
        [FunctionName("Nonces_GetBlockNonceStat")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/nonces/stat")]HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var queryClient = BigQueryFactory.Create(config["GOOGLE_APPLICATION:CREDENTIALS"], config["GOOGLE_APPLICATION:PROJECT_ID"]);
            var query = new BlockNonceStatQuery(queryClient);
            var cache = new RedisCacheManager(config["REDIS:CONNECTION_STRING"], int.Parse(config["REDIS:LIVE_TIME"]));

            var result = cache.Get(CacheKey.BLOCK_NONCE_STAT, query.Get);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
