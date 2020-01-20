using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public class ImagePersistingHelper
    {
        public static string generateImageKey(string processType, string imageId, string processRef, string fileExtension)
        {
            return $"{processType}/{processRef}/{imageId}.{fileExtension}";
        }
    }
}
