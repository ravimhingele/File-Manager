using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;


namespace FMg
{
    public partial class Form1 : Form
    {
        Label label1 = new Label();

        ListBox listBox = new ListBox();

        ListView listView = new ListView();
       
        Button ReturnButton = new Button();

        Button FindButton = new Button();

        ComboBox comboBox = new ComboBox();

        NumericUpDown numOfBooks = new NumericUpDown();

        WebClient Client;

        string path;

        string copiedDir;

        MemoryStream copiedFile = new MemoryStream();
        string copiedFileName;
        string copiedFileExtension;

        BinaryFormatter binFormatter;
        User CurrentUser;

        Font GlobalFont;

        public Form1()
        {
            CurrentUser = new User();
            

            InitializeComponent();
            FormClosed += Form1_FormClosed;

            //--Добавление и настройка элементов формы--//
            //---Иконка---//
            this.Icon = new Icon("C:\\Programming\\FMg\\calibre_104497.ico");
            //BackColor = CurrentUser.BackgroundColor;
            GlobalFont = new Font(CurrentUser.FontFamily, (int)CurrentUser.FontSize);



            

            //---TextBox---//
            comboBox.Location = new Point(3, 2);
            comboBox.Height = 30;
            comboBox.Width = 490;
            comboBox.Font = GlobalFont;
            Controls.Add(comboBox);
            string[] exampleSearches = { "Python", "C++", "Java" };
            foreach (string item in exampleSearches)
            {
                comboBox.Items.Add(item);
            }


            numOfBooks.Location = new Point(505, 2);
            numOfBooks.Font = new Font(CurrentUser.FontFamily, 12);
            numOfBooks.Maximum = 100;
            numOfBooks.Minimum = 10;
            numOfBooks.Width = 70;
            Controls.Add(numOfBooks);

            
            //--ListView--//
            ColumnHeader column1 = new ColumnHeader();
            ColumnHeader column2 = new ColumnHeader();
            ColumnHeader column3 = new ColumnHeader();
            ColumnHeader column4 = new ColumnHeader();
            ColumnHeader column5 = new ColumnHeader();

            column1.Text = "Title";
            column2.Text = "Author";
            column3.Text = "Rating";
            column4.Text = "Price";
            column5.Text = "Date";


            column1.Width = 260;
            column2.Width = 200;
            column3.Width = 110;
            column4.Width = 100;
            column5.Width = 100;


            // добавляем столбцы в ListView
            listView.View = View.Details;
            listView.Columns.Add(column1);
            listView.Columns.Add(column2);
            listView.Columns.Add(column3);
            listView.Columns.Add(column4);
            listView.Columns.Add(column5);
            listView.Location = new Point(1, 35);
            listView.Height = 390;
            listView.Width = 780;
            listView.Font = GlobalFont;
            listView.ItemActivate += new EventHandler(listView_ItemActivate);
            Controls.Add(listView);
            





            
            //--ListBox--//
            listBox.Location = new Point(3, 35);
            listBox.Height = 390; //433
            listBox.Width = 677;
            listBox.Font = GlobalFont;
            listBox.BackColor = CurrentUser.BackgroundColor;
            //Controls.Add(listBox);
            





            //---Размер окна---//
            SetSize(800, 500);
            this.Text = "Файловый менеджер"; 
            CenterToScreen();

            //ShowFiles("C:\\");



            listBox.MouseDoubleClick += ListBox_MouseDoubleClick;


            //--Кнопки--//
            ReturnButton.Text = "<-";
            SetButtonSize(ReturnButton);
            ReturnButton.Location = new Point(5, 425);
            ReturnButton.BackColor = Color.PeachPuff;
            ReturnButton.Click += ReturnButton_Click;
            Controls.Add(ReturnButton);

            FindButton.Text = "Найти";
            FindButton.Font = GlobalFont;
            FindButton.Location = new Point(580 ,0);
            FindButton.Height = 30;
            FindButton.Width = 100;
            FindButton.Click += FindButton_Click;
            Controls.Add(FindButton);
            
        }

        

        public void ChangeAppearance(int fontSize, string fontFamily, Color backColor)
        {
            GlobalFont = new Font(fontFamily, fontSize);
            label1.Font = GlobalFont;
            comboBox.Font = GlobalFont;
            listBox.Font = GlobalFont;
            listBox.BackColor = backColor;
        }



        private void ListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Book currentBook = listBox.SelectedItem as Book;
            Process.Start(currentBook.Link);
        }


        private void listView_ItemActivate(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem item = listView.SelectedItems[0];
                Process.Start((string)item.Tag);
            }

        }
        //---Функции кнопок---

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(path) != null)
            {
                path = Path.GetDirectoryName(path);
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SetForm formSettings = new SetForm(listBox, this, CurrentUser);
            formSettings.ShowDialog();
        }



        private void FindButton_Click(object sender, EventArgs e)
        {
            Client = new WebClient();

            Client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/111.0");
            Client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            Client.Headers.Add("Accept-Encoding", "br");
            Client.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");


            List<Book> books = new List<Book>();
            int PageNumber = 1;
            while (books.Count < numOfBooks.Value)
            {
                string re = comboBox.Text;
                string searchUrl = $"https://www.amazon.com/s?k={CreateRef(re)}&i=stripbooks-intl-ship&page={PageNumber}&ref=nb_sb_noss";
                string htmlCode = Client.DownloadString(searchUrl);
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(htmlCode);
                string xpath = "//div[@data-component-type='s-search-result']";
                HtmlNodeCollection bookNodes = document.DocumentNode.SelectNodes(xpath);
                if (bookNodes != null)
                {


                    foreach (HtmlNode node in bookNodes)
                    {
                        if (books.Count < numOfBooks.Value)
                        {
                            var titleNode = node.SelectSingleNode(".//span[@class='a-size-medium a-color-base a-text-normal']");
                            string title = titleNode?.InnerText.Trim();

                            var authorNode = node.SelectSingleNode(".//a[contains(@class, 'a-size-base a-link-normal s-underline-text s-underline-link-text s-link-style')]");
                            string author = authorNode?.InnerText.Trim();

                            var ratingNode = node.SelectSingleNode(".//span[contains(@class, 'a-icon-alt')]");
                            string rating = ratingNode?.InnerText.Trim();

                            var priceNode = node.SelectSingleNode(".//span[contains(@class, 'a-offscreen')]");
                            string price = priceNode?.InnerText.Trim();


                            var dateNode = node.SelectSingleNode(".//span[contains(@class, 'a-size-base a-color-secondary a-text-normal')]");
                            string date = dateNode?.InnerText.Trim();

                            // Получаем ссылку на книгу
                            var linkNode = node.SelectSingleNode(".//a[contains(@class, 'a-link-normal') and contains(@class, 's-underline-text') and contains(@class, 's-underline-link-text') and contains(@class, 's-link-style') and contains(@class, 'a-text-normal')]");
                            string link = linkNode?.GetAttributeValue("href", "");

                            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(link))
                            {
                                books.Add(new Book(title, "https://www.amazon.com" + link, author, rating, price, date));
                            }
                        }
                    }
                    ShowResult(books);
                }
                else
                {
                    MessageBox.Show("Ничего не найдено. Попробуйте переформулировать свой запрос");
                }
                PageNumber++;
            }
        }


        public void ShowResult(List<Book> books)
        {
            listView.Items.Clear();
            foreach (Book book in books)
            {
                ListViewItem item = new ListViewItem(new[] {book.Title, book.Author, book.Rating, book.Price, book.Date});
                item.Tag = book.Link;
                listView.Items.Add(item);
            }
            /*
            listBox.Items.Clear();
            foreach (Book book in books)
            {
                listBox.Items.Add(book);
            }*/
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }







        //---Вспомогательные функции---
        private void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        private void SetButtonSize(Button button)
        {
            button.Height = 35;
            button.Width = 35;
        }

        private string CreateRef(string r)
        {
            r.Replace(" ", "+");
            return r;
        }



        /*
        private void ShowFiles(string p)
        {
            if (File.Exists(p))
            {
                try
                {
                    Process.Start(p);
                    path = Path.GetDirectoryName(p);
                }
                catch { }
            }
            else if (Directory.Exists(p))
            {
                try
                {
                    string[] files = Directory.GetFiles(p);
                    string[] dirs = Directory.GetDirectories(p);
                    label1.Text = p;
                    listView1.Items.Clear();
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        ListViewItem folderItem = new ListViewItem(di.Name, 0);
                        folderItem.SubItems.Add("Папка");
                        folderItem.SubItems.Add(di.LastWriteTime.ToString());
                        folderItem.ImageIndex = 0;
                        folderItem.Tag = di.FullName;
                        listView1.Items.Add(folderItem);
                    }
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        ListViewItem fileItem = new ListViewItem(fi.Name, 1);
                        fileItem.SubItems.Add(fi.Extension);
                        fileItem.SubItems.Add(fi.LastWriteTime.ToString());
                        fileItem.Tag = fi.FullName;
                        fileItem.ImageIndex = 1;
                        listView1.Items.Add(fileItem);
                    }
                }
                catch { }
            }
        }
    */
        
        

       

        

    }
}
