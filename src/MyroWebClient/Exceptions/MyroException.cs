using System;

namespace MyroWebClient.Exceptions
{
    public class MyroException : Exception
    {
        public MyroException() : base() {}
        public MyroException(string message) : base(message) {}
    }
}