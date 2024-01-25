using System;
using System.Text;

namespace TicTacToe.Services.Analytics
{
    public static class ToStringUtil
    {
        private static readonly StringBuilder Sb;

        static ToStringUtil()
        {
            Sb = new StringBuilder();
        }

        public static string ToLowerSnakeCase(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length < 2)
            {
                return text.ToLowerInvariant();
            }

            Sb.Clear();
            Sb.Append(char.ToLowerInvariant(text[0]));
            for (var i = 1; i < text.Length; ++i)
            {
                var c = text[i];
                if (char.IsUpper(c))
                {
                    Sb.Append('_').Append(char.ToLowerInvariant(c));
                }
                else
                {
                    Sb.Append(c);
                }
            }

            return Sb.ToString();
        }
    }
}