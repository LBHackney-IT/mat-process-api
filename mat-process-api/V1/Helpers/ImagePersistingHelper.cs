using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public class ImagePersistingHelper
    {
        public static async Task<string> generateImageKey(string processType, string imageId, string processRef, string fileExtension)
        {
            return await Task.FromResult($"{processType}/{processRef}/{imageId}.{fileExtension}");
        }
    }
}
