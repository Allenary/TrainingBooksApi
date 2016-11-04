using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BooksAPI.Test
{
    public class  HttpHelper
    {
        private string baseUrl = "http://localhost/library/api/";
        public HttpClient NewHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

    }
}
