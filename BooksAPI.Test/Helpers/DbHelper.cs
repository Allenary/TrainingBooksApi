using BooksAPI.DTOs;
using BooksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BooksAPI.Test
{
    public class DbHelper
    {
        private BooksAPIContext db;
        public DbHelper() {
            db = new BooksAPIContext();
        }
        
        public int GetMaxBookExternalId()
        {
            return db.Books.Max(b => b.ExternalId);
        }

        public int GetBooksCount()
        {
            return db.Books.Count();
        }

        public BookDetailDto AddNewBookWithAllFields(DateTime publishDate,
            string author = "Ralls, Kim",
            string title = "test title",
            decimal price = 0.0m,
            string genre = "Genre",
            string description = "Description"
            )
        {
            int bookId = GetMaxBookExternalId() + 1;
            BookDetailDto newBook = new BookDetailDto
            {
                Id = bookId,
                Author = author,
                Title = title,
                Price = 100.1m,
                Genre = genre,
                Description = "Description",
                PublishDate = publishDate
            };
            db.Books.Add(newBook.ToModel());
            db.SaveChanges();
            return newBook;
        }

        public BookDetailDto GetDetailBookByExternalId(int bookId)
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

        public List<BookDto> GetBooksByDate(DateTime date)
        {
            try
            {
                return (from b in db.Books
                        join a in db.Authors on b.AuthorId equals a.AuthorId
                        where b.PublishDate == date
                        select new BookDto
                        {
                            Id = b.ExternalId,
                            Title = b.Title,
                            Genre = b.Genre,
                            Author = b.Author.Name
                        }).ToList<BookDto>();
            }
            catch (Exception E) { return null; }
        }

        public List<BookDto> GetAllBooksInDb()
        {
            return db.Books.Select(b => new BookDto
            {
                Id = b.ExternalId,
                Author = b.Author.Name,
                Genre = b.Genre,
                Title = b.Title
            }).ToList();
        }

        public void DeleteBooksFromId(int id)
        {
            var booksToDelete = db.Books.Where(b => b.ExternalId > id);
            db.Books.RemoveRange(booksToDelete);
            db.SaveChanges();
        }
    }
}
