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
            string query = @"WITH ContractWeeks AS ( 
                  SELECT Week, COUNT(*) as _contractsSum
                  FROM (
                    SELECT DIV(TIMESTAMP_DIFF(MAX(t.block_timestamp), c.block_timestamp, HOUR), 168) as Week
                    FROM `bigquery-public-data.ethereum_blockchain.contracts` AS c
                    LEFT JOIN `bigquery-public-data.ethereum_blockchain.transactions` AS t ON c.address = t.to_address
                    WHERE c.block_timestamp < t.block_timestamp
                    GROUP BY c.address, c.block_timestamp) z 
                  GROUP BY week
                ) 

                SELECT SUM(_contractsSum)
                FROM UNNEST(GENERATE_ARRAY(0, (SELECT MAX(Week) From ContractWeeks) )) AS Week
                CROSS JOIN ContractWeeks c
                WHERE Week <= c.Week
                GROUP BY Week
                ORDER BY Week";

            return _client.ExecuteQuery(query, parameters: null).Select(x => (int)(long)x[0]);
        }
    }
}
