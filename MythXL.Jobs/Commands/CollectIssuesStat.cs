﻿using CommandLine;
using EtherData.Data;
using EtherData.Data.Entities;
using EtherData.Data.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MythXL.Jobs.Commands
{
    [Verb("collect-issues-stat", HelpText = "Collects issues statistics.")]
    public class CollectIssuesStatOptions
    {
        [Option('c', "connection-string", Required = true, HelpText = "Connection string to Azure Storage.")]
        public string Connection { get; set; }

        [Option('t', "contract-table-name", Required = true, HelpText = "Contract table.")]
        public string ContractTableName { get; set; }

        [Option('a', "analysis-blob-container", Required = true, HelpText = "Analysis blob container.")]
        public string AnalysisBlobContainerName { get; set; }

        [Option('s', "stat-table-name", Required = true, HelpText = "Stat table.")]
        public string StatTableName { get; set; }

        [Option('b', "batchSize", Required = false, HelpText = "Batch size.", Default = 100)]
        public int BatchSize { get; set; }
    }

    public class CollectIssuesStat
    {
        public static int RunAddAndReturnExitCode(CollectIssuesStatOptions options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.Connection);
            var tableClient = storageAccount.CreateCloudTableClient();
            var contractTable = tableClient.GetTableReference(options.ContractTableName);

            var counter = 0;
            var stat = new ConcurrentDictionary<string, long>();

            TableContinuationToken token = null;
            do
            {
                var query = new TableQuery<ContractEntity> { TakeCount = options.BatchSize };
                var segment = contractTable.ExecuteQuerySegmentedAsync(query, token).Result;

                Parallel.ForEach(segment.Results, entry =>
                {
                    if (entry.AnalysisStatus != "Finished")
                    {
                        return;
                    }

                    var content = Blob.ReadAsync(options.Connection, options.AnalysisBlobContainerName, entry.AnalysisId).Result;
                    if (string.IsNullOrEmpty(content))
                    {
                        return;
                    }

                    var list = JsonConvert.DeserializeObject<List<AnalysisResult>>(content);
                    foreach (var issue in list.SelectMany(x => x.Issues))
                    {
                        stat.AddOrUpdate(issue.SwcID, 1, (key, oldValue) => oldValue + 1);
                    }
                });

                counter += segment.Results.Count;
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fffffff ")} Handled {counter} records");

                token = segment.ContinuationToken;
            } while (token != null);

            var batchOperation = new TableBatchOperation();
            foreach (var key in stat.Keys)
            {
                var entry = new StatEntity
                {
                    PartitionKey = "IssueStat",
                    RowKey = key,
                    Count = stat[key]
                };
                batchOperation.InsertOrReplace(entry);
            }

            var statTable = tableClient.GetTableReference(options.StatTableName);
            statTable.CreateIfNotExistsAsync().Wait();
            statTable.ExecuteBatchAsync(batchOperation).Wait();

            return 0;
        }
    }
}
