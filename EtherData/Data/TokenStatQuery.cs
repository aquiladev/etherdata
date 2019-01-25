using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EtherData.Data
{
    public class TokenStatQuery
    {
        private readonly BigQueryClient _client;

        public TokenStatQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<TokenStat> Get30()
        {
            return Get(30);
        }

        public IEnumerable<TokenStat> Get365()
        {
            return Get(365);
        }

        public IEnumerable<TokenStat> GetAll()
        {
            return Get(0);
        }

        public IEnumerable<TokenStat> Get(int period)
        {
            string query = @"SELECT tt.token_address, t.name, COUNT(*) as transfer_count
                FROM `bigquery-public-data.ethereum_blockchain.token_transfers` as tt
                LEFT JOIN `bigquery-public-data.ethereum_blockchain.tokens` as t ON tt.token_address = t.address
                {0}
                GROUP BY tt.token_address, t.name
                ORDER BY 3 DESC
                LIMIT 1000";
            string where = "";
            if (period != 0)
            {
                where = $"WHERE DATE(tt.block_timestamp) > DATE_ADD(current_date(), INTERVAL -{period} DAY)";
            }
            query = string.Format(query, where);

            var result = _client.ExecuteQuery(query, parameters: null);

            var list = new List<TokenStat>();
            foreach (var row in result)
            {
                list.Add(new TokenStat
                {
                    Address = (string)row["token_address"],
                    Name = (string)row["name"],
                    TransferCount = (long)row["transfer_count"],
                });
            }
            return list;
        }
    }

    public class TokenStat
    {
        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("tf_c")]
        public long TransferCount { get; set; }
    }
}
