using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.Helpers;
using mat_process_api.V1.Infrastructure;

namespace mat_process_api.V1.Gateways
{
    public class ImagePersistingGateway
    {
        IAmazonS3Client s3Client;
        IAwsAssumeRoleHelper assumeRoleHelper;
        public ImagePersistingGateway(IAmazonS3Client _s3Client, IAwsAssumeRoleHelper _assumeRoleHelper)
        {
            s3Client = _s3Client;
            assumeRoleHelper = _assumeRoleHelper;
        }

        public void UploadImage()
        {
            try
            {
                //insert image
                var result = s3Client.insertImage(assumeRoleHelper.GetTemporaryCredentials(), "", "");
                if (result.HttpStatusCode != HttpStatusCode.NoContent)
                {
                    throw new ImageNotInsertedToS3();
                }
            }
            catch(Exception ex)
            {
                throw new ImageNotInsertedToS3(); //500
            }
        }

        public void RetrieveImage()
        {
            try
            {
                var result = s3Client.retrieveImage(assumeRoleHelper.GetTemporaryCredentials(), "", "");
                if(result.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new ImageNotFound();
                }
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

