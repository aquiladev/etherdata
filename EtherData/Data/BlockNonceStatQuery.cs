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

            var result = _client.ExecuteQuery(query, parameters: null)
                .Select(x => new BlockNonceStat
                {
                    Date = (DateTime)x["d"],
                    Nonces = ((IList<string>)x["n"]).Select(n => Convert.ToUInt64(n, 16))
                });
            return result;
        }

        private IEnumerable<BlockNonceStat> ToModel(BigQueryResults result)
        {
            var list = new List<BlockNonceStat>((int)result.TotalRows);
            foreach (var row in result)
            {
                list.Add(new BlockNonceStat
                {
                    Date = (DateTime)row["date"],
                    Nonces = (string)row["nonces"]
                });
            }
            return list;
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
