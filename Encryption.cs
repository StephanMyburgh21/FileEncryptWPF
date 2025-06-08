using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FileEncryptWPF
{
    public class Encryption
    {
        const int SaltSize = 16;
        const int NonceSize = 12;
        const int KeySize = 32;
        const int TagSize = 16;
        const int DeriveIterations = 100000;
        const string Magic = "ENCRFILE";

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new(password, salt, DeriveIterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        public static void EncryptFile(string inputPath, string outputPath, string password)
        {
            try
            {
                string ext = Path.GetExtension(inputPath);
                byte[] plaintext = File.ReadAllBytes(inputPath);
                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
                byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
                byte[] key = DeriveKey(password, salt);

                using AesGcm aes = new(key, TagSize);
                byte[] ciphertext = new byte[plaintext.Length];
                byte[] tag = new byte[TagSize];

                aes.Encrypt(nonce, plaintext, ciphertext, tag);

                using BinaryWriter outFile = new(File.OpenWrite(outputPath));
                outFile.Write(Encoding.UTF8.GetBytes(Magic));
                outFile.Write((byte)Encoding.UTF8.GetBytes(ext).Length);
                outFile.Write(Encoding.UTF8.GetBytes(ext));
                outFile.Write(salt);
                outFile.Write(nonce);
                outFile.Write(tag);
                outFile.Write(ciphertext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string DecryptFile(string inputPath, string password)
        {
            try
            {
                using BinaryReader inFile = new BinaryReader(File.OpenRead(inputPath));
                string magic = Encoding.UTF8.GetString(inFile.ReadBytes(Magic.Length));
                if (magic != Magic) throw new Exception("Invalid file header");
                int extLen = inFile.ReadByte();
                string ext = Encoding.UTF8.GetString(inFile.ReadBytes(extLen));
                byte[] salt = inFile.ReadBytes(SaltSize);
                byte[] nonce = inFile.ReadBytes(NonceSize);
                byte[] tag = inFile.ReadBytes(TagSize);
                byte[] ciphertext = inFile.ReadBytes((int)(inFile.BaseStream.Length - inFile.BaseStream.Position));

                byte[] key = DeriveKey(password, salt);
                byte[] plaintext = new byte[ciphertext.Length];

                using AesGcm aes = new AesGcm(key, TagSize);
                aes.Decrypt(nonce, ciphertext, tag, plaintext);

                string outputPath = Path.ChangeExtension(inputPath, ext);

                File.WriteAllBytes(outputPath, plaintext);

                return outputPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }
    }
}

