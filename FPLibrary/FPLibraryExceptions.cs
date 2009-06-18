using System;
using System.Runtime.Serialization;

namespace FPLibrary
{
  class FPLibraryException : Exception
  {
    public static int LibraryInitialisationError = 1;
    public static int LibraryFPCaptureError = 2;
    public static int LibraryDalError = 3;
    public static int LibraryParameterError = 4;
    public FPLibraryException()
    {
      ErrorCode = 0;
    }
    public FPLibraryException(string message)
      : base(message)
    {
      ErrorCode = 0;
    }
    public FPLibraryException(string message, Exception innerException)
      : base(message, innerException)
    {
      ErrorCode = 0;
    }
    protected FPLibraryException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      ErrorCode = 0;
    }
    public FPLibraryException( int errorCode,string message) : base(message)
    {
      ErrorCode = errorCode;
    }
    public int ErrorCode { get; set; }
  }
}
