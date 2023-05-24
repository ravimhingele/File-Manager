using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMg
{
    public class Book
    {
        public string Title { get; set; }
        public string Link { get; set; }

        public string Author { get; set; }
        public string Rating { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }

        //public string Author { get; set; }

        public Book(string title, string link, string author, string rating, string price, string date)
        {
            Title = title;
            Link = link;
            Author = author;
            Rating = rating;
            Price = price;
            Date = date;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
