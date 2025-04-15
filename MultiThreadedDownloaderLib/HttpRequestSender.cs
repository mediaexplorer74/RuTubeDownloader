using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiThreadedDownloaderLib
{
    public static class HttpRequestSender
    {
        public static bool ParseRangeHeaderValue(string headerValue, out long rangeFrom, out long rangeTo)
        {
            rangeFrom = 0;
            rangeTo = -1;

            if (string.IsNullOrEmpty(headerValue))
            {
                return false;
            }

            string[] parts = headerValue.Replace("bytes=", "").Split('-');
            if (parts.Length > 0 && long.TryParse(parts[0], out long from))
            {
                rangeFrom = from;
            }

            if (parts.Length > 1 && long.TryParse(parts[1], out long to))
            {
                rangeTo = to;
            }
            return true;
        }

        internal static HttpRequestResult Send(string v1, string url, object value, NameValueCollection inHeaders, int timeout, bool v2)
        {
            throw new NotImplementedException();
        }

        internal static HttpRequestResult Send(string v, string url, int connectionTimeout, NameValueCollection headers)
        {
            throw new NotImplementedException();
        }
    }
}
