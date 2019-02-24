using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class BlockNonceStatQuery
    {
        private readonly BigQueryClient _client;

        public BlockNonceStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<BlockNonceStat> Get()
        {
            string query = @"SELECT TIMESTAMP_TRUNC(timestamp, DAY) as d, ARRAY_AGG(nonce) as n
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                GROUP BY d
                ORDER BY d
                LIMIT 10";

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new BlockNonceStat
                {
                    Date = (DateTime)x["d"],
                    Nonces = ((IList<string>)x["n"]).Select(n => Convert.ToUInt64(n, 16))
                });
        }
    }

    public class BlockNonceStat
    {
        [JsonProperty("d")]
        public DateTime Date { get; set; }

        [JsonProperty("n")]
        public object Nonces { get; set; }
    }
}
