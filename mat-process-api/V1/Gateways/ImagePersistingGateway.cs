using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Helpers;
using mat_process_api.V1.Infrastructure;

namespace mat_process_api.V1.Gateways
{
    public class ImagePersistingGateway : IImagePersistingGateway
    {
        IAmazonS3Client s3Client;
        IAwsAssumeRoleHelper assumeRoleHelper;
        public ImagePersistingGateway(IAmazonS3Client _s3Client, IAwsAssumeRoleHelper _assumeRoleHelper)
        {
            s3Client = _s3Client;
            assumeRoleHelper = _assumeRoleHelper;
        }

        public void UploadImage(ProcessImageData request)
        {   
            //insert image
            var result = s3Client.insertImage(assumeRoleHelper.GetTemporaryCredentials(), request.imageData.imagebase64String.ToString(), request.key,
                request.imageData.imageType);

            if (result.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new ImageNotInsertedToS3();
            }
        }

        public string RetrieveImage(string imageKey)
        {
            try
            {
                var result = s3Client.retrieveImage(assumeRoleHelper.GetTemporaryCredentials(), imageKey,
                    Environment.GetEnvironmentVariable("bucket-name"));
                return ImageRetrievalFactory.EncodeStreamToBase64(result);
            }
            catch(AggregateException ex)
            {
                if(ex.InnerException != null && ex.InnerException.Message == "The specified key does not exist.")
                {
                    throw new ImageNotFound(); //if image not found
                }
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

