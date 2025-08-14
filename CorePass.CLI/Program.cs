using System;
using CorePass.Core.Storage;
using CorePass.Core.Util;

namespace CorePass.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Banner
            Console.WriteLine("============================= CorePass CLI Version 1.0 =============================");
            Console.WriteLine(@"
+==================================================================================+
|    █████▒██▓    ▄▄▄    ██▒   █▓█████▄  ▒█████  ▄▄▄█████▓▓█████ ▒██   ██▒▓█████   |
|  ▓██   ▒▓██▒   ▒████▄ ▓██░   █▒██▀ ██▌▒██▒  ██▒▓  ██▒ ▓▒▓█   ▀ ▒▒ █ █ ▒░▓█   ▀   |
|  ▒████ ░▒██░   ▒██  ▀█▄▓██  █▒░██   █▌▒██░  ██▒▒ ▓██░ ▒░▒███   ░░  █   ░▒███     |
|  ░▓█▒  ░▒██░   ░██▄▄▄▄██▒██ █░░▓█▄   ▌▒██   ██░░ ▓██▓ ░ ▒▓█  ▄  ░ █ █ ▒ ▒▓█  ▄   |
|  ░▒█░   ░██████▒▓█   ▓██▒▒▀█░ ░▒████▓ ░ ████▓▒░  ▒██▒ ░ ░▒████▒▒██▒ ▒██▒░▒████▒  |
|   ▒ ░   ░ ▒░▓  ░▒▒   ▓▒█░░ ▐░  ▒▒▓  ▒ ░ ▒░▒░▒░   ▒ ░░   ░░ ▒░ ░▒▒ ░ ░▓ ░░░ ▒░ ░  |
|   ░     ░ ░ ▒  ░ ▒   ▒▒ ░░ ░░  ░ ▒  ▒   ░ ▒ ▒░     ░     ░ ░  ░░░   ░▒ ░ ░ ░  ░  |
|   ░ ░     ░ ░    ░   ▒     ░░  ░ ░  ░ ░ ░ ░ ▒    ░         ░    ░    ░     ░     |
|             ░  ░     ░  ░   ░    ░        ░ ░              ░  ░ ░    ░     ░  ░  |
|                            ░   ░   H̷e̷l̷l̷ø̷ ̷W̷o̷r̷l̷d̷!̷                                  |
+==================================================================================+
");

            Console.WriteLine("“Privacy is the soul’s shield; without it, freedom is merely a mirage that power crosses without asking permission.”");
            Console.WriteLine("Author: https://github.com/flavdotexe\n");

            // Define caminho fixo do vault (fora do bin/Release)
            string vaultFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "my_vault.vault");
            var vaultStore = new VaultStore(vaultFilePath);

            Vault vault;
            string password;

            // Verifica se existe vault
            if (!System.IO.File.Exists(vaultFilePath))
            {
                Console.WriteLine("Vault not found, creating a new one...");
                while (true)
                {
                    Console.Write("Choose a password for your vault (WARNING: If you forget it, there is no recovery): ");
                    password = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(password)) break;
                    Console.WriteLine("Password cannot be empty!");
                }

                vault = new Vault();
                vaultStore.SaveVault(vault, password);
            }
            else
            {
                Console.Write("Enter your existing vault password: ");
                password = Console.ReadLine() ?? "";

                try
                {
                    vault = vaultStore.LoadVault(password);
                    Console.WriteLine("Vault loaded successfully!");
                }
                catch
                {
                    Console.WriteLine("Incorrect password or corrupted vault!");
                    return;
                }
            }

            // Loop principal do menu
            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1 - Add entry");
                Console.WriteLine("2 - List entries");
                Console.WriteLine("3 - Generate secure password");
                Console.WriteLine("4 - Export vault");
                Console.WriteLine("5 - Delete vault");
                Console.WriteLine("0 - Exit");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        Console.Write("Service: ");
                        string service = Console.ReadLine() ?? "";
                        Console.Write("Username: ");
                        string username = Console.ReadLine() ?? "";
                        Console.Write("Password: ");
                        string pwd = Console.ReadLine() ?? "";

                        vault.Entries.Add(new LoginEntry
                        {
                            Service = service,
                            Username = username,
                            Password = pwd
                        });

                        vaultStore.SaveVault(vault, password);
                        Console.WriteLine("Entry added!");
                        break;

                    case "2":
                        Console.WriteLine("\n============================= Vault Entries =============================");
                        foreach (var e in vault.Entries)
                        {
                            Console.WriteLine($"{e.Service} | {e.Username} | {e.Password}");
                        }
                        break;

                    case "3":
                        string newPassword = SecurityHelper.GeneratePassword(16);
                        Console.WriteLine($"Generated password: {newPassword}");
                        break;

                    case "4":
                        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        Console.Write($"Choose destination fold {defaultPath}): ");
                        string? input = Console.ReadLine();
                        string targetFolder = string.IsNullOrWhiteSpace(input) ? defaultPath : input;

                        try
                        {
                            string targetPath = System.IO.Path.Combine(targetFolder, "my_vault.vault");
                            System.IO.File.Copy(vaultFilePath, targetPath, overwrite: true);
                            Console.WriteLine($"Vault exported successfully to: {targetPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error exporting vault: {ex.Message}");
                        }
                        break;

                    case "5":
                        Console.Write("Are you sure you want to DELETE the entire vault? This will remove the file and everything will be lost! (y/N): ");
                        string confirmDelete = (Console.ReadLine() ?? "").Trim().ToLower();
                        if (confirmDelete == "y")
                        {
                            if (System.IO.File.Exists(vaultFilePath))
                            {
                                System.IO.File.Delete(vaultFilePath);
                                Console.WriteLine("Vault deleted successfully!");
                                Environment.Exit(0);
                            }
                            else
                            {
                                Console.WriteLine("No vault found to delete.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Operation canceled.");
                        }
                        break;

                    case "0":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }
    }
}
