using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Factories
{
    public class ImageRetrievalFactory
    {
        public static string EncodeStreamToBase64(GetObjectResponse s3Response)
        {
            if (s3Response.ResponseStream != null)
            {
                using (Stream responseStream = s3Response.ResponseStream)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        responseStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    return $"data:{s3Response.Headers.ContentType};base64,"+Convert.ToBase64String(bytes);
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
