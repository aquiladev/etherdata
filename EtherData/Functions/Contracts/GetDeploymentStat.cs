using System.Net;
using System.Net.Http;
using EtherData.Cache;
using EtherData.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EtherData.Functions.Contracts
{
    public static class GetDeploymentStat
    {
        [FunctionName("GetDeploymentStat")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/contracts/deployment")] HttpRequestMessage req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var query = new DeploymentStatQuery(BigQueryFactory.Create(config));
            var cache = new RedisCacheManager(config);

            var result = cache.Get(CacheKey.CONTRACT_DEPLOYMENT_STAT, query.Get);
            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
