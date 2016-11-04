using BooksAPI.Models;
using System;
using System.Linq;

namespace BooksAPI.DTOs
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (obj.GetType() != this.GetType()) return false;
            var comparedObj = (BookDetailDto)obj;
            return (this.Id == comparedObj.Id) &&
                (this.Author == comparedObj.Author) &&
                (this.Title == comparedObj.Title) &&
                (this.Genre == comparedObj.Genre) &&
                (this.PublishDate.Equals(comparedObj.PublishDate)) &&
                (this.Description == comparedObj.Description) &&
                (this.Price == comparedObj.Price);
        }
        public override int GetHashCode()
        {
            return this.Id;
        }

    }

    

    public static class DtoConverter
    {
        public static Book ToModel(this BookDetailDto book)
        {
            using (var db = new BooksAPIContext())
            {
                var authorId = db.Authors.Where(a => a.Name == book.Author).Single().AuthorId;
                return new Book {ExternalId = book.Id, Title = book.Title, AuthorId = authorId,
                    Description = book.Description, Genre = book.Genre,
                    Price = book.Price, PublishDate = book.PublishDate };
            }
        }
    }
}