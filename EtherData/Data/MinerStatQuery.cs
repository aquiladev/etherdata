using EtherData.Models;
using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class MinerStatQuery
    {
        private readonly BigQueryClient _client;

        public MinerStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<MinerStat> Get(MinerStatFilter filter)
        {
            string query = @"SELECT miner, COUNT(*) as count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                {0}
                GROUP BY miner
                ORDER BY 2 DESC
                LIMIT 1000";
            string where = "";
            if (filter != MinerStatFilter.Default)
            {
                where = $"WHERE DATE(timestamp) > DATE_ADD(current_date(), INTERVAL -{(int)filter} DAY)";
            }
            query = string.Format(query, where);

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new MinerStat
                {
                    Address = (string)x["miner"],
                    Blocks = (long)x["count"],
                });
        }
    }

    public class MinerStat
    {
        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("b")]
        public long Blocks { get; set; }
    }
}
