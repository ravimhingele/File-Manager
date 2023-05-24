using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMg
{
    [Serializable]
    public class User
    {
        public string Login;
        public string Password;

        
        public decimal FontSize = 12;
        public string FontFamily = "Arial";
        public Color BackgroundColor = Color.White;
        

        public User() { }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }

        

        private string Encrypt(string data)
        {
            // XOR с ключом
            byte[] key = Encoding.Unicode.GetBytes("MySecretKey");
            byte[] bytes = Encoding.Unicode.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
            }

            return Convert.ToBase64String(bytes);
        }

        private string Decrypt(string data)
        {
            // Обратное преобразование для дешифрования
            byte[] key = Encoding.Unicode.GetBytes("MySecretKey");
            byte[] bytes = Convert.FromBase64String(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
            }

            return Encoding.Unicode.GetString(bytes);
        }

        public bool CheckPassword(string password)
        {
            if (password.Length != Password.Length)
            {
                return false;
            }
            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] != Password[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
