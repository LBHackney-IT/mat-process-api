using mat_process_api.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public class ProcessImageDecoder : IProcessImageDecoder
    {
        public async Task<Base64DecodedData> DecodeBase64ImageString(string imageString)
        {
            Base64DecodedData data = Decode(imageString);

            return await Task.FromResult(data);
            
        }

        private Base64DecodedData Decode(string imageString)
        {
            string base64Part = imageString.Split(",")[1];
            string fileTypePart = Regex.Match(imageString, @"(?<=:).+(?=;)").Value;
            string fileExt = Regex.Match(fileTypePart, @"(?<=\/).+").Value;

            return new Base64DecodedData()
            {
                imagebase64String = base64Part,
                imageType = fileTypePart,
                imageExtension = fileExt
            };
        }
    }
}
