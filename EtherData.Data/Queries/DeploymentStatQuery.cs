using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data.Queries
{
    public class DeploymentStatQuery
    {
        private readonly BigQueryClient _client;

        public DeploymentStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<DeploymentStat> Get()
        {
            string query = @"SELECT TIMESTAMP_TRUNC(block_timestamp, DAY) AS d, COUNT(*) AS c
                FROM `bigquery-public-data.ethereum_blockchain.contracts`
                GROUP BY 1
                ORDER BY 1";

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new DeploymentStat
                {
                    Date = (DateTime)x["d"],
                    Count = (long)x["c"],
                });
        }
    }

    public class DeploymentStat
    {
        [JsonProperty("d")]
        public DateTime Date { get; set; }

        [JsonProperty("c")]
        public long Count { get; set; }
    }
}
