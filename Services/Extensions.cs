using System;
using AngleSharp.Network;

namespace Accursed.Services
{
    public static class Extensions
    {
        public static String Stringify(this HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.Get:
                    return "GET";
                case HttpMethod.Delete:
                    return "DELETE";
                case HttpMethod.Post:
                    return "POST";
                case HttpMethod.Put:
                    return "PUT";
                default:
                    return method.ToString().ToUpperInvariant();
            }
        }

        public static string StripStart(this string value, string prefix)
        {
            return value.StartsWith(prefix) ? value.Substring(prefix.Length) : value;
        }

        public static string StripEnd(this string value, string prefix)
        {
            return value.EndsWith(prefix) ? value.Substring(0, value.Length - prefix.Length) : value;
        }
    }
}
