       +==================================================================================+tm
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

# CorePass CLI V1.0 - Security and Warnings

##  About Encryption

CorePass uses **AES-256 (Advanced Encryption Standard)** in **CBC (Cipher Block Chaining)** mode with **PKCS7 Padding**,
deriving the key from the user's password via **PBKDF2** (`Rfc2898DeriveBytes`) with:

- **Iterations:** 100,000  
- **Key size:** 256 bits  
- **IV size:** 128 bits  
- **Hash algorithm:** SHA-256

This process ensures that even short passwords are strengthened through derivation, making brute force attacks more difficult.

====================================================================================================

## Important Warnings

- It is **NOT** recommended to use CorePass to store **seed phrases** or private keys for cryptocurrencies or credit card.
- If you choose to use it for this purpose, **do so at your own risk**.
- The data in my_vault.vault is secure and encrypted offline, but if you lose the file, you lose everything.
- Be careful with keyloggers. If the data is very sensitive, use a clean VM or TailsOS.
- **Always** keep a secure physical backup of all logins and critical data.
- Remember: ultimate security also depends on the strength of your master password.

====================================================================================================

## Requirements

**.NET 8 SDK**

**Arch**: 
`sudo pacman -S dotnet-sdk-8.0`

**Debian/Ubuntu**:
`sudo apt-get update && sudo apt-get install -y wget apt-transport-https software-properties-common` 
`wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb`
`sudo dpkg -i packages-microsoft-prod.deb`
`rm packages-microsoft-prod.deb`
`sudo apt-get update`
`sudo apt-get install -y dotnet-sdk-8.0`

**Windows**: 
==https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.413-windows-x64-installer==
 

- Terminal with **UTF-8** encoding to ensure compatibility with special Latin characters (e.g.: `ñ, ç, ã, ü`)  
- On **Linux**, make sure that the `LANG` or `LC_ALL` environment variable is set to UTF-8:
`export LANG=en_US.UTF-8

====================================================================================================

##️ Running on Linux
Edit CorePass.sh file with Notepad, adjust path ("$HOME/your/data/path/CorePass-MVP-CLI/CorePass.CLI/CorePass.CLI.csproj") with your data.

## Runing on Windows
Edit CorePass.bat file with Notepad, adjust path ("PROJECT_PATH=C:\Users\your\path\data\CorePass-MVP-CLI\CorePass.CLI\CorePass.CLI.csproj" 
with your data.

## License

Free for personal or educational use. I'm not responsible for your data loss. You are responsible for your data, so do responsibly and freely.

`chmod +x CorePass.sh`

`./CorePass.SH`



"That the only permission that slows us down is sudo."
