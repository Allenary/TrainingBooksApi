﻿namespace BooksAPI.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType()!=this.GetType()) return false;
            var comparedObj = (BookDto)obj;
            return (this.Id == comparedObj.Id)&&
               (this.Title == comparedObj.Title);

        }
    }
}