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
            string[] args = e.Args;

            if (args is null || args.Length < 2 || (args[1] != "e" && args[1] != "d"))
            {
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (Path.GetExtension(args[0]) == ".encr" && args[1] == "e")
            {
                MessageBox.Show("File is already encrypted", "Cannot Encrypt", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (Path.GetExtension(args[0]) != ".encr" && args[1] == "d")
            {
                MessageBox.Show("File is not encrypted", "Cannot Decrypt", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.GetCurrentProcess().Kill();
                return;
            }

            PasswordWindow p = new(args[1][0], args[0]);
            p.Show(); 
        }
    }

}
