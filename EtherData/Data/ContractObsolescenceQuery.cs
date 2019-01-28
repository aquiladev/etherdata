using Google.Cloud.BigQuery.V2;
using System.Collections.Generic;
using System.Linq;

namespace EtherData.Data
{
    public class ContractObsolescenceQuery
    {
        private readonly BigQueryClient _client;

        public ContractObsolescenceQuery(BigQueryClient client)
        {
            _client = client;
        }

        public IEnumerable<int> Get()
        {
            string query = @"SELECT DIV(TIMESTAMP_DIFF(MAX(t.block_timestamp), c.block_timestamp, HOUR), 168) AS d
                FROM `bigquery-public-data.ethereum_blockchain.contracts` AS c
                LEFT JOIN `bigquery-public-data.ethereum_blockchain.transactions` AS t ON c.address = t.to_address
                WHERE c.block_timestamp < t.block_timestamp
                GROUP BY c.address, c.block_timestamp
                ORDER BY 1 DESC";

            var result = _client.ExecuteQuery(query, parameters: null).ToList();
            return Enumerable.Range(0, (int)(long)result[0][0])
                .Select(x => result.Count(y => (long)y[0] > x));
        }
    }
}
