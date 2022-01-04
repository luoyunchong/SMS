using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace IGeekFan.SMS.Core
{
    public class SecurityUtil
    {
        public static string Md5(string s)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                string result = BitConverter.ToString(md5.ComputeHash(bytes));
                return result.Replace("-", "");
            }
        }
    }
}
