using System;

namespace PostHog.Exceptions
{
    public class ApiException : Exception
    {
        public string Code { get; set; }

        public ApiException(string code, string message) : base($"Status Code: {code}, Message: {message}")
        {
            Code = code;
        }
    }
}