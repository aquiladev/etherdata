using EtherData.Func.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EtherData.Func.MythXL.Block
{
    public static class Processor
    {
        [FunctionName("MythXL_Block_Processor")]
        public static async Task Run(
            [QueueTrigger("%Storage:BlockQueue%", Connection = "Storage:Connection")] BlockMessage message,
            [Queue("%Storage:BlockQueue%", Connection = "Storage:Connection")] CloudQueue blockQueue,
            [Queue("%Storage:ContractQueue%", Connection = "Storage:Connection")] CloudQueue contractQueue,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var web3 = new Web3(config.GetValue<string>("Blockchain:Endpoint"));
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(message.Block));

            if (block.Transactions.Length > 0)
            {
                foreach (var tx in block.Transactions)
                {
                    if (tx.To != null)
                    {
                        continue;
                    }

                    var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(tx.TransactionHash);
                    string cMsg = JsonConvert.SerializeObject(new ContractMessage
                    {
                        Address = receipt.ContractAddress,
                        TxHash = tx.TransactionHash
                    });
                    await contractQueue.AddMessageAsync(new CloudQueueMessage(cMsg));
                }
            }
        }
    }
}
