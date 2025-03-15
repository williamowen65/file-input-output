using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("\n\nWelcome to an encryption/decryption tool for your text files...\n\n");

        while (handleApp()) {
    
           
        }
    }

    static bool handleApp()
    {
        Console.WriteLine("Enter the path of the file to import (or press 'q' to quit):");
        string inputFilePath = Console.ReadLine();

        if (inputFilePath == "q")
        {
            return false;
        }

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("File not found.");
            return true;
        }

        string fileContent = File.ReadAllText(inputFilePath);
        Console.WriteLine("File imported successfully.");

        string resultContent = string.Empty;
        Console.WriteLine("Choose an option: (1) Encrypt (2) Decrypt");
        while (resultContent == string.Empty)
        {
            string option = Console.ReadLine();
            if (option == "1")
            {
                resultContent = Encrypt(fileContent);
                Console.WriteLine("File encrypted successfully.");
            }
            else if (option == "2")
            {
                resultContent = Decrypt(fileContent);
                Console.WriteLine("File decrypted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid option.");
                continue;
            }
        }

        Console.WriteLine("Enter the path to save the result file:");
        string outputFilePath = Console.ReadLine();
        File.WriteAllText(outputFilePath, resultContent);
        Console.WriteLine("File saved successfully!!");
        Console.WriteLine("\n\n");

        return true;
    }

    static string Encrypt(string plainText)
    {
        byte[] key = Encoding.UTF8.GetBytes("A very strong ke"); // Key should be 16, 24 or 32 bytes for AES
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV();
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] iv = new byte[16];
        byte[] cipher = new byte[fullCipher.Length - iv.Length];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        byte[] key = Encoding.UTF8.GetBytes("A very strong ke"); // Key should be 16, 24 or 32 bytes for AES
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipher))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
