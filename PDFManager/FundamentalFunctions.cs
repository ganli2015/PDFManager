using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Documents;
using System.IO;
using System.Windows.Controls;


namespace PDFManager
{
    public class FundamentalFunctions
    {
        static public bool IsBooknameExist(List<Book> books, string name)
        {
            foreach (Book book in books)
            {
                if (book.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        static public Book GetBook(List<Book> books, string name)
        {
            Book res = null;

            foreach (Book book in books)
            {
                if (book.Name == name)
                {
                    res = book;
                    break;
                }
            }

            return res;
        }

        static public int GetIndexOfBook(List<Book> books, string name)
        {
            int res = -1;

            for (int i = 0; i < books.Count; ++i)
            {
                if (books[i].Name == name)
                {
                    res = i;
                }
            }

            return res;
        }

        static public void AddXMLElement(ref XmlDocument document, Book book)
        {
            XmlNode root = document.SelectSingleNode("Books");            

            XmlElement book_xml = document.CreateElement("Book");
            book_xml.SetAttribute("Name", book.Name);

            List<string> labels = book.Labels;
            foreach (string label in labels)
            {
                XmlElement label_name = document.CreateElement("Label");
                label_name.SetAttribute("Name", label);
                book_xml.AppendChild(label_name);
            }

            book_xml.SetAttribute("Path", book.Directory);
            root.AppendChild(book_xml);
        }

        static public bool SetXMLElement(ref XmlDocument document, Book book)
        {
            XmlNode root = document.SelectSingleNode("Books");

            XmlNodeList nodelist = root.SelectNodes("Book");
            foreach (XmlNode node in nodelist)
            {
                XmlElement desired = node as XmlElement;
                if (book.Name == desired.GetAttribute("Name"))
                {
                    XmlNodeList labelNodeList = desired.SelectNodes("Label");
                    foreach (XmlElement labelNode in labelNodeList)
                    {
                        desired.RemoveChild(labelNode);
                    }

                    List<string> labels = book.Labels;
                    foreach (string label in labels)
                    {
                        XmlElement label_name = document.CreateElement("Label");
                        label_name.SetAttribute("Name", label);
                        node.AppendChild(label_name);
                    }

                    return true;

                }
            }

            return false;
        }

        static public void OutputBookInfo(string filename, string bookDir,List<Book> books)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
            document.AppendChild(declaration);
            XmlElement root = document.CreateElement("Books");

            foreach (Book book in books)
            {
                XmlElement book_xml = document.CreateElement("Book");
                book_xml.SetAttribute("Name", book.Name);

                List<string> labels = book.Labels;
                foreach (string label in labels)
                {
                    XmlElement label_name = document.CreateElement("Label");
                    label_name.SetAttribute("Name", label);
                    book_xml.AppendChild(label_name);
                }

                book_xml.SetAttribute("Path", book.Directory);

                root.AppendChild(book_xml);
            }
            XmlElement directory_elem = document.CreateElement("Directory");
            directory_elem.SetAttribute("Value", bookDir);
            root.AppendChild(directory_elem);

            document.AppendChild(root);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            FileStream stream = new FileStream(filename, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            document.WriteContentTo(writer);
            writer.Flush();
            writer.Close();
            stream.Close();

        }

        static public void UpdateHistoryBookInfo(string filename, string bookDir, List<Book> books)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            XmlNode root = document.SelectSingleNode("Books");
            XmlNodeList nodeList = root.SelectNodes("Book");

            //Update books
            foreach (Book book in books)
            {
                XmlElement book_elem;
                FindBookInXMLNodes(nodeList, book, out book_elem);

                if (book_elem == null)//If the book does not exist , then create new one
                {
                    XmlElement book_xml = document.CreateElement("Book");
                    book_xml.SetAttribute("Name", book.Name);

                    List<string> labels = book.Labels;
                    foreach (string label in labels)
                    {
                        XmlElement label_name = document.CreateElement("Label");
                        label_name.SetAttribute("Name", label);
                        book_xml.AppendChild(label_name);
                    }

                    book_xml.SetAttribute("Path", book.Directory);

                    root.AppendChild(book_xml);
                }
                else//If the book exists , then check labels
                {
                    XmlNodeList labelNodes = book_elem.SelectNodes("Label");
                    List<string> labelList = new List<string>();
                    foreach (XmlElement label_elem in labelNodes)
                    {
                        labelList.Add(label_elem.GetAttribute("Name"));
                    }
                    foreach (string label in book.Labels)
                    {
                        if (!labelList.Contains(label))
                        {
                            XmlElement newLabelElem = document.CreateElement("Label");
                            newLabelElem.SetAttribute("Name", label);
                            book_elem.AppendChild(newLabelElem);
                        }
                    }
                }
            }

            //Update directory
            XmlNodeList directory_elem = root.SelectNodes("Directory");
            bool bookDirExist=false;
            foreach (XmlElement dir in directory_elem)
            {
                if (dir.GetAttribute("Value") == bookDir)
                {
                    bookDirExist=true;
                    break;
                }
            }
            if(!bookDirExist)
            {
                XmlElement newDirElem = document.CreateElement("Directory");
                newDirElem.SetAttribute("Value", bookDir);
                root.AppendChild(newDirElem);
            }

            document.AppendChild(root);

            FileStream stream = new FileStream(filename, FileMode.Open);
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            document.WriteContentTo(writer);
            writer.Flush();
            writer.Close();
            stream.Close();

        }


        static public void OutputLabels(string filename, List<string> labels)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
            document.AppendChild(declaration);
            XmlElement root = document.CreateElement("Labels");

            foreach (string label in labels)
            {
                XmlElement label_xml = document.CreateElement("Label");
                label_xml.SetAttribute("Name", label);

                root.AppendChild(label_xml);
            }

            document.AppendChild(root);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            FileStream stream = new FileStream(filename, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            document.WriteContentTo(writer);
            writer.Flush();
            writer.Close();
            stream.Close();
        }

        static public void ReadBookInfo(string filename,out string bookDir, out List<Book> books)
        {
            books = new List<Book>();

            XmlDocument document = new XmlDocument();
            document.Load(filename);
            XmlNode root = document.SelectSingleNode("Books");
            XmlNodeList nodeList = root.SelectNodes("Book");
            XmlElement directory_elem = root.SelectSingleNode("Directory") as XmlElement;
            bookDir = directory_elem.GetAttribute("Value");

            foreach (XmlNode node in nodeList)
            {
                XmlElement element = node as XmlElement;
                Book book = new Book();
                book.Name = element.GetAttribute("Name");

                XmlNodeList labelnodes = element.SelectNodes("Label");
                List<string> labels = new List<string>();
                foreach (XmlNode labelNode in labelnodes)
                {
                    XmlElement element_label = labelNode as XmlElement;
                    labels.Add(element_label.GetAttribute("Name"));
                }
                book.Labels = labels;

                book.Directory = element.GetAttribute("Path");

                books.Add(book);
            }


        }

        static public void ReadLabels(string filename,  out List<string> labels)
        {
            labels=new List<string>();

            XmlDocument document = new XmlDocument();
            document.Load(filename);
            XmlNode root = document.SelectSingleNode("Labels");
            XmlNodeList nodeList = root.SelectNodes("Label");

            foreach (XmlNode node in nodeList)
            {
                XmlElement element = node as XmlElement;
                string label= element.GetAttribute("Name");
                labels.Add(label);
            }
        }

        static public List<Book> GetUnlabeledBooks(List<Book> books)
        {
            List<Book> res = new List<Book>();

            foreach (Book book in books)
            {
                if (book.Labels.Count == 0)
                {
                    res.Add(book);
                }
            }

            return res;
        }

        static public List<Book> GetLabeledBooks(string label, List<Book> books)
        {
            List<Book> res = new List<Book>();

            foreach (Book book in books)
            {
                if (book.Labels.Contains(label))
                {
                    res.Add(book);
                }
            }

            return res;
        }

        static public void FindBookInXMLNodes(XmlNodeList nodeList, Book book,out XmlElement element_out)
        {
            element_out=null;
            foreach (XmlElement element in nodeList)
            {
                string bookName = element.GetAttribute("Name");
                if (book.Name == bookName)
                {
                    element_out = element;
                    return;
                }
                
            }
        }

        static public void RmoveMenuItem(ContextMenu menu , string head)
        {
            MenuItem toRemove = null;
            foreach (MenuItem item in menu.Items)
            {
                if (item.Header == head)
                {
                    toRemove = item;
                    break;
                }
            }

            if (toRemove != null)
            {
                menu.Items.Remove(toRemove);
            }
        }

        static public void AddMenuItem(ContextMenu menu, MenuItem newItem)
        {
            bool itemExist = false;
            foreach (MenuItem item in menu.Items)
            {
                if (item.Header == newItem.Header)
                {
                    itemExist = true;
                    break;
                }
            }

            if (!itemExist)
            {
                menu.Items.Add(newItem);
            }
        }

        static public List<string> GetLabelsInXML(string filename, Book book)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            XmlNode root = document.SelectSingleNode("Books");
            XmlNodeList nodeList = root.SelectNodes("Book");
            XmlElement book_elem;
            FindBookInXMLNodes(nodeList, book, out book_elem);

            List<string> res = new List<string>();
            if (book_elem == null) return res;

            XmlNodeList labelNodes=book_elem.SelectNodes("Label");
            foreach (XmlElement elem in labelNodes)
            {
                res.Add(elem.GetAttribute("Name"));
            }

            return res;
        }

        static public string TrimBookName(string path, string bookDir,string ext)
        {
            string res = "";
            int startTrimmed = bookDir.Length+1;//考虑"\\"
            if (bookDir[bookDir.Length - 1] == Convert.ToChar("\\"))
            {
                startTrimmed -= 1;
            }

            int endTrimmed = ext.Length;
            for (int i = startTrimmed; i < (path.Length-endTrimmed);++i )
            {
                res += path[i];
            }

            return res;
        }
        
    }
}
