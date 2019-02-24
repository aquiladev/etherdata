using EtherData.Models;
using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class BlockStatQuery
    {
        private readonly BigQueryClient _client;

        public BlockStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<BlockStat> Get(BlockStatFilter filter)
        {
            string query = @"SELECT
                    TIMESTAMP_TRUNC(timestamp, {0}) as date,
                    COUNT(*) as count,
                    CAST(AVG(difficulty) / 1000000000000 AS FLOAT64) as avg_difficulty,
                    SUM(size) as size,
                    AVG(gas_limit) as avg_gas_limit,
                    SUM(gas_used) as gas_used,
                    SUM(transaction_count) as tx_count
                FROM `bigquery-public-data.ethereum_blockchain.blocks`
                {1}
                GROUP BY date
                ORDER BY date";
            string where = "";
            if (filter != BlockStatFilter.Default)
            {
                where = $"WHERE DATE(timestamp) > DATE_ADD(current_date(), INTERVAL -{(int)filter} DAY)";
            }
            var accuracy = filter == BlockStatFilter.Month ? "HOUR" : "DAY";
            query = string.Format(query, accuracy, where);

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new BlockStat
                {
                    Date = (DateTime)x["date"],
                    Count = (long)x["count"],
                    AvgDifficulty = (double)x["avg_difficulty"],
                    Size = (long)x["size"],
                    AvgGasLimit = (double)x["avg_gas_limit"],
                    GasUsed = (long)x["gas_used"],
                    TxCount = (long)x["tx_count"],
                });
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
