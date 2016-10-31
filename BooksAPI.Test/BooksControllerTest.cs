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

        private BookDetailDto addNewBookWithAllFields()
        {
            int bookId = GetMaxBookExternalId() + 1;
            BookDetailDto newBook = new BookDetailDto { Id = bookId,
                Author = "Ralls, Kim",
                Title = "Integration testing",
                Price = 100.1m,
                Genre = "Genre",
                Description = "Description",
                PublishDate = DateTime.Now };
            db.Books.Add(newBook.ToModel());
            db.SaveChanges();
            return newBook;
        }

        private BookDetailDto getDetailBookByExternalId(int bookId)
        {
            try
            {
                return (from b in db.Books
                        join a in db.Authors on b.AuthorId equals a.AuthorId
                        where b.ExternalId == bookId
                        select new BookDetailDto
                        {
                            Id = b.ExternalId,
                            Title = b.Title,
                            Genre = b.Genre,
                            PublishDate = b.PublishDate,
                            Price = b.Price,
                            Description = b.Description,
                            Author = b.Author.Name
                        }).First();
            }
            catch (Exception E) { return null; }

        }
        private int GetBooksCount()
        {
            return db.Books.Count();
        }

        private BookDto BookDetailToBook (BookDetailDto book)
        {
            return new BookDto { Id = book.Id, Author = book.Author, Genre = book.Genre, Title = book.Title };
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
            //TODO: get list of all books in db and compare list from db with what we get from api

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
            BookDetailDto expectedBook = addNewBookWithAllFields();
            int expectedBookId = expectedBook.Id;

            using (var client = NewHttpClient())
            {
                var resp = await client.GetAsync("books/"+ expectedBookId);
                resp.EnsureSuccessStatusCode();
                var book = await resp.Content.ReadAsAsync<BookDto>();

                Assert.AreEqual<BookDto>(BookDetailToBook(expectedBook), book);
                
            }
            
        }

        [TestMethod]
        public async Task PostBook_ShouldCreateNewBookAndRetutnItsDto()
        {
            var book = new BookDetailDto { Id = GetMaxBookExternalId() + 1,
                Author = "Ralls, Kim",
                Title = "Integration testing",
                Genre = "Genre",
                Description = "Description",
                Price = 100.1m,
                PublishDate = DateTime.Now };

            var countBooksBeforeAdd = GetBooksCount();

            using (var client = NewHttpClient())
            {
                var resp = await client.PostAsJsonAsync("books", book);
                resp.EnsureSuccessStatusCode();
                var result = await resp.Content.ReadAsAsync<BookDetailDto>();

                Assert.AreEqual(countBooksBeforeAdd + 1, GetBooksCount());
                Assert.AreEqual<BookDetailDto>(result, book);
            }
        }

        [TestMethod]
        public async Task PutBook_ShouldUpdateBook_IfBookExistsInDatabase()
        {
            int bookId = addNewBookWithAllFields().Id;
            BookDetailDto bookToUpdate = new BookDetailDto { Id = bookId,
                Author = "Ralls, Kim",
                Title = "Testing title",
                Price = 99.9m,
                Description = "abcde",
                Genre = "new genre",
                PublishDate = new DateTime(2016, 9, 20, 10, 10, 10)
            };              

            using (var client = NewHttpClient())
            {
                var resp = await client.PutAsJsonAsync("books/" + bookId.ToString(), bookToUpdate);
                resp.EnsureSuccessStatusCode();

                var updatedBook = getDetailBookByExternalId(bookId);

                Assert.AreEqual<BookDetailDto>(bookToUpdate, updatedBook);
            }
        }

        [TestMethod]
        public async Task DeleteBook_ShouldDeleteBook_IfProvidedValidBookId()
        {
            int bookId = addNewBookWithAllFields().Id;
            int countBooksBeforeTest = GetBooksCount();

            using (var client = NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());
                resp.EnsureSuccessStatusCode();

                Assert.AreEqual(countBooksBeforeTest - 1, GetBooksCount());
                Assert.AreEqual(null, getDetailBookByExternalId(bookId));

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
