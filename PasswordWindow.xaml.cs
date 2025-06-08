using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace FileEncryptWPF
{
    public partial class PasswordWindow : Window
    {
        private readonly char _mode;
        private readonly string _inFileName;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Process.GetCurrentProcess().Kill();
        }

        public PasswordWindow(char mode, string inFileName)
        {
            InitializeComponent();

            _mode = mode;
            _inFileName = inFileName;

            if (_mode == 'd')
            {
                ConfirmPasswordTxt.Visibility = Visibility.Hidden;
                ConfirmPasswordLbl.Visibility = Visibility.Hidden;
                PswdWindow.Height = 165;
                Grid.SetRowSpan(PasswordTxt, 2);
                Grid.SetRowSpan(PasswordLbl, 2);
            }
            else
            if (_mode == 'e')
            {
                PswdWindow.Height = 265;
                RemoveEncryptionCheck.Visibility = Visibility.Hidden;
            }

            PasswordTxt.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mode == 'e')  EncryptLogic();
            else if (_mode == 'd') DecryptLogic();
        }

        private void DecryptLogic()
        {
            try
            {
                string outFile = Encryption.DecryptFile(_inFileName, PasswordTxt.Password);


                if (File.Exists(outFile))
                {
                    if (RemoveEncryptionCheck.IsChecked == false)
                    {
                        var process = Process.Start(new ProcessStartInfo(outFile) { UseShellExecute = true });
                        process.WaitForExit();

                        File.Delete(_inFileName);

                        Encryption.EncryptFile(outFile, _inFileName, PasswordTxt.Password);

                        if (File.Exists(_inFileName))
                        {
                            File.Delete(outFile);
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo(outFile) { UseShellExecute = true });
                        File.Delete(_inFileName);
                    }
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    throw new Exception("Failed to decrypt file");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EncryptLogic()
        {
            if (PasswordTxt.Password.Length < 4)
            {
                ShowPasswordWarning("Password must be at least 4 characters");
                return;
            }

            if (PasswordTxt.Password != ConfirmPasswordTxt.Password)
            {
                ShowPasswordWarning("Passwords must match");
                return;
            }

            try
            {
                string outFileName = Path.ChangeExtension(_inFileName, ".encr");
                Encryption.EncryptFile(_inFileName, outFileName, PasswordTxt.Password);

                if (File.Exists(outFileName))
                {
                    File.Delete(_inFileName);
                    MessageBox.Show($"Successfully encrypted file {Path.GetFileName(_inFileName)} to {Path.GetFileName(outFileName)}");
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    throw new Exception("Failed to encrypt file");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPasswordWarning(string msg)
        {
            MessageBox.Show(msg, "Password Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
            PasswordTxt.Clear();
            ConfirmPasswordTxt.Clear();
            PasswordTxt.Focus();
        }
    }
}
