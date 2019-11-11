using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

namespace EtherData.Data.Queries
{
    public class BigQueryFactory
    {
        public static BigQueryClient Create(string credentialFile, string projectId)
        {
            var credential = GoogleCredential.FromFile(credentialFile);
            return BigQueryClient.Create(projectId, credential);
        }
    }
}
