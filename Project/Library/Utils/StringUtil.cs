using System;
using System.Text;

namespace PointBlank
{
    public class StringUtil : IDisposable
    {
        private static StringBuilder builder;
        public StringUtil()
        {
            builder = new StringBuilder();
        }
        public void AppendLine(string text)
        {
            builder.AppendLine(text);
        }
        public string GetString()
        {
            if (builder.Length == 0)
            {
                return builder.ToString();
            }
            return builder.Remove(builder.Length - 1, 1).ToString();
        }
        public void Dispose()
        {
            builder = null;
        }
    }
}