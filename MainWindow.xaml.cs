using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimplePasswordHasher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Hash function
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static string ComputeHash(string rawData)
        {
            int iterations = 100000;
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(rawData, salt, iterations);
            byte[] hash = rfc2898DeriveBytes.GetBytes(20);
            byte[] hashByte = new byte[36];
            Array.Copy(salt, 0, hashByte, 0, 16);
            Array.Copy(hash, 0, hashByte, 16, 20);
            string b64Hash = Convert.ToBase64String(hashByte);
            string datetime = DateTime.Now.ToString("MMddyyyyTHHmmssfffffffK");
            return string.Format(datetime + "$MYHASH$V1${0}${1}", iterations, b64Hash);
        }
        /// <summary>
        /// Actions when click on the hash button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HashButton_Click(object sender, RoutedEventArgs e)
        {
            // We verify if the password and its confirmation are the same
            if (Password.Password != PasswordVerification.Password)
            {
                MessageBox.Show("The passwords are not the same.");
                Password.Clear();
                PasswordVerification.Clear();
            }
            // Then we verify if they are not null or only white spaces
            else if (string.IsNullOrWhiteSpace(Password.Password) || string.IsNullOrWhiteSpace(PasswordVerification.Password))
            {
                MessageBox.Show("There is no password.");
                Password.Clear();
                PasswordVerification.Clear();
            }
            // Finally we check the length of the password
            else if (Password.Password.Length < 8 || PasswordVerification.Password.Length < 8)
            {
                MessageBox.Show("Preferably choose a password longer than 8 characters.");
                Password.Clear();
                PasswordVerification.Clear();
            }
            // Now that everything is fine, we can print the password and copy it to the clipboard, and close the app
            else if (Password.Password == PasswordVerification.Password && !string.IsNullOrWhiteSpace(Password.Password) &&
                    !string.IsNullOrWhiteSpace(PasswordVerification.Password))
            {
                HashButton.Content = "Hashed successfully !";
                string message = ComputeHash(Password.Password);
                MessageBox.Show("Password : " + message + Environment.NewLine + Environment.NewLine + "Password automatically copied to clipboard.");
                Clipboard.SetText(message);
                Environment.Exit(0);
            }
        }
    }
}
