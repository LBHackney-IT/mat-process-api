using mat_process_api.V1.Domain;
using System;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public interface IProcessImageDecoder
    {
        Task<Base64DecodedData> DecodeBase64ImageString(string imageString);
    }
}
