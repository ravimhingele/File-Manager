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
       
        Button ReturnButton = new Button();

        Button FindButton = new Button();

        ComboBox comboBox = new ComboBox();

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
            comboBox.Width = 570;
            comboBox.Font = GlobalFont;
            Controls.Add(comboBox);
            string[] exampleSearches = { "Python", "C++", "Java" };
            foreach (string item in exampleSearches)
            {
                comboBox.Items.Add(item);
            }

            //--ListBox--//
            listBox.Location = new Point(3, 35);
            listBox.Height = 390; //433
            listBox.Width = 677;
            listBox.Font = GlobalFont;
            listBox.BackColor = CurrentUser.BackgroundColor;
            Controls.Add(listBox);


            //---Размер окна---//
            SetSize(700, 500);
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

        private void AddDirectoriesAndFiles(TreeNode node, DirectoryInfo parent)
        {
            try
            {
                foreach (DirectoryInfo directory in parent.GetDirectories())
                {
                    TreeNode childNode = new TreeNode(directory.Name);
                    childNode.Tag = directory.FullName;
                    node.Nodes.Add(childNode);
                    AddDirectoriesAndFiles(childNode, directory);
                }

                foreach (FileInfo file in parent.GetFiles())
                {
                    TreeNode childNode = new TreeNode(file.Name);
                    childNode.Tag = file.FullName;
                    node.Nodes.Add(childNode);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок доступа к файлам или каталогам.
            }
        }

        

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDirInfo in subDirs)
            {
                try
                {
                    aNode = new TreeNode(subDirInfo.Name, 0, 0);
                    aNode.Tag = subDirInfo;
                    aNode.ImageKey = "folder";
                    if (subDirInfo.GetDirectories().Length > 0)
                    {
                        subSubDirs = subDirInfo.GetDirectories();
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch { }
            }
        }



        private void ListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Book currentBook = listBox.SelectedItem as Book;
            Process.Start(currentBook.Link);
        }
        



        //---Функции кнопок---

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(path) != null)
            {
                path = Path.GetDirectoryName(path);
            }
            //ShowFiles(path);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SetForm formSettings = new SetForm(listBox, this, CurrentUser);
            formSettings.ShowDialog();
        }



        private void FindButton_Click(object sender, EventArgs e)
        {
            Client = new WebClient();
            string searchUrl = "https://www.amazon.com/s?k=python&i=stripbooks-intl-ship&ref=nb_sb_noss";
            string htmlCode = Client.DownloadString(searchUrl);
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(htmlCode);
            string xpath = "//div[@data-component-type='s-search-result']";
            HtmlNodeCollection bookNodes = document.DocumentNode.SelectNodes(xpath);
            if (bookNodes != null)
            {
                List<Book> books = new List<Book>();

                foreach (HtmlNode node in bookNodes)
                {
                    var titleNode = node.SelectSingleNode(".//span[@class='a-size-medium a-color-base a-text-normal']");
                    string title = titleNode?.InnerText.Trim();

                    // Получаем ссылку на книгу
                    var linkNode = node.SelectSingleNode(".//a[contains(@class, 'a-link-normal') and contains(@class, 's-underline-text') and contains(@class, 's-underline-link-text') and contains(@class, 's-link-style') and contains(@class, 'a-text-normal')]");
                    string link = linkNode?.GetAttributeValue("href", "");

                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(link))
                    {
                        books.Add(new Book(title, "https://www.amazon.com" + link));
                    }
                }
                ShowResult(books);
            }
            else
            {
                MessageBox.Show("Ничего не найдено. Попробуйте переформулировать свой запрос");
            }
        }


        public void ShowResult(List<Book> books)
        {
            listBox.Items.Clear();
            foreach (Book book in books)
            {
                listBox.Items.Add(book);
            }
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
