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

namespace FMg
{
    public partial class Form1 : Form
    {
        Label label1 = new Label();
        TreeView treeView1 = new TreeView();
        ListView listView1 = new ListView();
        Button CopyButton = new Button();
        Button InsertButton = new Button();
        Button DeleteButton = new Button();
        Button RenameButton = new Button();
        Button ArchiveButton = new Button();
        Button ReturnButton = new Button();
        Button SettingsButton = new Button();
        Button NewFolderButton = new Button();
        Button DeArchivateButton = new Button();
        ImageList imageList = new ImageList();

        string path;

        string copiedDir;

        MemoryStream copiedFile = new MemoryStream();
        string copiedFileName;
        string copiedFileExtension;

        BinaryFormatter binFormatter;
        User CurrentUser;
        LoginForm LoginForm;

        Font GlobalFont;

        public Form1(LoginForm loginForm, User currentUser)
        {
            CurrentUser = currentUser;
            LoginForm = loginForm;

            InitializeComponent();
            FormClosed += Form1_FormClosed;

            //--Добавление и настройка элементов формы--//
            //---Иконка---//
            this.Icon = new Icon("C:\\Programming\\FMg\\calibre_104497.ico");
            //BackColor = CurrentUser.BackgroundColor;
            GlobalFont = new Font(CurrentUser.FontFamily, (int)CurrentUser.FontSize);



            //---TreeView---//
            treeView1.Width = 300;
            treeView1.Height = 460;
            treeView1.Font = GlobalFont; // устанавливаем шрифт Arial размера 12
            Controls.Add(treeView1);

            //---TextBox---//
            label1.Location = new Point(303, 2);
            label1.Height = 30;
            label1.Width = 640;
            label1.Font = GlobalFont;
            label1.BackColor = Color.Gray;
            Controls.Add(label1);

            //--ListView--//
            listView1.View = View.LargeIcon;
            
            imageList.ImageSize = new Size(30, 30);
            imageList.Images.Add("folder", Properties.Resources.downloadfolderblank_99350);
            imageList.Images.Add("file", Properties.Resources.fileinterfacesymboloftextpapersheet_79740);
            listView1.Location = new Point(303, 35);
            listView1.Height = 390; //433
            listView1.Width = 677;
            listView1.Font = GlobalFont;
            listView1.BackColor = currentUser.BackgroundColor;
            listView1.LargeImageList = imageList;
            Controls.Add(listView1);


            //---Размер окна---//
            SetSize(1000, 500);
            this.Text = "Файловый менеджер"; // заголовок
            CenterToScreen(); // центрируем форму

            PopulateTreeView(treeView1);


            //treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            treeView1.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
            listView1.ItemActivate += new EventHandler(listView1_ItemActivate);


            //--Кнопки--//
            ReturnButton.Text = "<-";
            SetButtonSize(ReturnButton);
            ReturnButton.Location = new Point(763, 425);
            ReturnButton.BackColor = Color.PeachPuff;
            ReturnButton.Click += ReturnButton_Click;
            Controls.Add(ReturnButton);

            
            CopyButton.Text = "C";
            SetButtonSize(CopyButton);
            CopyButton.BackColor = Color.PeachPuff;
            CopyButton.Location = new Point(800, 425);
            CopyButton.Click += CopyButton_Click;
            Controls.Add(CopyButton);
            

            /*
            InsertButton.Text = "I";
            SetButtonSize(InsertButton);
            InsertButton.BackColor = Color.PeachPuff;
            InsertButton.Location = new Point(837, 425);
            InsertButton.Click += InsertButton_Click;
            Controls.Add(InsertButton);
            */ 


            DeleteButton.Text = "D";
            SetButtonSize(DeleteButton);
            DeleteButton.BackColor = Color.PeachPuff;
            DeleteButton.Location = new Point(874, 425);
            DeleteButton.Click += DeleteButton_Click;
            Controls.Add(DeleteButton);


            RenameButton.Text = "R";
            SetButtonSize(RenameButton);
            RenameButton.BackColor = Color.PeachPuff;
            RenameButton.Location = new Point(911, 425);
            RenameButton.Click += RenameButton_Click;
            Controls.Add(RenameButton);


            ArchiveButton.Text = "A";
            SetButtonSize(ArchiveButton);
            ArchiveButton.BackColor = Color.PeachPuff;
            ArchiveButton.Location = new Point(948, 425);
            ArchiveButton.Click += ArchiveButton_Click;
            Controls.Add(ArchiveButton);


            SettingsButton.Text = "S";
            SetButtonSize(SettingsButton);
            SettingsButton.BackColor = Color.Beige;
            SettingsButton.Location = new Point(945, 0);
            SettingsButton.Click += SettingsButton_Click;
            Controls.Add(SettingsButton);


            NewFolderButton.Text = "N";
            SetButtonSize(NewFolderButton);
            NewFolderButton.BackColor = Color.PeachPuff;
            NewFolderButton.Location = new Point(726, 425);
            NewFolderButton.Click += NewFolderButton_Click;
            Controls.Add(NewFolderButton);

            DeArchivateButton.Text = "Z";
            SetButtonSize(DeArchivateButton);
            DeArchivateButton.BackColor = Color.PeachPuff;
            DeArchivateButton.Location = new Point(689, 425);
            DeArchivateButton.Click += DeArchivateButton_Click;
            Controls.Add(DeArchivateButton);
        }

        

        public void ChangeAppearance(int fontSize, string fontFamily, Color backColor)
        {
            GlobalFont = new Font(fontFamily, fontSize);
            label1.Font = GlobalFont;
            treeView1.Font = GlobalFont;
            listView1.Font = GlobalFont;
            listView1.BackColor = backColor;
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

        private void PopulateTreeView(TreeView tree)
        {

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    DirectoryInfo info1 = new DirectoryInfo(drive.RootDirectory.FullName);
                    if (info1.Exists)
                    {
                        TreeNode rootNode = new TreeNode(info1.Name);
                        rootNode.Tag = info1;
                        treeView1.Nodes.Add(rootNode);
                        
                        AddDirectoriesAndFiles(rootNode, info1);
                    }
                }
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

        
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            /*
            if (e.Node != null)
            {
                string p = "";

                if (e.Node.Tag != null)
                {
                    string tag = e.Node.Tag.ToString();
                    if (File.Exists(tag))
                    {
                        p = Path.GetDirectoryName(tag);
                    }
                    else if (Directory.Exists(tag))
                    {
                        p = Path.GetDirectoryName(tag);
                        //path = tag;
                    }
                    else
                    {
                        p = "Указанный путь не существует";
                    }
                }
                label1.Text = p; //p - path
            }
            */

            // Получаем выбранный узел
            TreeNode selectedNode = e.Node;

            // Определяем тип элемента (файл или папка)
            path = selectedNode.Tag.ToString();
            if (File.Exists(path))
            {
                try
                {
                    // Открываем файл
                    Process.Start(path);
                }
                catch { }
            }
            else if (Directory.Exists(path))
            {
                try
                {
                    // Получаем список файлов и папок внутри папки
                    string[] files = Directory.GetFiles(path);
                    string[] dirs = Directory.GetDirectories(path);
                    label1.Text = path;
                    // Заполняем данные для отображения в ListView
                    listView1.Items.Clear();
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        ListViewItem item = new ListViewItem(di.Name, 0);
                        item.Tag = di;
                        item.SubItems.Add("Папка");
                        item.SubItems.Add(di.LastWriteTime.ToString());
                        item.ImageIndex = 0; // Индекс иконки для папки
                        listView1.Items.Add(item);
                    }
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        ListViewItem item = new ListViewItem(fi.Name, 1);
                        item.Tag = fi;
                        item.SubItems.Add(fi.Extension);
                        item.SubItems.Add(fi.LastWriteTime.ToString());
                        //item.Tag = fi.FullName;
                        item.ImageIndex = 1; // Индекс иконки для папки
                        listView1.Items.Add(item);
                    }
                }
                catch { }
            }
        }


        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                path = item.Tag.ToString();

                ShowFiles(path);
            }
            
        }
        



        //---Функции кнопок---

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            if (Path.GetDirectoryName(path) != null)
            {
                path = Path.GetDirectoryName(path);
            }
            ShowFiles(path);
        }


        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0]; // имеет тип listViewItem
                Console.WriteLine(selectedItem.Tag.ToString());
                string p = selectedItem.Tag.ToString();
                // Обработка выбранного элемента

                if (selectedItem != null)
                {
                    if (Path.HasExtension(p))
                    {
                        Console.WriteLine("The file is copied!" + p);
                        CopyFile(p);
                    }
                    else if (!Path.HasExtension(p))
                    {
                        Console.WriteLine("The directory is copied!");
                        CopyDir(p);
                    }
                    else
                    {
                        // объект не является ни FileInfo, ни DirectoryInfo
                    }
                }
            }
            
        }

        
        private void InsertButton_Click(object sender, EventArgs e)
        {
            if (copiedFileName != "")
            {
                Console.WriteLine("The file is inserted!");
                string filePath = NewPath(Path.Combine(path, copiedFileName + copiedFileExtension));
                File.WriteAllBytes(filePath, copiedFile.ToArray());
                //using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    copiedFile.WriteTo(fileStream);
                //}
                ShowFiles(path);
            }
            if (copiedDir != null)
            {
                string pathForCopy = Path.Combine(path, Path.GetFileName(copiedDir));
                Directory.CreateDirectory(pathForCopy); //2TestFileManager/New Folder
                foreach (string dirPath in Directory.GetDirectories(copiedDir, "*", System.IO.SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(copiedDir, pathForCopy));

                foreach (string newPath in Directory.GetFiles(copiedDir, "*.*", System.IO.SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(copiedDir, pathForCopy), true);
                ShowFiles(path);
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                try
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    //throw new Exception();
                    string p = selectedItem.Tag.ToString();
                    
                    if (Path.HasExtension(p))
                    {
                        FileInfo fileInfo = new FileInfo(p);
                        string message = $"Вы уверены, что хотите удалить {fileInfo.Name}?";
                        DialogResult result = MessageBox.Show(message, "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            FileSystem.DeleteFile(fileInfo.FullName, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }
                    else if (!Path.HasExtension(p))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(p);
                        string message = $"Вы уверены, что хотите удалить {directoryInfo.Name}?";
                        DialogResult result = MessageBox.Show(message, "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            FileSystem.DeleteDirectory(directoryInfo.FullName, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show($"Не удалось удалить {listView1.SelectedItems[0]}");
                }
            }
            ShowFiles(path);
        }

        
        private void RenameButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string p = selectedItem.Tag.ToString();
                if (Path.HasExtension(p))
                {
                    FileInfo fileInfo = new FileInfo(p);
                    string oldName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    string extension = Path.GetExtension(fileInfo.Name);
                    string newName = Microsoft.VisualBasic.Interaction.InputBox($"Новое имя файла {oldName}:", "Переименование файла", oldName);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(fileInfo.FullName), newName + extension);
                        File.Move(fileInfo.FullName, newPath);
                    }
                }
                else if (!Path.HasExtension(p))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(p);
                    string oldName = Path.GetFileNameWithoutExtension(directoryInfo.Name);
                    string extension = Path.GetExtension(directoryInfo.Name);
                    string newName = Microsoft.VisualBasic.Interaction.InputBox($"Новое имя папки {oldName}:", "Переименование папки", oldName);
                    if (!string.IsNullOrEmpty(newName) && newName != oldName)
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(directoryInfo.FullName), newName + extension);
                        Directory.Move(directoryInfo.FullName, newPath);
                    }
                }
                ShowFiles(path);
            }
        }


        private void ArchiveButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string p = selectedItem.Tag.ToString();
                if (Path.HasExtension(p))
                {
                    FileInfo fileInfo = new FileInfo(p);
                    string zipFilePath = Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + ".zip");
                    int n = 0;
                    while (File.Exists(zipFilePath))
                    {
                        n += 1;
                        zipFilePath = Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + $"({n})" + ".zip");
                    }
                    using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                    ShowFiles(path);
                }
                else if (!Path.HasExtension(p))
                {
                    ArchDir(p);
                }
            }
        }


        private void NewFolderButton_Click(object sender, EventArgs e)
        {
            string pathOfNewFolder = Path.Combine(path, "Новая папка");
            if (Directory.Exists(pathOfNewFolder))
            {
                int n = 0;
                string newPath = pathOfNewFolder;
                while (Directory.Exists(newPath))
                {
                    n += 1;
                    newPath = Path.Combine(Path.GetDirectoryName(pathOfNewFolder), Path.GetFileNameWithoutExtension(pathOfNewFolder) + $"({n})" + Path.GetExtension(pathOfNewFolder));
                }
                pathOfNewFolder = newPath;
            }

            Directory.CreateDirectory(pathOfNewFolder);
            ShowFiles(path);
        }



        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SetForm formSettings = new SetForm(listView1, this, CurrentUser);
            formSettings.ShowDialog();
        }


        private void DeArchivateButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string p = selectedItem.Tag.ToString();
                try
                {
                    string directoryPath = Path.GetDirectoryName(p);
                    ZipFile.ExtractToDirectory(p, directoryPath);
                    Console.WriteLine("Файл успешно разархивирован.");
                    ShowFiles(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при разархивировании файла: " + ex.Message);
                }
            }
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            binFormatter = new BinaryFormatter();
            Console.WriteLine("Объекты сериализуются");
            using (FileStream file = new FileStream("users.bin", FileMode.OpenOrCreate))
            {
                binFormatter.Serialize(file, LoginForm.Users);
            }
            Application.Exit();
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
    
        private void CopyFile(string p)
        {
            try
            {
                copiedFile = new MemoryStream();
                FileInfo fileInfo = new FileInfo(p);
                copiedFileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                copiedFileExtension = Path.GetExtension(fileInfo.Name);
                using (FileStream fileStream = File.OpenRead(p))
                {
                    fileStream.CopyTo(copiedFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при копировании файла в память: " + ex.Message);
            }
        }
        

        private void CopyDir(string p)
        {
            copiedDir = p;
            copiedFileName = "";
            copiedFileExtension = "";
        }


        public string NewPath(string currentPath)
        {
            if (File.Exists(currentPath))
            {
                string message = $"В этой папке уже существует файл с именем {copiedFileName}. Вы уверены, что хотите его заменить?";
                DialogResult result = MessageBox.Show(message, "Вставка", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    FileSystem.DeleteFile(currentPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    int n = 0;
                    string cPath = currentPath;
                    while (File.Exists(currentPath))
                    {
                        n += 1;
                        currentPath = Path.Combine(Path.GetDirectoryName(cPath), Path.GetFileNameWithoutExtension(cPath) + $"({n})" + Path.GetExtension(cPath));
                    }
                }
            }
            return currentPath;
        }




        private void ArchDir(string p)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(p);
            string zipDirName = $"{directoryInfo.FullName}.zip";
            int n = 0;
            while (File.Exists(zipDirName))
            {
                n += 1;
                zipDirName = Path.Combine(directoryInfo.Parent.FullName, Path.GetFileNameWithoutExtension(directoryInfo.FullName) + $"({n})" + ".zip");
            }
            ZipFile.CreateFromDirectory(directoryInfo.FullName, zipDirName);
            ShowFiles(path);
            //return zipDirName;
        }
        

        private void DeleteFile(string p)
        {
            FileInfo fileInfo = new FileInfo(p);
            FileSystem.DeleteFile(fileInfo.FullName, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }

        
        //кнопки: создать файл

        

    }
}
