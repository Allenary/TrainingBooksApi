using BooksAPI.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BooksAPI.Test
{
    [TestClass]
    public class BooksControllerTest
    {      
        private HttpClient NewHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost/library/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        [TestMethod]
        public async Task GetBooks_ShouldRetutnAllBooks_IfBookIdIsNotProvided()
        {
            var count = Enumerable.Range(0, 560);
            foreach (var attempt in count)
            {
                using (var client = NewHttpClient())
                {
                    var resp = await client.GetAsync("books");
                    resp.EnsureSuccessStatusCode();
                    var books = await resp.Content.ReadAsAsync<List<BookDto>>();
                    Assert.IsTrue(books.Count > 0);
                }
            }
        }

        [TestMethod]
        public async Task PostBook_ShouldCreateNewBookAndRetutnItsDto()
        {
            var book = new BookDetailDto {Id = 100, Author = "Ralls, Kim", Title = "Integration testing", Price = 100.1m, PublishDate = DateTime.Now };

            using (var client = NewHttpClient())
            {
                var resp = await client.PostAsJsonAsync("books", book);
                resp.EnsureSuccessStatusCode();
                var result = await resp.Content.ReadAsAsync<BookDetailDto>();

                Assert.AreEqual(result.Title, book.Title);
            }
        }

        [TestMethod]
        public async Task PutBook_ShouldUpdateBook_IfBookExistsInDatabase()
        {
            var book = new BookDetailDto { Id = 100, Author = "Ralls, Kim", Title = "New testing", Price = 100.1m, PublishDate = DateTime.Now };

            using (var client = NewHttpClient())
            {
                var resp = await client.PutAsJsonAsync("books/100", book);
                resp.EnsureSuccessStatusCode();
            }
        }

        //[TestMethod]
        //public async Task GetBooks_ShouldRetutnAllBooks_ViaInMemoryHttpServer()
        //{
        //    using (var config = new HttpConfiguration())
        //    {
        //        WebApiConfig.Register(config);
        //        config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        //        var controller = typeof(BooksController);
        //        using (var server = new HttpServer(config))
        //        {
        //            var count = Enumerable.Range(0, 560);
        //            foreach (var attempt in count)
        //            {
        //                var client = new HttpClient(server);
        //                client.BaseAddress = new Uri("http://localhost/api/");
        //                client.DefaultRequestHeaders.Accept.Clear();
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                var resp = await client.GetAsync("books");
        //                resp.EnsureSuccessStatusCode();
        //                var books = await resp.Content.ReadAsAsync<List<BookDto>>();
        //                Assert.IsTrue(books.Count > 0);
        //            }

        //        }
        //    }
        //}

        //[TestMethod]
        //public void SelfHostingServiceTest()
        //{
        //    var config = new HttpSelfHostConfiguration("http://localhost:3000");

        //    var controllerType = typeof(Controllers.BooksController);
        //    config.Routes.MapHttpRoute("default", "api/{controller}/{id}", 
        //        new { id = RouteParameter.Optional });
        //    var server = new HttpSelfHostServer(config);
        //    server.OpenAsync().Wait();

        //    var client = new HttpClient(server);
        //    var response = client.GetAsync("http://localhost:3000/api/Books").Result;
        //    response.EnsureSuccessStatusCode();
        //    var books = response.Content.ReadAsAsync<List<BookDto>>().Result;
        //    Assert.IsTrue(books.Count > 0);
        //}
    }
}
