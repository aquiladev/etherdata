using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using EtherData.Cache;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Contracts
{
    public static class GetObsolescence
    {
        [FunctionName("GetObsolescence")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/contracts/obsolescence")] HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cache = new RedisCacheManager(config);
            var result = cache.Get<IEnumerable<int>>(CacheKey.CONTRACT_OBSOLESCENCE);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
