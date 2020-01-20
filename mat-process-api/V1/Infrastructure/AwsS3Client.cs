using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace mat_process_api.V1.Infrastructure
{
    public class AwsS3Client : IAmazonS3Client
    {
      
        public PutObjectResponse insertImage(AWSCredentials credentials, string base64, string key,string contentType)
        {
          
            try
            {
                var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest2);
            
                byte[] data = Convert.FromBase64String(base64);
                using (var stream = new MemoryStream(data))
                {
                    var putRequest1 = new PutObjectRequest
                    {
                        BucketName = Environment.GetEnvironmentVariable("bucket-name"),
                        Key = key, //file path in S3 to be included here
                        InputStream = stream,
                        ContentType = contentType
                    };
                    PutObjectResponse response = s3Client.PutObjectAsync(putRequest1).Result;
                    return response;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }     
        }

        public GetObjectResponse retrieveImage(AWSCredentials credentials, string key, string bucketName)
        {
            try
            {
                using (var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest2))
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = key
                    };

                    GetObjectResponse response = s3Client.GetObjectAsync(request).Result;
                    return response;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
