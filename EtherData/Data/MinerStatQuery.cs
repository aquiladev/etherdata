using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EtherData.Data
{
    public class MinerStatQuery
    {
        private readonly BigQueryClient _client;

        public MinerStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<MinerStat> Get30()
        {
            return Get(30);
        }

        public IEnumerable<MinerStat> Get365()
        {
            return Get(365);
        }

        public IEnumerable<MinerStat> GetAll()
        {
            return Get(0);
        }

        private IEnumerable<MinerStat> Get(int period)
        {
            string query = @"SELECT miner, COUNT(*) as count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                {0}
                GROUP BY miner
                ORDER BY 2 DESC
                LIMIT 1000";
            string where = "";
            if (period != 0)
            {
                where = $"WHERE DATE(timestamp) > DATE_ADD(current_date(), INTERVAL -{period} DAY)";
            }
            query = string.Format(query, where);

            var result = _client.ExecuteQuery(query, parameters: null);
            return ToModel(result);
        }

        private IEnumerable<MinerStat> ToModel(BigQueryResults result)
        {
            var list = new List<MinerStat>();
            foreach (var row in result)
            {
                list.Add(new MinerStat
                {
                    Address = (string)row["miner"],
                    Blocks = (long)row["count"],
                });
            }
            return list;
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
