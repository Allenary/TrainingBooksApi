using BooksAPI.DTOs;
using BooksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BooksAPI.Test
{
    public class DbHelper
    {
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
           x => new BookDto
           {
               Id = x.ExternalId,
               Title = x.Title,
               Author = x.Author.Name,
               Genre = x.Genre
           };

        public int NextBookExternalId()
        {
            using (var db = new BooksAPIContext())
            {
                return db.Books.Max(b => b.ExternalId) + 1;
            }
        }

        public int GetBooksCount()
        {
            using (var db = new BooksAPIContext())
            {
                return db.Books.Count();
            }
        }

        public BookDetailDto AddNewBookWithAllFields(DateTime publishDate,
            string author = "Ralls, Kim",
            string title = "test title",
            decimal price = 0.0m,
            string genre = "Genre",
            string description = "Description"
            )
        {
            int bookId = NextBookExternalId() + 1;
            BookDetailDto newBook = new BookDetailDto
            {
                Id = bookId,
                Author = author,
                Title = title,
                Price = 100.1m,
                Genre = genre,
                Description = description,
                PublishDate = publishDate
            };
            using (var db = new BooksAPIContext())
            {
                db.Books.Add(newBook.ToModel());
                db.SaveChanges();
            }
            return newBook;
        }

        public BookDetailDto GetDetailBookByExternalId(int bookId)
        {
            using (var db = new BooksAPIContext())
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
                        }).FirstOrDefault();
            }
           
        }

        public List<BookDto> GetBooksByDate(DateTime date)
        {
            using (var db = new BooksAPIContext())
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
        }

        public List<BookDto> GetAllBooksInDb()
        {
            using (var db = new BooksAPIContext())
            {
                return db.Books.Select(AsBookDto).ToList();
            }
        }

        public void DeleteBooksFromId(int id)
        {
            using (var db = new BooksAPIContext())
            {
                var booksToDelete = db.Books.Where(b => b.ExternalId >= id);
                db.Books.RemoveRange(booksToDelete);
                db.SaveChanges();
            }
        }
    }
}
