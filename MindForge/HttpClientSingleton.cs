using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClient
{
    internal class HttpClientSingleton
    {
        internal static HttpClient? httpClient;
        internal static void Set()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };
            httpClient = new HttpClient(socketsHandler);
        }
    }
}
