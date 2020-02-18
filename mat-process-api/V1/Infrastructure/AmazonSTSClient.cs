using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace mat_process_api.V1.Infrastructure
{
    public class AmazonSTSClient : IAmazonSTSClient
    {
        public AmazonSecurityTokenServiceClient getStsClient(BasicAWSCredentials credentials)
        {
            return new AmazonSecurityTokenServiceClient(credentials, Amazon.RegionEndpoint.EUWest2);
        }
        public async Task<AssumeRoleResponse> assumeRole(AmazonSecurityTokenServiceClient client,AssumeRoleRequest request)
        {
            return await client.AssumeRoleAsync(request);
        }
    }
}
