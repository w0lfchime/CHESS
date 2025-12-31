using System;

namespace PurrNet
{
    public class BypassLoggingException : Exception
    {
        public static readonly BypassLoggingException instance = new BypassLoggingException();
    }
}
