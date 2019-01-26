using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EtherData.Data
{
    public class BlockStatQuery
    {
        private readonly BigQueryClient _client;

        public BlockStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<BlockStat> Get()
        {
            string query = @"SELECT
                    TIMESTAMP_TRUNC(timestamp, DAY) as date,
                    COUNT(*) as count,
                    CAST(AVG(difficulty) / 1000000000000 AS FLOAT64) as avg_difficulty,
                    SUM(size) as size,
                    AVG(gas_limit) as avg_gas_limit,
                    SUM(gas_used) as gas_used,
                    SUM(transaction_count) as tx_count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                GROUP BY date
                ORDER BY date";

            var result = _client.ExecuteQuery(query, parameters: null);
            return ToModel(result);
        }

        public IEnumerable<BlockStat> Get30()
        {
            string query = @"SELECT
                    TIMESTAMP_TRUNC(timestamp, HOUR) as date,
                    COUNT(*) as count,
                    CAST(AVG(difficulty) / 1000000000000 AS FLOAT64) as avg_difficulty,
                    SUM(size) as size,
                    AVG(gas_limit) as avg_gas_limit,
                    SUM(gas_used) as gas_used,
                    SUM(transaction_count) as tx_count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                WHERE DATE(timestamp) > DATE_ADD(current_date(), INTERVAL -30 DAY)
                GROUP BY date
                ORDER BY date";

            var result = _client.ExecuteQuery(query, parameters: null);
            return ToModel(result);
        }

        private IEnumerable<BlockStat> ToModel(BigQueryResults result)
        {
            var list = new List<BlockStat>();
            foreach (var row in result)
            {
                list.Add(new BlockStat
                {
                    Date = (DateTime)row["date"],
                    Count = (long)row["count"],
                    AvgDifficulty = (double)row["avg_difficulty"],
                    Size = (long)row["size"],
                    AvgGasLimit = (double)row["avg_gas_limit"],
                    GasUsed = (long)row["gas_used"],
                    TxCount = (long)row["tx_count"],
                });
            }
            return list;
        }
    }

    public class BlockStat
    {
        [JsonProperty("d")]
        public DateTime Date { get; set; }

        [JsonProperty("c")]
        public long Count { get; set; }

        [JsonProperty("a_d")]
        public double AvgDifficulty { get; set; }

        [JsonProperty("s")]
        public long Size { get; set; }

        [JsonProperty("a_g_l")]
        public double AvgGasLimit { get; set; }

        [JsonProperty("g_u")]
        public long GasUsed { get; set; }

        [JsonProperty("tx_c")]
        public long TxCount { get; set; }
    }
}
