using mat_process_api.V1.Domain;
using System;
namespace mat_process_api.V1.Helpers
{
    public interface IProcessImageDecoder
    {
        Base64DecodedData DecodeBase64ImageString(string imageString);
    }
}
