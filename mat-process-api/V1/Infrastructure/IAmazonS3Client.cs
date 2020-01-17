using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Infrastructure
{
    public interface IAmazonS3Client
    {
        PutObjectResponse insertImage(AWSCredentials credentials, string base64, string key,string contentType);
        GetObjectResponse retrieveImage(AWSCredentials credentials, string key, string bucketName);
    }
}
