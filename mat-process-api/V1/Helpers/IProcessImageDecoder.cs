using System;
namespace mat_process_api.V1.Helpers
{
    public interface IProcessImageDecoder
    {
        byte[] DecodeBase64ImageString(string imageString);
    }
}
