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
        public AssumeRoleResponse assumeRole(AmazonSecurityTokenServiceClient client,AssumeRoleRequest request)
        {
            return client.AssumeRoleAsync(request).Result;
        }
    }
}
