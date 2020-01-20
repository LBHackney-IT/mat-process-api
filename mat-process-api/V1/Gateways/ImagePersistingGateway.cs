using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            try
            {
                //insert image
                var result = s3Client.insertImage(assumeRoleHelper.GetTemporaryCredentials(), request.imageData.imagebase64String.ToString(), request.key,
                    request.imageData.imageType);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new ImageNotInsertedToS3();
                }
            }
            catch(Exception ex)
            {
                throw new ImageNotInsertedToS3(); //500
            }
        }

        public string RetrieveImage()
        {
            try
            {
                var result = s3Client.retrieveImage(assumeRoleHelper.GetTemporaryCredentials(), "", "");
                if(result.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new ImageNotFound();
                }
                return ImageRetrievalFactory.EncodeStreamToBase64(result);
            }
            catch(AggregateException ex)
            {
                if(ex.Message == "The specified key does not exist.")
                {
                    throw new ImageNotFound();
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

