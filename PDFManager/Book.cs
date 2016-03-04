using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFManager
{
    public class Book : IComparable<Book>
    {
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        List<string> _labels;
        public List<string> Labels
        {
            set { _labels = value; }
            get { return _labels; }
        }

        string _directory;
        public string Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public Book()
        {
            _labels = new List<string>();
        }

        public int CompareTo(Book rightBook)
        {
            try
            {
                return _name.CompareTo(rightBook.Name);
                
            }
            catch (Exception ex)
            {
                throw new Exception("比较异常", ex.InnerException);
            }
        }  

    }
}
