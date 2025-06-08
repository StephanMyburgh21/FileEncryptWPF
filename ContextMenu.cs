using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace FileEncryptWPF
{
    public static class ContextMenu
    {
        public static void AddEncryptToContextMenu()
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                string menuName = "Encrypt";
                string registryPath = $@"*\shell\{menuName}";
                string commandSubKeyPath = $@"{registryPath}\command";
                string arg = "e";

                string expectedCommand = $"\"{appPath}\" \"%1\" \"{arg}\"";

                using (RegistryKey existingCommandKey = Registry.ClassesRoot.OpenSubKey(commandSubKeyPath, writable: false))
                {
                    string? existingCommand = existingCommandKey?.GetValue("") as string;

                    if (existingCommand != null)
                    {
                        if (string.Equals(existingCommand, expectedCommand, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                        Registry.ClassesRoot.DeleteSubKeyTree(registryPath, throwOnMissingSubKey: false);
                    }
                }

                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(registryPath))
                {
                    key?.SetValue("", menuName);

                    using (RegistryKey commandKey = key?.CreateSubKey("command"))
                    {
                        commandKey?.SetValue("", expectedCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to Create Context Menu For Encrypt", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void AddDecryptToContextMenu()
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                string menuName = "Decrypt";
                string registryPath = $@"*\shell\{menuName}";
                string commandSubKeyPath = $@"{registryPath}\command";
                string arg = "d";

                string expectedCommand = $"\"{appPath}\" \"%1\" \"{arg}\"";

                using (RegistryKey existingCommandKey = Registry.ClassesRoot.OpenSubKey(commandSubKeyPath, writable: false))
                {
                    string? existingCommand = existingCommandKey?.GetValue("") as string;

                    if (existingCommand != null)
                    {
                        if (string.Equals(existingCommand, expectedCommand, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                        Registry.ClassesRoot.DeleteSubKeyTree(registryPath, throwOnMissingSubKey: false);
                    }
                }

                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(registryPath))
                {
                    key?.SetValue("", menuName);

                    using (RegistryKey commandKey = key?.CreateSubKey("command"))
                    {
                        commandKey?.SetValue("", expectedCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to Create Context Menu For Decrypt", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}
