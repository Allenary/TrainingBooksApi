﻿using BooksAPI.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooksAPI.Test
{
    [TestClass]
    public class BooksControllerTest
    {
        private DbHelper db = new DbHelper();
        private HttpHelper http = new HttpHelper();

        #region tests for GET


        [TestMethod]
        public async Task GetBooks_ShouldReturnAllBooks_IfBookIdIsNotProvided()
        {
            int expectedBooksCount = db.GetBooksCount();
            List<BookDto> allBooks = db.GetAllBooksInDb();

            using (var client = http.NewHttpClient())
            {
                var resp = await client.GetAsync("books");
                resp.EnsureSuccessStatusCode();
                var books = await resp.Content.ReadAsAsync<List<BookDto>>();


                Assert.AreEqual(expectedBooksCount, books.Count);
                CollectionAssert.AreEqual(allBooks, books);
            }
            
        }
        [TestMethod]
        public async Task GetBook_ShouldReturnBook_IfProvidedValidBookId()
        {
            BookDetailDto expectedBook = db.AddNewBookWithAllFields(DateTime.Now);
            int expectedBookId = expectedBook.Id;
            
            using (var client = http.NewHttpClient())
            {
                var resp = await client.GetAsync("books/" + expectedBookId);
                resp.EnsureSuccessStatusCode();
                var book = await resp.Content.ReadAsAsync<BookDto>();

                Assert.AreEqual<BookDto>(BooksConverter.BookDetailToBook(expectedBook), book);
            }
        }
        

        #endregion tests for GET

        [TestMethod]
        public async Task PostBook_ShouldCreateNewBookAndReturnItsDto()
        {
            var book = new BookDetailDto { Id = db.GetMaxBookExternalId() + 1,
                Author = "Ralls, Kim",
                Title = "Integration testing",
                Genre = "Genre",
                Description = "Description",
                Price = 100.1m,
                PublishDate = DateTime.Now };

            var countBooksBeforeAdd = db.GetBooksCount();

            using (var client = http.NewHttpClient())
            {
                var resp = await client.PostAsJsonAsync("books", book);
                resp.EnsureSuccessStatusCode();
                var result = await resp.Content.ReadAsAsync<BookDetailDto>();

                Assert.AreEqual(countBooksBeforeAdd + 1, db.GetBooksCount());
                Assert.AreEqual<BookDetailDto>(result, book);
            }
        }

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

            using (var client = http.NewHttpClient())
            {
                var resp = await client.PutAsJsonAsync("books/" + bookId.ToString(), bookToUpdate);
                resp.EnsureSuccessStatusCode();

                var updatedBook = db.GetDetailBookByExternalId(bookId);

                Assert.AreEqual<BookDetailDto>(bookToUpdate, updatedBook);
            }
        }


        #region tests for DELETE
        [TestMethod]
        public async Task DeleteBook_ShouldDeleteBook_IfProvidedValidBookId()
        {
            int bookId = db.AddNewBookWithAllFields(DateTime.Now).Id;
            int countBooksBeforeTest = db.GetBooksCount();

            using (var client = http.NewHttpClient())
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
            int bookId = db.GetMaxBookExternalId() + 1;
            using (var client = http.NewHttpClient())
            {
                var resp = await client.DeleteAsync("books/" + bookId.ToString());

                Assert.AreEqual(resp.StatusCode, System.Net.HttpStatusCode.NotFound);
            }
        }

        #endregion tests for DELETE     
    }

}
