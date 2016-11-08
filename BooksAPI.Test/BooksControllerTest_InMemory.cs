using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooksAPI.DTOs;
using System.Web.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace BooksAPI.Test
{
    [TestClass]
    public class BooksControllerTest_InMemory: BookControllerTest_Base
    {
        [TestMethod]
        public async Task GetBook_ShouldReturnBook_IfProvidedValidBookId()
        {
            BookDetailDto expectedBook = db.AddNewBookWithAllFields(DateTime.Now);
            int expectedBookId = expectedBook.Id;
            for (int i = 0; i < 1000; i++)
            {
                using (var config = new HttpConfiguration())
                {
                    WebApiConfig.Register(config);
                    config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                    using (var server = new HttpServer(config))
                    {
                        var client = new HttpClient(server);
                        client.BaseAddress = new Uri("http://localhost/api/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var resp = await client.GetAsync("books/" + expectedBookId);
                        resp.EnsureSuccessStatusCode();
                        var book = await resp.Content.ReadAsAsync<BookDto>();

                        Assert.AreEqual<BookDto>(BooksConverter.BookDetailToBook(expectedBook), book);
                    }
                }
            }
        }

        [TestMethod]
        public async Task GetBook_ShouldReturnBooks_IfProvidedValidDate()
        {
            DateTime date = new DateTime(2011, 10, 20, 12, 55, 53);
            db.AddNewBookWithAllFields(date, title: "test1");
            db.AddNewBookWithAllFields(date, title: "test2");


            using (var config = new HttpConfiguration())
            {
                WebApiConfig.Register(config);
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                using (var server = new HttpServer(config))
                {
                    var client = new HttpClient(server);
                    client.BaseAddress = new Uri("http://localhost/api/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var resp = await client.GetAsync("books/date/"+ date.ToString("yyyy-MM-dd"));
                    resp.EnsureSuccessStatusCode();
                    List<BookDto> actualBooks = await resp.Content.ReadAsAsync<List<BookDto>>();

                    var expectedBooks = db.GetBooksByDate(date);

                    Assert.IsTrue(actualBooks.Count > 1);
                    CollectionAssert.AreEqual(expectedBooks, actualBooks);

                }
            }
        }

        [TestMethod]
        public async Task GetBooks_ShouldReturnAllBooks_ViaInMemoryHttpServer()
        {
            using (var config = new HttpConfiguration())
            {
                WebApiConfig.Register(config);
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                using (var server = new HttpServer(config))
                {
                    var client = new HttpClient(server);
                    client.BaseAddress = new Uri("http://localhost/api/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var resp = await client.GetAsync("books");
                        resp.EnsureSuccessStatusCode();
                        var books = await resp.Content.ReadAsAsync<List<BookDto>>();

                        Assert.IsTrue(books.Count > 0);
                    
                }
            }
        }
    }
}
