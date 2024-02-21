using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WpfApp1
{
    static class SecurityAuth
    {
        public static string hashPassword(string password)
        {
            MD5 mD5 = MD5.Create();

            byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = mD5.ComputeHash(bytes);

            return $"{Convert.ToHexString(hash)}";
        }
    }
}
