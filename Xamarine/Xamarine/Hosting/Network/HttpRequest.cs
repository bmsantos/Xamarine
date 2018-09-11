﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if !NET47

#region License

/*
 * HttpRequest.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2015 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion

#region Contributors

/*
 * Contributors:
 * - David Burhans
 */

#endregion

namespace Xamarine.Hosting.Network
{
    internal class HttpRequest : HttpBase
    {
        private bool _websocketRequest;
        private bool _websocketRequestSet;

        internal HttpRequest(string method, string uri)
            : this(method, uri, HttpVersion.Version11, new NameValueCollection())
        {
            Headers["User-Agent"] = "embedio/1.0";
        }

        private HttpRequest(string method, string uri, Version version, NameValueCollection headers)
            : base(version, headers)
        {
            HttpMethod = method;
            RequestUri = uri;
        }

        public CookieCollection Cookies => Headers.GetCookies(false);

        public string HttpMethod { get; }

        public bool IsWebSocketRequest
        {
            get
            {
                if (!_websocketRequestSet)
                {
                    var headers = Headers;
                    _websocketRequest = HttpMethod == "GET" && ProtocolVersion > HttpVersion.Version10 && headers.Contains("Upgrade", "websocket") && headers.Contains("Connection", "Upgrade");

                    _websocketRequestSet = true;
                }

                return _websocketRequest;
            }
        }

        public string RequestUri { get; }

        public void SetCookies(CookieCollection cookies)
        {
            if (cookies.Count == 0)
                return;

            var buff = new StringBuilder(64);
            foreach (Cookie cookie in cookies)
                if (!cookie.Expired)
                    buff.AppendFormat("{0}; ", cookie);

            var len = buff.Length;

            if (len > 2)
            {
                buff.Length = len - 2;
                Headers["Cookie"] = buff.ToString();
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder(64);
            output.AppendFormat("{0} {1} HTTP/{2}{3}", HttpMethod, RequestUri, ProtocolVersion, CrLf);

            var headers = Headers;
            foreach (var key in headers.AllKeys)
                output.AppendFormat("{0}: {1}{2}", key, headers[key], CrLf);

            output.Append(CrLf);

            var entity = EntityBody;
            if (entity.Length > 0)
                output.Append(entity);

            return output.ToString();
        }

        internal static HttpRequest CreateWebSocketRequest(Uri uri)
        {
            var req = new HttpRequest("GET", uri.PathAndQuery);
            var headers = req.Headers;

            // Only includes a port number in the Host header value if it's non-default.
            // See: https://tools.ietf.org/html/rfc6455#page-17
            var port = uri.Port;
            var schm = uri.Scheme;
            headers["Host"] = port == 80 && schm == "ws" || port == 443 && schm == "wss" ? uri.DnsSafeHost : uri.Authority;

            headers["Upgrade"] = "websocket";
            headers["Connection"] = "Upgrade";

            return req;
        }

        internal static HttpRequest Parse(string[] headerParts)
        {
            var requestLine = headerParts[0].Split(new[] {' '}, 3);
            if (requestLine.Length != 3)
                throw new ArgumentException("Invalid request line: " + headerParts[0]);

            return new HttpRequest(requestLine[0], requestLine[1], new Version(requestLine[2].Substring(5)), ParseHeaders(headerParts));
        }

        internal Task<HttpResponse> GetResponse(Stream stream)
        {
            var buff = ToByteArray();
            stream.Write(buff, 0, buff.Length);

            return ReadAsync(stream, HttpResponse.Parse);
        }
    }
}
#endif
