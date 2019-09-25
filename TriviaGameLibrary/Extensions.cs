using System;
using System.Text;

namespace TriviaGameLibrary
{
    public static class Extensions
    {
        public static string DecodeBase64(this string input)
        {
            var bits = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bits);
        }
    }
}