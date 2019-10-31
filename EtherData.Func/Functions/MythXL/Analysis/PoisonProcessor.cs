using EtherData.Func.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EtherData.Func.MythXL.Analysis
{
    public static class PoisonProcessor
    {
        [FunctionName("MythXL_Analysis_PoisonProcessor")]
        public static async Task Run(
            [QueueTrigger("%Storage:AnalysisPoisonQueue%", Connection = "Storage:Connection")] AnalysisMessage message,
            [Queue("%Storage:AnalysisQueue%", Connection = "Storage:Connection")] CloudQueue contractQueue,
            ILogger log)
        {
            string msg = JsonConvert.SerializeObject(message);
            var visibilityDelay = TimeSpan.FromHours(1);
            await contractQueue.AddMessageAsync(new CloudQueueMessage(msg), null, visibilityDelay, null, null);
        }
    }
}
