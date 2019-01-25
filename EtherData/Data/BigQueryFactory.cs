using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using System;

namespace EtherData.Data
{
    internal class BigQueryFactory
    {
        private static string GCPCredential = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION:CREDENTIALS");

        public static BigQueryClient Create(IConfigurationRoot config)
        {
            var credential = string.IsNullOrEmpty(GCPCredential) ?
                GoogleCredential.FromFile(config["GOOGLE_APPLICATION:CREDENTIALS"]) :
                GoogleCredential.FromJson(GCPCredential);

            return BigQueryClient.Create(config["GOOGLE_APPLICATION:PROJECT_ID"], credential);
        }
    }
}
