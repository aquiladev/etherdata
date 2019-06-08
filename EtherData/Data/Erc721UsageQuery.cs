using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class Erc721UsageQuery
    {
        private readonly BigQueryClient _client;

        public Erc721UsageQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<Erc721Usage> Get()
        {
            string query = @"SELECT TIMESTAMP_TRUNC(tt.block_timestamp, MONTH) AS d, tt.token_address AS a, COUNT(*) as tf_c
                FROM `bigquery-public-data.ethereum_blockchain.token_transfers` as tt
                LEFT JOIN `bigquery-public-data.ethereum_blockchain.contracts` as c ON tt.token_address = c.address
                WHERE c.is_erc721 = true
                GROUP BY 1, 2
                ORDER BY 1";

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new Erc721Usage
                {
                    Date = (DateTime)x["d"],
                    Address = (string)x["a"],
                    Count = (long)x["tf_c"],
                });
        }
    }

    public class Erc721Usage
    {
        [JsonProperty("d")]
        public DateTime Date { get; set; }

        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("c")]
        public long Count { get; set; }
    }
}
