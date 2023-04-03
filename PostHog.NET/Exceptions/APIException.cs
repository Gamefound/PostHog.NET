using System;

namespace PostHog.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string code, string message) : base($"Status Code: {code}, Message: {message}")
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}