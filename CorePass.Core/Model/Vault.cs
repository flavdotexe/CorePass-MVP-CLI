using System.Collections.Generic;

namespace CorePass.Core.Storage
{
    public class Vault
    {
        public List<LoginEntry> Entries { get; set; } = new List<LoginEntry>();
    }

    public class LoginEntry
    {
        public string Service { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
