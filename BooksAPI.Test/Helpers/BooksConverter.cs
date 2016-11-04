using BooksAPI.DTOs;

namespace BooksAPI.Test
{
    public static class BooksConverter
    {
        public static BookDto BookDetailToBook(BookDetailDto book)
        {
            return new BookDto { Id = book.Id, Author = book.Author, Genre = book.Genre, Title = book.Title };
        }
    }
}
