using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using mat_process_api.V1.Infrastructure;

namespace mat_process_api.V1.Helpers
{
    public class AwsAssumeRoleHelper : IAwsAssumeRoleHelper
    {
        private IAmazonSTSClient client;
        public AwsAssumeRoleHelper(IAmazonSTSClient _client)
        {
            client = _client;
        }
        public async Task<Credentials> GetTemporaryCredentials()
        {
            BasicAWSCredentials creds = new BasicAWSCredentials(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"));
            try
            {
                var stsClient = client.getStsClient(creds);
                using (stsClient)
                {
                    var assumeRoleTokenRequest = new AssumeRoleRequest
                    {
                        DurationSeconds = 3600, // seconds
                        RoleArn = Environment.GetEnvironmentVariable("S3_ASSUME_ROLE_ARN"), //get from env var
                        RoleSessionName = "Session"
                    };
                    AssumeRoleResponse assumeRoleTokenResponse = await client.assumeRole(stsClient,assumeRoleTokenRequest);
                    Credentials credentials = assumeRoleTokenResponse.Credentials;
                    return credentials;
                }
            }
            catch(Exception ex)
            {
               throw ex;
            }           
        }
    }
}
