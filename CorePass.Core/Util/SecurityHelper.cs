using System;
using System.Text;

namespace CorePass.Core.Util
{
    public static class SecurityHelper
    {
        private static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890ãäâáàõôöóòêéèûüúùîíìïÃÂÄÁÀÔÖÕÎÍÌÏçÇñÑ!@#$%^&*()_+-=[]{}|;:,.<>?".ToCharArray();

        public static string GeneratePassword(int length)
        {
            var random = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }
}
