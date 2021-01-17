using System;
using System.Collections.Generic;
using System.IO;

namespace PointBlank
{
    public static class StringFilter
    {
        public static List<string> ChatFilters = new List<string>();
        public static char[] RegularExpression;
        public static void Load()
        {
            string path = "Data/Filters/Chat.txt";
            if (!File.Exists(path))
            {
                Logger.Warning($" [StringFilter] {path} no exists.");
                return;
            }
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        ChatFilters.Add(line);
                    }
                    file.Close();
                }
                Logger.Informations($" [StringFilter] Loaded {ChatFilters.Count} string filters.");
            }
            catch (Exception ex)
            {
                Logger.Error(" [StringFilter] " + ex.ToString());
            }
            RegularExpression = Settings.HasString.ToCharArray();
        }
        public static bool CheckFilterChat(string text)
        {
            foreach (string bad in ChatFilters)
            {
                if (text.Contains(bad))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckStringFilter(string text)
        {
            try
            {
                int count = 0;
                char[] textArray = text.ToUpper().ToCharArray();
                for (int i = 0; i < textArray.Length; i++)
                {
                    char textChar = textArray[i];
                    for (int j = 0; j < RegularExpression.Length; j++)
                    {
                        char regular = RegularExpression[j];
                        if (textChar == regular)
                        {
                            count++;
                        }
                    }
                }
                return text.Length == count;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return false;
            }
        }

        public static bool ContainsWhiteSpace(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}