using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMg
{
    public partial class LoginForm : Form
    {
        Label LoginLabel;
        Label PasswordLabel;
        TextBox LoginBox;
        TextBox PasswordBox;
        Font LoginFormFont;
        Button LoginButton;
        Button RegistrationButton;

        int LabelHeight = 20;
        public LinkedList<User> Users = new LinkedList<User>();

        public LoginForm()
        {
            InitializeComponent();
            Text = "Файловый менеджер";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Size = new Size(320, 210);
            LoginFormFont = new Font("Arial", 12);
            FormClosed += FormLogin_FormClosed;
            CenterToParent();

            LoginLabel = new Label();
            LoginLabel.Text = "Логин: ";
            LoginLabel.Font = LoginFormFont;
            LoginLabel.Location = new Point(30, 30);
            LoginLabel.Size = new Size(70, LabelHeight);
            Controls.Add(LoginLabel);

            LoginBox = new TextBox();
            LoginBox.Font = LoginFormFont;
            LoginBox.Location = new Point(120, 25);
            LoginBox.Size = new Size(150, 20);
            Controls.Add(LoginBox);

            PasswordLabel = new Label();
            PasswordLabel.Text = "Пароль:";
            PasswordLabel.Font = LoginFormFont;
            PasswordLabel.Location = new Point(30, 70);
            PasswordLabel.Size = new Size(70, LabelHeight);
            Controls.Add(PasswordLabel);

            PasswordBox = new TextBox();
            PasswordBox.Font = LoginFormFont;
            PasswordBox.Location = new Point(120, 65);
            PasswordBox.Size = new Size(150, 20);
            Controls.Add(PasswordBox);

            LoginButton = new Button();
            LoginButton.Text = "Вход";
            LoginButton.Font = LoginFormFont;
            LoginButton.Location = new Point(190, 110);
            LoginButton.Size = new Size(100, LabelHeight * 3 / 2);
            LoginButton.Click += LoginButton_Click;
            Controls.Add(LoginButton);

            RegistrationButton = new Button();
            RegistrationButton.Text = "Регистрация";
            RegistrationButton.Font = LoginFormFont;
            RegistrationButton.Location = new Point(20, 110);
            RegistrationButton.Size = new Size(160, LabelHeight * 3 / 2);
            RegistrationButton.Click += RegistrationButton_Click;
            Controls.Add(RegistrationButton);


            BinaryFormatter binFormatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("users.bin", FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    Users = (LinkedList<User>)binFormatter.Deserialize(fs);
                }
                else
                {
                    Users = new LinkedList<User>();
                }
            }
            
        }

        private void RegistrationButton_Click(object sender, EventArgs e) // регистрация нового пользователя
        {
            if (LoginBox.Text != "" && PasswordBox.Text != "")
            {
                if (FindLogin(Users, LoginBox.Text) == null)
                {
                    Users.AddLast(new User(LoginBox.Text, PasswordBox.Text));
                    Form1 formMain = new Form1(this, Users.Last.Value);
                    formMain.FormClosed += Form1_FormClosed;
                    Hide();
                    formMain.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Этот логин уже используется!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void LoginButton_Click(object sender, EventArgs e) // вход
        {
            if (LoginBox.Text != "" && PasswordBox.Text != "")
            {
                User FindedUser = FindLogin(Users, LoginBox.Text);
                if (FindedUser != null) //если найден пользователь
                {
                    if (FindedUser.CheckPassword(PasswordBox.Text)) // проверяем правильность пароля
                    {
                        Form1 formMain = new Form1(this, FindedUser);
                        formMain.FormClosed += Form1_FormClosed;
                        Hide();
                        formMain.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Неправильный пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователя с данным логином не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Не были введены данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }

        public User FindLogin(LinkedList<User> UsersList, string login)
        {
            LinkedListNode<User> temp = UsersList.First;
            while (temp != null)
            {
                if (temp.Value.Login == login)
                {
                    return temp.Value;
                }
                temp = temp.Next;
            }
            return null;
        }
        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, System.EventArgs e)
        {

        }
    }
}
