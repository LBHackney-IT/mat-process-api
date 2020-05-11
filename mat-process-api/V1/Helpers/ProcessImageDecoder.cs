using mat_process_api.V1.Domain;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using mat_process_api.V1.Exceptions;

namespace mat_process_api.V1.Helpers
{
    public class ProcessImageDecoder : IProcessImageDecoder
    {
        //check for valid 'image/' part as well
        private static readonly List<string> allowedFileTypes = new List<string>() { "image/jpeg", "image/png", "image/bmp", "image/gif" };

        public Base64DecodedData DecodeBase64ImageString(string imageString)
        {
            try
            {
                string base64Part = imageString.Split(",")[1];
                string fileTypePart = Regex.Match(imageString, @"(?<=:).+(?=;)").Value;
                string fileExt = Regex.Match(fileTypePart, @"(?<=\/).+").Value;

                //do additional validation here since it's too heavy to run against the boundary object

                if (string.IsNullOrWhiteSpace(base64Part))
                {
                    throw new ProcessImageDecoderException("Missing base64 content");
                }
                if (!allowedFileTypes.Contains(fileTypePart))
                {
                    throw new ProcessImageDecoderException("Invalid image type");
                }

                return new Base64DecodedData()
                {
                    imagebase64String = base64Part,
                    imageType = fileTypePart,
                    imageExtension = fileExt
                };
            }
            catch (Exception ex)
            {
                throw ex is ProcessImageDecoderException ? ex : new ProcessImageDecoderException("Unable to parse base64 string");
            }
        }
    }
}
