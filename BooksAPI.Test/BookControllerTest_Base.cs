using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BooksAPI.Test
{
    /*
     * This class is made only with purpose of cleaning data after tests execution.
     */
    [TestClass]
    public class BookControllerTest_Base
    {
        protected static DbHelper db = new DbHelper();
        private static int firstExternalIdForTests;
        private const string baseUrl = "http://localhost/library/api/";

        public HttpClient NewHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        
        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            firstExternalIdForTests = db.NextBookExternalId();
        }

        [AssemblyCleanup]
        public static void ClassCleanup()
        {
            db.DeleteBooksFromId(firstExternalIdForTests);
        }
    }
}
