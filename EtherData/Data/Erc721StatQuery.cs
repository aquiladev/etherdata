using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class Erc721StatQuery
    {
        private readonly BigQueryClient _client;

        public Erc721StatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<Erc721Stat> Get()
        {
            string query = @"SELECT TIMESTAMP_TRUNC(block_timestamp, MONTH) AS d, COUNT(*) AS c
                FROM `bigquery-public-data.ethereum_blockchain.contracts`
                WHERE is_erc721 = true
                GROUP BY 1
                ORDER BY 1";

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new Erc721Stat
                {
                    Date = (DateTime)x["d"],
                    Count = (long)x["c"],
                });
        }
    }

    public class Erc721Stat
    {
        [JsonProperty("d")]
        public DateTime Date { get; set; }

        [JsonProperty("c")]
        public long Count { get; set; }
    }
}
