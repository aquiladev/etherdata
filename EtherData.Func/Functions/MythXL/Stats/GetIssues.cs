using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using EtherData.Data.Entities;
using EtherData.Func.ViewModels;

namespace EtherData.Func.MythXL.Contract
{
    public static class GetIssues
    {
        [FunctionName("MythXL_Stats_GetIssuesStat")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mythxl/v0.1/stats/issues")] HttpRequest req,
            [Table("%Storage:StatTable%", Connection = "Storage:Connection")] CloudTable table,
            ILogger log)
        {
            var partKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "IssueStat");
            var query = new TableQuery<StatEntity> { FilterString = partKey };
            var segment = await table.ExecuteQuerySegmentedAsync(query, null);

            var model = new List<IssueStatModel>();
            foreach (var entry in segment.Results)
            {
                model.Add(new IssueStatModel
                {
                    Key = entry.RowKey,
                    Value = entry.Count
                });
            }

            return new OkObjectResult(model);
        }
    }
}
