using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using System;

namespace EtherData.Data.Queries
{
    public class BigQueryFactory
    {
        private static string GCPCredential = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION:CREDENTIALS");

        public static BigQueryClient Create(string credentialFile, string projectId)
        {
            var credential = string.IsNullOrEmpty(GCPCredential) ?
                GoogleCredential.FromFile(credentialFile) :
                GoogleCredential.FromJson(GCPCredential);

            return BigQueryClient.Create(projectId, credential);
        }
    }
}
