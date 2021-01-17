using System;
using System.Collections.Generic;
using System.IO;

namespace PointBlank
{
    public static class AddressFilter
    {
        public static List<string> filters = new List<string>();
        public static void Load()
        {
            string path = "Data/Filters/Address.txt";
            if (!File.Exists(path))
            {
                Logger.Warning($" [AddressFilter] {path} no exists.");
                return;
            }
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        filters.Add(line);
                    }
                    file.Close();
                }
                Logger.Informations($" [AddressFilter] Loaded {filters.Count} string filters.");
            }
            catch (Exception ex)
            {
                Logger.Error(" [AddressFilter] " + ex.ToString());
            }
        }

        public static void Reload()
        {
            lock (filters)
            {
                filters.Clear();
                Load();
            }
        }
    }
}