using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Nethereum.Util;
using Microsoft.WindowsAzure.Storage.Table;
using EtherData.Data.Entities;
using System.Linq;
using EtherData.Func.ViewModels;
using Nethereum.ENS;
using Nethereum.Web3;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;

namespace EtherData.Func.Functions.PublicKeys
{
    public static class Lookup
    {
        [FunctionName("PublicKeys_Lookup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v0.1/publickey/lookup")] HttpRequest req,
            [Table("%PublicKey:Storage:PublicKeyTable%", Connection = "PublicKey:Storage:Connection")] CloudTable table,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var queryParam = (string)req.Query["q"];
            var model = new PublicKeyLookupModel();

            if (!string.IsNullOrEmpty(queryParam) &&
                !AddressUtil.Current.IsValidEthereumAddressHexFormat(queryParam))
            {
                var web3 = new Web3(config.GetValue<string>("Blockchain:Endpoint"));
                var nameHash = new EnsUtil().GetNameHash(queryParam);
                var ensRegistryService = new ENSRegistryService(web3, config.GetValue<string>("Blockchain:ENSRegistryService"));
                var resolverAddress = await ensRegistryService.ResolverQueryAsync(
                    new ResolverFunction() { Node = nameHash.HexToByteArray() });
                var resolverService = new PublicResolverService(web3, resolverAddress);
                var address = await resolverService.AddrQueryAsync(nameHash.HexToByteArray());

                model.Ens = new EnsModel
                {
                    Name = queryParam,
                    Hash = nameHash,
                    Resolver = resolverAddress
                };
                model.Address = address;

                queryParam = address;
            }

            if (AddressUtil.Current.IsValidEthereumAddressHexFormat(queryParam))
            {
                var entity = await FindPublicKey(table, queryParam);
                if (entity != null)
                {
                    model.Address = entity.PartitionKey;
                    model.PublicKey = entity.RowKey;
                }
            }

            return string.IsNullOrEmpty(model.Address) && model.Ens == null ?
                new BadRequestObjectResult("Wrong format") :
                (IActionResult)new OkObjectResult(model);
        }

        private static async Task<PublicKeyEntity> FindPublicKey(CloudTable table, string address)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, address);
            var query = new TableQuery<PublicKeyEntity> { TakeCount = 1, FilterString = filter };
            var segment = await table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results.FirstOrDefault();
        }
    }
}
