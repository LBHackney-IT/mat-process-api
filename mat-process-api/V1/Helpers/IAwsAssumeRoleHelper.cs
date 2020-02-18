using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public interface IAwsAssumeRoleHelper
    {
        Task<Credentials> GetTemporaryCredentials();
    }
}
