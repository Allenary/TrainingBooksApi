using BooksAPI.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooksAPI.Test
{
    [TestClass]
    public class BooksControllerTest:BookControllerTest_Base
    {

        #region tests for GET

        [TestMethod]
        public async Task GetBooks_ShouldReturnAllBooks_IfBookIdIsNotProvided()
        {
            int expectedBooksCount = db.GetBooksCount();
            List<BookDto> allBooks = db.GetAllBooksInDb();

            using (var client = NewHttpClient())
            {
                var resp = await client.GetAsync("books");
                resp.EnsureSuccessStatusCode();
                var books = await resp.Content.ReadAsAsync<List<BookDto>>();

                
                Assert.AreEqual(expectedBooksCount, books.Count);
                // order of items doesn't metter
                CollectionAssert.AreEquivalent(allBooks, books);
            }
            
        }
        [TestMethod]
        public async Task GetBook_ShouldReturnBook_IfProvidedValidBookId()
        {
            BookDetailDto expectedBook = db.AddNewBookWithAllFields(DateTime.Now);
            int expectedBookId = expectedBook.Id;
            
            using (var client = NewHttpClient())
            {
                var resp = await client.GetAsync("books/" + expectedBookId);
                resp.EnsureSuccessStatusCode();
                var book = await resp.Content.ReadAsAsync<BookDto>();

                Assert.AreEqual<BookDto>(BooksConverter.BookDetailToBook(expectedBook), book);
            }
        }

        [TestMethod]
        public async Task GetBook_ShouldReturnBooks_IfProvidedValidDate()
        {
            DateTime date = new DateTime(2011, 10, 20, 12, 55, 53);
            db.AddNewBookWithAllFields(date, title: "test1");
            db.AddNewBookWithAllFields(date, title: "test2");

            using (var client = NewHttpClient())
            {
                var resp = await client.GetAsync("books/date/" + date.ToString("yyyy-MM-dd"));
                resp.EnsureSuccessStatusCode();
                var books = await resp.Content.ReadAsAsync<List<BookDto>>();
                var expectedBooks = db.GetBooksByDate(date);

                Assert.IsTrue(books.Count > 1);
                CollectionAssert.AreEquivalent(expectedBooks, books);
            }
        }


        #endregion tests for GET

        [TestMethod]
        public async Task PostBook_ShouldCreateNewBookAndReturnItsDto()
        {
            var book = new BookDetailDto { Id = db.NextBookExternalId(),
                Author = "Ralls, Kim",
                Title = "Integration testing",
                Genre = "Genre",
                Description = "Description",
                Price = 100.1m,
                PublishDate = DateTime.Now };

            var countBooksBeforeAdd = db.GetBooksCount();

            using (var client = NewHttpClient())
            {
                var resp = await client.PostAsJsonAsync("books", book);
                resp.EnsureSuccessStatusCode();
                var result = await resp.Content.ReadAsAsync<BookDetailDto>();

                Assert.AreEqual(countBooksBeforeAdd + 1, db.GetBooksCount());
                Assert.AreEqual<BookDetailDto>(result, book);
            }
        }

        #region tests for PUT
        [TestMethod]
        public async Task PutBook_ShouldUpdateBook_IfBookExistsInDatabase()
        {
            int bookId = db.AddNewBookWithAllFields(DateTime.Now).Id;
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

                var updatedBook = db.GetDetailBookByExternalId(bookId);

                Assert.AreEqual<BookDetailDto>(bookToUpdate, updatedBook);
            }
        }

        [TestMethod]
        public async Task PutBook_ShouldReturnError_IfProvidedInvalidBookId()
        {
            int bookId = db.NextBookExternalId();
            BookDetailDto updatedBook = new BookDetailDto
            {
                Id = bookId,
                Author = "Ralls, Kim",
                Title = "Testing title",
                Price = 99.9m,
                Description = "abcde",
                Genre = "new genre",
                PublishDate = new DateTime(2016, 9, 20, 10, 10, 10)
            };
            using (var client = NewHttpClient()) { 
                var resp = await client.PutAsJsonAsync("books/" + bookId.ToString(), updatedBook);
                
                //Not sure this is the best Assertion
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, resp.StatusCode);
            }

        }
        #endregion tests for PUT

        #region tests for DELETE
        [TestMethod]
        public async Task DeleteBook_ShouldDeleteBook_IfProvidedValidBookId()
        {
            int bookId = db.AddNewBookWithAllFields(DateTime.Now).Id;
            int countBooksBeforeTest = db.GetBooksCount();

            using (var client = NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());
                resp.EnsureSuccessStatusCode();

                Assert.AreEqual(countBooksBeforeTest - 1, db.GetBooksCount());
                Assert.IsNull(db.GetDetailBookByExternalId(bookId));

            }
        }
        [TestMethod]

        public async Task DeleteBook_ShouldThrowException_IfProvidedInvalidBookId()
        {
            int bookId = db.NextBookExternalId();
            using (var client = NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());

                Assert.AreEqual(resp.StatusCode, System.Net.HttpStatusCode.NotFound);
            }
        }

        #endregion tests for DELETE     


    }

}
