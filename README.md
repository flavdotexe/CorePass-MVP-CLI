      =================================== CorePass CLI v2.0 ==================================
       ______________________________________________________________________________________
        _______________   _________________________________________________   ______________
        RedZ1@viA23$?@3| |                                                 | |3$erRfCsA-X!&6
        aScs@3bÑdff@109| |  ___|                     _                    | |dR4Tg%5ãCxç]0!
        ÃcvSv*d2bj#f5%t| | |       _     __|  _   |   |  _` |   __|   __|| |@dcCvdfõ2fá9W1
        B12f;+F$Nãoc30?| | |      (   |  |     __/  ___/  (   | __  __ | |0&rCb?c>[$cXÔi
        cFv2&tg0áô0?[!d| |____| ___/  _|   ___| _|    __,_| ____/ ____/| |42tGv6üzÇ)2SxG
        s͟6͟Ü͟v͟@͟x͟S͟z͟!͟4͟õ͟P͟l͟s͟$͟| |_________________________________________________| |3͟%͟f͟Cx͟Z͟c͟s͟k͟a͟#͟Ñ͟)͟!͟
        _____________________________________________________________________________________
   
   # CorePass CLI V1.1 - Your Hardcore Password Manager
   
   CorePass CLI is a secure, offline, and lightweight password manager built for those who want 
   security without the fluff. Store your logins, generate bulletproof passwords, and manage
   everything with a slick terminal interface—all while keeping your data locked down with 
   military-grade encryption. No internet, no nonsense, just pure *hardcore* password management.
   
   ## Features
   
   - **Interactive Menu**: Navigate with arrow keys and `Enter` through options like Add Entry,
   list Entries, Generate Secure Password, Export Vault, Delete Vault, Help, and Exit.
   - **Add Entry**: Store service name, username, and password in your encrypted vault. Press 
   `Esc` to cancel anytime.
   - **List Entries**: View all entries with a searchable list. Type letters to filter services in
   real-time, use arrows to select, and dive into actions for each entry.
   - **Entry Actions**: For each entry, you can:
     - Edit Service, Username, or Password.
     - Show Password (visible for 5 seconds before hiding).
     - Copy Password to clipboard (cleared when you press `Enter` or exit the program).
     - Delete Entry with confirmation.
     - Access a Help menu with action descriptions.
   - **Generate Secure Password**: Create a random 12-character password with a mix of letters, numbers, and symbols. Show it (`S`), copy to clipboard (`C`), or return (`Enter`).
   - **Export Vault**: Copy your encrypted `my_vault.vault` file to a chosen folder (defaults to Documents).
   - **Delete Vault**: Permanently delete the vault file with confirmation—use with caution!
   - **Secure Input**: Passwords are masked with `********` in the terminal and never appear in command history.
   - **Clipboard Security**: Passwords copied to the clipboard are cleared when you press `Enter` (after "Password copied to clipboard! Enter to clear clipboard and return...") or when the program exits.
   
   ## Encryption
   
   CorePass uses **AES-256** in **CBC** (Cipher Block Chaining) mode with **PKCS7 Padding**, ensuring your data is locked tight. The encryption key is derived from your master password using **PBKDF2** (`Rfc2898DeriveBytes`) with:
   - **Iterations**: 100,000
   - **Key size**: 256 bits
   - **IV size**: 128 bits
   - **Hash algorithm**: SHA-256
   
   Everything is **00% local and offline**, so your data never touches the internet. The built-in password generator creates military-grade passwords to keep your accounts secure.
   
   ## Security Warnings
   
   - **Do NOT store seed phrases or private keys** for cryptocurrencies or credit card details in CorePass. If you choose to do so, it’s at your own risk.
   - **Vault File Safety**: The `my_vault.vault` file is encrypted, but if you lose it, your data is gone. Keep a secure backup!
   - **Keyloggers**: Be cautious of keyloggers, especially for sensitive data. Consider using a clean virtual machine or TailsOS for maximum security.
   - **Master Password**: Your vault’s security depends on a strong master password. Choose wisely and memorize it—there’s no recovery option.
   - **Physical Backups**: Always maintain a secure physical backup of your logins and critical data.
   - **Clipboard**: Passwords are cleared from the clipboard after you press `Enter` or exit the program, minimizing exposure.
   
   ## Requirements
   
   - **.NET 8 SDK** (required to run the program).
   - **UTF-8 Terminal**: Ensure your terminal supports UTF-8 for compatibility with special Latin characters (e.g., `ñ`, `ç`, `ã`, `ü`).
   
   ### Linux Installation
   
   For **Arch**:
```
sudo pacman -S dotnet-sdk-8.0
```
For **Debian/Ubuntu**:
```
sudo apt-get update && sudo apt-get install -y wget apt-transport-https software-properties-common
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

Set UTF-8 encoding:

```bash
export LANG=en_US.UTF-8
```

### Windows Installation

- Download and install the .NET 8 SDK: Download .NET 8 SDK.

## Running on Linux

1. Clone the repository:

- [ ] git clone https://github.com/flavdotexe/CorePass-MVP-CLI.git
  cd CorePass-MVP-CLI

1. Edit `CorePass.sh` with a text editor (e.g., `nano`):

   ```bash
   nano CorePass.sh
   ```

   Update the `PROJECT_PATH` to point to your `CorePass.CLI.csproj` file, e.g.:

   ```bash
   PROJECT_PATH="/home/your/path/CorePass-MVP-CLI/CorePass.CLI/CorePass.CLI.csproj"
   ```

2. Make the script executable:

   ```bash
   chmod +x CorePass.sh
   ```

3. Run the script:

   ```bash
   ./CorePass.sh
   ```

**Note**: The `my_vault.vault` file is created in the directory where you run the script (the *working directory*). To use an existing vault, ensure you run the script from the directory containing `my_vault.vault` or move the vault file to your current directory.

## Running on Windows

1. Clone the repository:

   ```bash
   git clone https://github.com/flavdotexe/CorePass-MVP-CLI.git
   cd CorePass-MVP-CLI
   ```

2. Edit `CorePass.bat` with Notepad: Update the `PROJECT_PATH` to point to your `CorePass.CLI.csproj` file, e.g.:

   ```batch
   set PROJECT_PATH=C:"Users\your\path\CorePass-MVP-CLI-main\CorePass.CLI\CorePass.CLI.csproj"
   ```

3. Run the batch file:

   ```batch
   CorePass.bat
   ```

**Note**: Like on Linux, the `my_vault.vault` file is created in the directory where you run the batch file. Ensure the vault file is in the same directory or move it there to use an existing vault.

## License

Free for personal or educational use. I’m not responsible for your data loss. You are responsible for your data, so use CorePass responsibly and freely.

"That the only permission that slows us down is sudo."
