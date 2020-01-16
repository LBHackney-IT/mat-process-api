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
        public Base64DecodedData DecodeBase64ImageString(string imageString)
        {
            string base64Part = imageString.Split(",")[1];
            byte[] base64Bytes = Convert.FromBase64String(base64Part);
            string fileTypePart = Regex.Match(imageString, @"(?<=:).+(?=;)").Value;
            string fileExt = Regex.Match(fileTypePart, @"(?<=\/).+").Value;

            return new Base64DecodedData()
            {
                imageBytes = base64Bytes,
                imageType = fileTypePart,
                imageExtension = fileExt
            };
        }
    }
}
