using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AngleSharp;
using AngleSharp.Network;

namespace Accursed.Services
{
     /// <summary>
    /// The default HTTP response encapsulation object.
    /// </summary>
    public sealed class Response : IResponse
    {
        #region ctor

        /// <summary>
        /// Creates a new default response object.
        /// </summary>
        public Response()
        {
            Headers = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            StatusCode = HttpStatusCode.Accepted;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the url of the response.
        /// </summary>
        public Url Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the headers (key-value pairs) of the response.
        /// </summary>
        public IDictionary<String, String> Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a stream for content of the response.
        /// </summary>
        public Stream Content
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (Content != null)
                Content.Dispose();

            Headers.Clear();
        }

        #endregion
    }

    public static class Helpers
    {
        /// <summary>
        /// Returns the string representation for the specified HTTP method.
        /// </summary>
        /// <param name="method">The type of HTTP method to stringify.</param>
        /// <returns>The string representing the HTTP method.</returns>
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
    }
}
