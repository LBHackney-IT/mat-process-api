using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Exceptions
{
    public class ConflictException : System.Exception
    {
        public ConflictException(String message, Exception inner) : base(message, inner) { }
    }

    public class DocumentNotFound : Exception { }
    public class ImageNotInsertedToS3 : Exception { }
    public class ImageNotFound : Exception { }
    public class ProcessImageDecoderException : Exception
    {
        public ProcessImageDecoderException() : base()
        {
        }
        public ProcessImageDecoderException(string message) : base(message)
        {
        }
    }
    public class Base64StringConversionToByteArrayException : Exception { }

}
