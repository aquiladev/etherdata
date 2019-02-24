using EtherData.Models;
using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class TokenUsageQuery
    {
        private readonly BigQueryClient _client;

        public TokenUsageQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<TokenUsage> Get(TokenStatFilter filter)
        {
            string query = @"SELECT tt.token_address, t.name, COUNT(*) as transfer_count
                FROM `bigquery-public-data.ethereum_blockchain.token_transfers` as tt
                LEFT JOIN `bigquery-public-data.ethereum_blockchain.tokens` as t ON tt.token_address = t.address
                {0}
                GROUP BY tt.token_address, t.name
                ORDER BY 3 DESC
                LIMIT 1000";
            string where = "";
            if (filter != TokenStatFilter.Default)
            {
                where = $"WHERE DATE(tt.block_timestamp) > DATE_ADD(current_date(), INTERVAL -{(int)filter} DAY)";
            }
            query = string.Format(query, where);

            return _client.ExecuteQuery(query, parameters: null)
                .Select(x => new TokenUsage
                {
                    Address = (string)x["token_address"],
                    Name = (string)x["name"],
                    TransferCount = (long)x["transfer_count"],
                });
        }
    }

    public class TokenUsage
    {
        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("tf_c")]
        public long TransferCount { get; set; }
    }
}
