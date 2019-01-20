using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EtherData.Data
{
    public class BlocksPerDayQuery
    {
        private readonly BigQueryClient _client;

        public BlocksPerDayQuery(IConfigurationRoot config)
        {
            _client = BigQueryClient.Create(
                config["GOOGLE_APPLICATION:PROJECT_ID"],
                GoogleCredential.FromFile(config["GOOGLE_APPLICATION:CREDENTIALS"]));
        }

        public IEnumerable<BlcokPerDay> Get()
        {
            string query = @"SELECT EXTRACT(DATE FROM timestamp) as date, COUNT(*) as count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                GROUP BY EXTRACT(DATE FROM timestamp)";
            var result = _client.ExecuteQuery(query, parameters: null);

            var list = new List<BlcokPerDay>();
            foreach (var row in result)
            {
                list.Add(new BlcokPerDay
                {
                    Date = (DateTime)row["date"],
                    Count = (long)row["count"]
                });
            }
            return list;
        }
    }

    public class BlcokPerDay
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}
