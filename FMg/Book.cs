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

        //public string Author { get; set; }

        public Book(string title, string link)
        {
            Title = title;
            Link = link;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
