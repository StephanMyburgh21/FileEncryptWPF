using System.IO;
using System.Diagnostics;
using System.Windows;

namespace FileEncryptWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var args = e.Args;

            if (args is not [_, "e" or "d"])
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            string filePath = args[0];
            string mode = args[1];
            string ext = Path.GetExtension(filePath);

            if (ext == ".encr" && mode == "e")
            {
                MessageBox.Show("File is already encrypted", "Cannot Encrypt", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (ext != ".encr" && mode == "d")
            {
                MessageBox.Show("File is not encrypted", "Cannot Decrypt", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.GetCurrentProcess().Kill();
                return;
            }

            new PasswordWindow(mode[0], filePath).Show();

        }
    }

}
