using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using mat_process_api.V1.Exceptions;

namespace mat_process_api.V1.Infrastructure
{
    public class AwsS3Client : IAmazonS3Client
    {
        public PutObjectResponse insertImage(AWSCredentials credentials, string base64, string key, string contentType)
        {
            byte[] data;
            var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest2);

            try
            {
                data = Convert.FromBase64String(base64);
            }
            catch (Exception) //ArgumentNull or Format exception
            {
                throw new Base64StringConversionToByteArrayException();
            }

            try
            {
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
            catch (Exception)
            {   
                throw new ImageNotInsertedToS3();
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
