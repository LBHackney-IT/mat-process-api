using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Infrastructure
{
    public interface IAmazonSTSClient
    {
        AmazonSecurityTokenServiceClient getStsClient(BasicAWSCredentials credentials);
        AssumeRoleResponse assumeRole(AmazonSecurityTokenServiceClient client, AssumeRoleRequest request);
    }
}
