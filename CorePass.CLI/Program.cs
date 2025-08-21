using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CorePass.Core.Storage;
using CorePass.Core.Util;
using TextCopy;

namespace CorePass.CLI
{
    class Program
    {
        private static bool _passwordCopied = false;
        private static Task _clearClipboardTask = Task.CompletedTask;
        private static CancellationTokenSource _clearClipboardCts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                if (_passwordCopied)
                {
                    ClipboardService.SetText("");
                }
            };

            Console.Clear();
            HeaderDrawer.Draw();
            Console.WriteLine();
            Console.WriteLine("Project: github.com/flavdotexe/CorePass-MVP-CLI\n");
            Console.WriteLine();

            string vaultFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "my_vault.vault");
            var vaultStore = new VaultStore(vaultFilePath);

            Vault vault;
            string password;

            if (!System.IO.File.Exists(vaultFilePath))
            {
                Console.WriteLine("Vault not found, creating a new one...");
                while (true)
                {
                    Console.Write("Choose a password for your vault (WARNING: If you forget it, there is no recovery): ");
                    password = ReadPassword();
                    if (!string.IsNullOrEmpty(password)) break;
                    Console.WriteLine("Password cannot be empty!");
                }

                vault = new Vault();
                vaultStore.SaveVault(vault, password);
            }
            else
            {
                Console.Write("Enter your existing vault password: ");
                password = ReadPassword();

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

            string[] menuOptions = new string[] { "Add entry", "List entries", "Generate secure password", "Export vault", "Delete vault", "Help", "Exit" };
            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n===================================== Main Menu =====================================");
                Console.ResetColor();
                Console.WriteLine("Use ↑/↓ to navigate, Enter to select\n");

                for (int i = 0; i < menuOptions.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.White;
                        PrintOption((i + 1).ToString(), menuOptions[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        PrintOption((i + 1).ToString(), menuOptions[i]);
                    }
                }

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = Math.Min(menuOptions.Length - 1, selectedIndex + 1);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0:
                            AddEntry(vault, vaultStore, password);
                            break;

                        case 1:
                            ListEntriesInteractive(vault, vaultStore, password);
                            break;

                        case 2:
                            GeneratePassword();
                            break;

                        case 3:
                            ExportVault(vaultFilePath);
                            break;

                        case 4:
                            DeleteVault(vaultFilePath);
                            break;

                        case 5:
                            ShowHelp();
                            break;

                        case 6:
                            Console.Clear();
                            Console.WriteLine("Exiting...");
                            return;

                        default:
                            Console.WriteLine("Invalid option!");
                            break;
                    }
                }
            }
        }

        static void PrintOption(string number, string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{number}]");
            Console.ResetColor();
            Console.WriteLine($" {text}");
        }

        static void ShowHelp()
        {
            Console.Clear();
            HeaderDrawer.Draw();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n===================================== Help Menu =====================================");
            Console.ResetColor();
            Console.WriteLine("Below is a description of each menu option:\n");
            PrintOption("1", "Add entry: Adds a new login entry with a service name, username, and password to the vault.");
            PrintOption("2", "List entries: Displays all stored entries and allows you to search, view, edit, or delete them using arrow keys.");
            PrintOption("3", "Generate secure password: Creates a random, secure 12-character password for use in new entries.");
            PrintOption("4", "Export vault: Copies the vault file to a specified or default folder (e.g., Documents).");
            PrintOption("5", "Delete vault: Permanently deletes the vault file after confirmation. This action cannot be undone.");
            PrintOption("6", "Help: Shows this help menu with descriptions of all available options.");
            PrintOption("0", "Exit: Closes the application.");
            Console.WriteLine("\nPress ENTER to return to the main menu...");
            Console.ReadLine();
        }

        static void AddEntry(Vault vault, VaultStore vaultStore, string password)
        {
            Console.Clear();
            HeaderDrawer.Draw();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n===================================== Add Entry ====================================");
            Console.ResetColor();
            Console.WriteLine("Press Esc to cancel\n");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Service: ");
            Console.ResetColor();
            string? service = ReadInputWithCancel();
            if (service == null)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine("\nEntry creation canceled.");
                Console.WriteLine("Press ENTER to return to the main menu...");
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Username: ");
            Console.ResetColor();
            string? username = ReadInputWithCancel();
            if (username == null)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine("\nEntry creation canceled.");
                Console.WriteLine("Press ENTER to return to the main menu...");
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("Password: ");
            Console.ResetColor();
            string? pwd = ReadPasswordWithCancel();
            if (pwd == null)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine("\nEntry creation canceled.");
                Console.WriteLine("Press ENTER to return to the main menu...");
                Console.ReadLine();
                return;
            }

            vault.Entries.Add(new LoginEntry
            {
                Service = service,
                Username = username,
                Password = pwd
            });

            vaultStore.SaveVault(vault, password);
            Console.WriteLine("Entry added!");
        }

        static void ListEntriesInteractive(Vault vault, VaultStore vaultStore, string password)
        {
            if (vault.Entries.Count == 0)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine("\nNo entries found!");
                Console.WriteLine("Press ENTER to return to main menu...");
                Console.ReadLine();
                return;
            }

            int selectedIndex = 0;
            string searchQuery = "";

            while (true)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n================================== Vault Entries ===================================");
                Console.ResetColor();

                Console.WriteLine("Search: type to find service (letters only)");
                Console.WriteLine("Use ↑/↓ to navigate, Enter to select, Esc to return\n");

                var filteredEntries = vault.Entries
                    .Where(e => e.Service.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (filteredEntries.Count == 0)
                {
                    Console.WriteLine("\nNo matches found.");
                }

                for (int i = 0; i < filteredEntries.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{i + 1}. {filteredEntries[i].Service}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}. {filteredEntries[i].Service}");
                    }
                }

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = Math.Min(filteredEntries.Count - 1, selectedIndex + 1);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (filteredEntries.Count > 0 && selectedIndex >= 0 && selectedIndex < filteredEntries.Count)
                    {
                        int originalCount = filteredEntries.Count;
                        ShowEntryDetail(filteredEntries[selectedIndex], vault, vaultStore, password);
                        filteredEntries = vault.Entries
                            .Where(e => e.Service.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                        if (filteredEntries.Count == 0)
                        {
                            Console.Clear();
                            HeaderDrawer.Draw();
                            Console.WriteLine("\nNo entries found!");
                            Console.WriteLine("Press ENTER to return to main menu...");
                            Console.ReadLine();
                            return;
                        }
                        if (originalCount > filteredEntries.Count && selectedIndex > 0)
                        {
                            selectedIndex = Math.Max(0, selectedIndex - 1);
                        }
                        else if (selectedIndex >= filteredEntries.Count)
                        {
                            selectedIndex = Math.Max(0, filteredEntries.Count - 1);
                        }
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (searchQuery.Length > 0)
                    {
                        searchQuery = searchQuery.Substring(0, searchQuery.Length - 1);
                        selectedIndex = 0;
                    }
                }
                else
                {
                    char c = key.KeyChar;
                    if (char.IsLetter(c))
                    {
                        searchQuery += c;
                        selectedIndex = 0;
                    }
                }
            }
        }

        static void ShowEntryDetail(LoginEntry entry, Vault vault, VaultStore vaultStore, string password)
        {
            string[] actionOptions = new string[] { "Edit Service", "Edit Username", "Edit Password", "Show Password", "Copy Password", "Delete Entry", "Help", "Exit" };
            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine($"\nService: {entry.Service}");
                Console.WriteLine($"Username: {entry.Username}");
                Console.WriteLine($"Password: {MaskPassword(entry.Password)}");

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n===================================== Actions ======================================");
                Console.ResetColor();
                Console.WriteLine("Use ↑/↓ to navigate, Enter to select\n");

                for (int i = 0; i < actionOptions.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.White;
                        PrintOption((i + 1).ToString(), actionOptions[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        PrintOption((i + 1).ToString(), actionOptions[i]);
                    }
                }

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = Math.Min(actionOptions.Length - 1, selectedIndex + 1);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0: 
                            Console.Write("New Service: ");
                            entry.Service = Console.ReadLine() ?? entry.Service;
                            vaultStore.SaveVault(vault, password);
                            Console.WriteLine("Service updated!");
                            break;

                        case 1: 
                            Console.Write("New Username: ");
                            entry.Username = Console.ReadLine() ?? entry.Username;
                            vaultStore.SaveVault(vault, password);
                            Console.WriteLine("Username updated!");
                            break;

                        case 2: 
                            Console.Write("New Password: ");
                            entry.Password = ReadPassword();
                            vaultStore.SaveVault(vault, password);
                            Console.WriteLine("Password updated!");
                            break;

                        case 3: 
                            Console.Clear();
                            HeaderDrawer.Draw();
                            Console.WriteLine($"\nService: {entry.Service}");
                            Console.WriteLine($"Username: {entry.Username}");
                            Console.WriteLine($"Password: {entry.Password}");
                            Console.WriteLine("\nPassword will be hidden in 5 seconds...");
                            Thread.Sleep(5000);
                            break;

                        case 4: 
                            ClipboardService.SetText(entry.Password);
                            _passwordCopied = true;
                            StartClipboardClearTimer();
                            Console.Clear();
                            HeaderDrawer.Draw();
                            Console.WriteLine("\nPassword copied to clipboard!");
                            Console.WriteLine("Press ENTER to clear clipboard and continue...");
                            Console.ReadLine();
                            if (_passwordCopied)
                            {
                                ClipboardService.SetText("");
                                _passwordCopied = false;
                                _clearClipboardCts.Cancel();
                            }
                            break;

                        case 5: 
                            Console.Clear();
                            HeaderDrawer.Draw();
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write($"Are you sure you want to delete the entry for {entry.Service}? (y/N): ");
                            Console.ResetColor();
                            string confirmDelete = (Console.ReadLine() ?? "").Trim().ToLower();
                            if (confirmDelete == "y")
                            {
                                vault.Entries.Remove(entry);
                                vaultStore.SaveVault(vault, password);
                                Console.WriteLine("Entry deleted!");
                                return;
                            }
                            else
                            {
                                Console.Clear();
                                HeaderDrawer.Draw();
                                Console.WriteLine("Deletion canceled.");
                                Console.WriteLine("\nPress ENTER to continue...");
                                Console.ReadLine();
                            }
                            break;

                        case 6: 
                            ShowEntryDetailHelp();
                            break;

                        case 7: 
                            return;

                        default:
                            Console.WriteLine("Invalid option!");
                            break;
                    }
                }
            }
        }

        static void ShowEntryDetailHelp()
        {
            Console.Clear();
            HeaderDrawer.Draw();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n===================================== Help Menu =====================================");
            Console.ResetColor();
            Console.WriteLine("Below is a description of each action option:\n");
            PrintOption("1", "Edit Service: Change the service name for this entry.");
            PrintOption("2", "Edit Username: Update the username for this entry.");
            PrintOption("3", "Edit Password: Modify the password for this entry.");
            PrintOption("4", "Show Password: Display the password for 5 seconds.");
            PrintOption("5", "Copy Password: Copy the password to the clipboard.");
            PrintOption("6", "Delete Entry: Permanently delete this entry after confirmation.");
            PrintOption("7", "Help: Show this help menu with descriptions of all actions.");
            PrintOption("8", "Exit: Return to the list of entries.");
            Console.WriteLine("\nPress ENTER to return to the actions menu...");
            Console.ReadLine();
        }

        static void GeneratePassword()
        {
            Console.Clear();
            HeaderDrawer.Draw();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n=============================== CorePass Generator =================================");
            Console.ResetColor();
            string newPassword = SecurityHelper.GeneratePassword(12);
            Console.WriteLine($"Generated password: {MaskPassword(newPassword)}");
            Console.WriteLine("\nPress S to show password, C to copy to clipboard, Enter to return...");

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    HeaderDrawer.Draw();
                    return;
                }
                else if (key.Key == ConsoleKey.S)
                {
                    Console.Clear();
                    HeaderDrawer.Draw();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n=============================== CorePass Generator =================================");
                    Console.ResetColor();
                    Console.WriteLine($"Generated password: {newPassword}");
                    Console.WriteLine("\nPassword will be hidden in 5 seconds...");
                    Thread.Sleep(5000);
                    Console.Clear();
                    HeaderDrawer.Draw();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n=============================== CorePass Generator =================================");
                    Console.ResetColor();
                    Console.WriteLine($"Generated password: {MaskPassword(newPassword)}");
                    Console.WriteLine("\nPress S to show password, C to copy to clipboard, Enter to return...");
                }
                else if (key.Key == ConsoleKey.C)
                {
                    ClipboardService.SetText(newPassword);
                    _passwordCopied = true;
                    StartClipboardClearTimer();
                    Console.Clear();
                    HeaderDrawer.Draw();
                    Console.WriteLine("\nPassword copied to clipboard!");
                    Console.WriteLine("Press ENTER to clear clipboard and continue...");
                    Console.ReadLine();
                    if (_passwordCopied)
                    {
                        ClipboardService.SetText("");
                        _passwordCopied = false;
                        _clearClipboardCts.Cancel();
                    }
                    Console.Clear();
                    HeaderDrawer.Draw();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n=============================== CorePass Generator =================================");
                    Console.ResetColor();
                    Console.WriteLine($"Generated password: {MaskPassword(newPassword)}");
                    Console.WriteLine("\nPress S to show password, C to copy to clipboard, Enter to return...");
                }
            }
        }

        static void StartClipboardClearTimer()
        {
            _clearClipboardCts.Cancel();
            _clearClipboardCts = new CancellationTokenSource();

            _clearClipboardTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(30000, _clearClipboardCts.Token);
                    ClipboardService.SetText("");
                    _passwordCopied = false;
                }
                catch (TaskCanceledException)
                {
                    // Timer was canceled, do nothing
                }
            });
        }

        static string MaskPassword(string password)
        {
            return new string('*', password.Length);
        }

        static void ExportVault(string vaultFilePath)
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.Write($"Choose destination folder or press ENTER for default ({defaultPath}): ");
            string? input = Console.ReadLine();
            string targetFolder = string.IsNullOrWhiteSpace(input) ? defaultPath : input;

            try
            {
                string targetPath = System.IO.Path.Combine(targetFolder, "my_vault.vault");
                System.IO.File.Copy(vaultFilePath, targetPath, overwrite: true);

                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine($"\nVault exported successfully to {targetPath}");
                Console.WriteLine("\nPress ENTER to return to the main menu...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Clear();
                HeaderDrawer.Draw();
                Console.WriteLine($"\nError exporting vault: {ex.Message}");
                Console.WriteLine("\nPress ENTER to return to the main menu...");
                Console.ReadLine();
            }
        }

        static void DeleteVault(string vaultFilePath)
        {
            Console.Write("Are you sure you want to DELETE the entire vault? This cannot be undone! (y/N): ");
            string confirmDelete = (Console.ReadLine() ?? "").Trim().ToLower();
            if (confirmDelete == "y")
            {
                if (System.IO.File.Exists(vaultFilePath))
                {
                    System.IO.File.Delete(vaultFilePath);
                    Console.Clear();
                    Console.WriteLine("Vault deleted successfully!");
                    Environment.Exit(0);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("No vault found to delete.");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Operation canceled.");
            }
        }

        static string? ReadInputWithCancel()
        {
            string input = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        static string? ReadPasswordWithCancel()
        {
            string password = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return password;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += key.KeyChar;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("*");
                    Console.ResetColor();
                }
            }
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += key.KeyChar;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("*");
                    Console.ResetColor();
                }
            }

            return password;
        }
    }
}