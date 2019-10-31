using EtherData.Data;
using EtherData.Func.Models;
using EtherData.Func.MythX;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.Web3;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EtherData.Func.MythXL.Contract
{
    public static class Processor
    {
        [FunctionName("MythXL_Contract_Processor")]
        public static async Task Run(
            [QueueTrigger("%Storage:ContractQueue%", Connection = "Storage:Connection")] ContractMessage message,
            [Queue("%Storage:AnalysisQueue%", Connection = "Storage:Connection")] CloudQueue analysisQueue,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var web3 = new Web3(config.GetValue<string>("Blockchain:Endpoint"));
            var tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(message.TxHash);
            var code = tx.Input;
            if (string.IsNullOrEmpty(code) || code == "0x")
            {
                return;
            }

            System.Threading.Thread.Sleep(2000);
            var policy = new AnalysisExecutionPolicy(config, log);
            var analysis = await policy.AnalyzeAsync(code);
            var response = JsonConvert.DeserializeObject<AnalysisResponse>(analysis);

            await Blob.WriteAsync(
                config.GetValue<string>("Storage:Connection"),
                config.GetValue<string>("Storage:ContractInputContainer"),
                message.Address,
                code);

            string msg = JsonConvert.SerializeObject(new AnalysisMessage
            {
                Address = message.Address,
                TxHash = message.TxHash,
                Account = policy.Account,
                AnalysisId = response.UUID,
                Version = 2
            });
            await analysisQueue.AddMessageAsync(new CloudQueueMessage(msg));
        }
    }
}
