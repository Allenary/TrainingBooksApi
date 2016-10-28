using BooksAPI.DTOs;
using BooksAPI.Models;
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
        private BooksAPIContext db = new BooksAPIContext();

        private int GetMaxBookExternalId()
        {
            return db.Books.Max(b => b.ExternalId);
        }

        private int GetBooksCount()
        {
            return db.Books.Count();
        }
        private HttpClient NewHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost/library/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        [TestMethod]
        public async Task GetBooks_ShouldReturnAllBooks_IfBookIdIsNotProvided()
        {
            int expectedBooksCount = GetBooksCount();

            using (var client = NewHttpClient())
            {
                var resp = await client.GetAsync("books");
                resp.EnsureSuccessStatusCode();
                var books = await resp.Content.ReadAsAsync<List<BookDto>>();
                Assert.IsTrue(books.Count == expectedBooksCount);
            }
            
        }
        [TestMethod]
        public async Task GetBook_ShouldReturnBook_IfProvidedValidBookId()
        {
            var expectedBook = new BookDto { Id = 1, Author = "Ralls, Kim", Title = "Midnight Rain" };

            var count = Enumerable.Range(0, 560);
            foreach (var attempt in count)
            {
                using (var client = NewHttpClient())
                {
                    var resp = await client.GetAsync("books/1");
                    resp.EnsureSuccessStatusCode();
                    var book = await resp.Content.ReadAsAsync<BookDto>();
                    Assert.AreEqual<BookDto>(expectedBook, book);
                }
            }
        }

        [TestMethod]
        public async Task PostBook_ShouldCreateNewBookAndRetutnItsDto()
        {
            var book = new BookDetailDto { Id = GetMaxBookExternalId() + 1, Author = "Ralls, Kim", Title = "Integration testing", Price = 100.1m, PublishDate = DateTime.Now };

            using (var client = NewHttpClient())
            {
                var resp = await client.PostAsJsonAsync("books", book);
                resp.EnsureSuccessStatusCode();
                var result = await resp.Content.ReadAsAsync<BookDetailDto>();

                Assert.AreEqual(result, book);
            }
        }

        [TestMethod]
        public async Task PutBook_ShouldUpdateBook_IfBookExistsInDatabase()
        {
            DateTime date = new DateTime(2016, 9, 20, 10, 10, 10);
            //TODO:create book before update
            int bookId = 1;
            var bookToUpdate = new BookDetailDto { Id = bookId, Author = "Ralls, Kim", Title = "New testing", Price = 100.1m, PublishDate = date };

            using (var client = NewHttpClient())
            {
                var resp = await client.PutAsJsonAsync("books/" + bookId.ToString(), bookToUpdate);
                resp.EnsureSuccessStatusCode();

                var updatedBook = (from b in db.Books
                                   join a in db.Authors on b.AuthorId equals a.AuthorId
                                   where b.BookId == bookId
                                   select new BookDetailDto
                                   {
                                       Id = b.BookId,
                                       Title = b.Title,
                                       Genre = b.Genre,
                                       PublishDate = b.PublishDate,
                                       Price = b.Price,
                                       Description = b.Description,
                                       Author = b.Author.Name
                                   }).First();

                Assert.AreEqual<BookDetailDto>(bookToUpdate, updatedBook);
            }
        }

        [TestMethod]
        public async Task DeleteBook_ShouldDeleteBook_IfProvidedValidBookId()
        {
            //TODO:create book before delete
            int bookId = 105;
            using (var client = NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());
                resp.EnsureSuccessStatusCode();
            }
        }
        [TestMethod]
        public async Task DeleteBook_ShouldThrowException_IfProvidedInvalidBookId()
        {
            int bookId = GetMaxBookExternalId() + 1;
            using (var client = NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());
                Assert.AreEqual(resp.StatusCode, System.Net.HttpStatusCode.NotFound);
            }
        }
    }

}
