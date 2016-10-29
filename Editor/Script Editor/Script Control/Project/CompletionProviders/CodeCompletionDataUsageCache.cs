using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    /// <summary>
    /// Tracks the names of the top-used CodeCompletionData items and gives them higher
    /// priority in the completion dropdown.
    /// </summary>
    public static class CodeCompletionDataUsageCache
    {
        private struct UsageStruct
        {
            public int Uses;
            public int ShowCount;

            public UsageStruct(int Uses, int ShowCount)
            {
                this.Uses = Uses;
                this.ShowCount = ShowCount;
            }
        }

        private static Dictionary<string, UsageStruct> s_dict;

        // File format for stored CodeCompletionDataUsageCache
        // long   magic   = 0x6567617355444343 (identifies file type = 'CCDUsage')
        // short  version = 1                  (file version)
        // int    itemCount
        // {
        //   string itemName
        //   int    itemUses
        //   int    itemShowCount
        // }

        private const long magic = 0x6567617355444343;
        private const short version = 1;

        /// <summary>Minimum number how often an item must be used to be saved to
        /// the file. Items with less uses than this count also get a priority penalty.
        /// (Because the first use would otherwise always be 100% priority)</summary>
        private const int MinUsesForSave = 2;

        public static string CacheFilename
        {
            get
            {
                return Path.Combine(Parser.ProjectParser.DomPersistencePath, "CodeCompletionUsageCache.dat");
            }
        }

        private static void LoadCache()
        {
            s_dict = new Dictionary<string, UsageStruct>();
            //ScriptControl.SolutionClosed += delegate(object sender, EventArgs e) { SaveCache(); };
            if (!File.Exists(CacheFilename))
                return;
            using (FileStream fs = new FileStream(CacheFilename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    if (reader.ReadInt64() != magic)
                    {
                        //LoggingService.Warn("CodeCompletionDataUsageCache: wrong file magic");
                        return;
                    }
                    if (reader.ReadInt16() != version)
                    {
                        //LoggingService.Warn("CodeCompletionDataUsageCache: unknown file version");
                        return;
                    }
                    int itemCount = reader.ReadInt32();
                    for (int i = 0; i < itemCount; i++)
                    {
                        string key = reader.ReadString();
                        int uses = reader.ReadInt32();
                        int showCount = reader.ReadInt32();
                        if (showCount > 1000)
                        {
                            // reduce count because the usage in the next time
                            // should have more influence on the past
                            showCount /= 3;
                            uses /= 3;
                        }
                        s_dict.Add(key, new UsageStruct(uses, showCount));
                    }
                }
            }
        }

        public static void SaveCache()
        {
            if (s_dict == null)
            {
                return;
            }
            int count;
            using (FileStream fs = new FileStream(CacheFilename, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    count = SaveCache(writer);
                }
            }
            //LoggingService.Info("Saved CodeCompletionDataUsageCache (" + count + " of " + dict.Count + " items)");
        }

        private static int SaveCache(BinaryWriter writer)
        {
            writer.Write(magic);
            writer.Write(version);
            int maxSaveItems = 50;// CodeCompletionOptions.DataUsageCacheItemCount;
            if (s_dict.Count < maxSaveItems)
            {
                writer.Write(s_dict.Count);
                foreach (KeyValuePair<string, UsageStruct> entry in s_dict)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value.Uses);
                    writer.Write(entry.Value.ShowCount);
                }
                return s_dict.Count;
            }
            else
            {
                List<KeyValuePair<string, UsageStruct>> saveItems = new List<KeyValuePair<string, UsageStruct>>();
                foreach (KeyValuePair<string, UsageStruct> entry in s_dict)
                {
                    if (entry.Value.Uses > MinUsesForSave)
                    {
                        saveItems.Add(entry);
                    }
                }
                if (saveItems.Count > maxSaveItems)
                {
                    saveItems.Sort(new SaveItemsComparer());
                }
                int count = Math.Min(maxSaveItems, saveItems.Count);
                writer.Write(count);
                for (int i = 0; i < count; i++)
                {
                    KeyValuePair<string, UsageStruct> entry = saveItems[i];
                    writer.Write(entry.Key);
                    writer.Write(entry.Value.Uses);
                    writer.Write(entry.Value.ShowCount);
                }
                return count;
            }
        }

        private class SaveItemsComparer : IComparer<KeyValuePair<string, UsageStruct>>
        {
            public int Compare(KeyValuePair<string, UsageStruct> x, KeyValuePair<string, UsageStruct> y)
            {
                double a = ((double)x.Value.Uses / x.Value.ShowCount);
                return a.CompareTo((double)y.Value.Uses / y.Value.ShowCount);
            }
        }

        public static void ResetCache()
        {
            s_dict = new Dictionary<string, UsageStruct>();
            try
            {
                if (File.Exists(CacheFilename))
                {
                    File.Delete(CacheFilename);
                }
            }
            catch
            {
                //LoggingService.Warn("CodeCompletionDataUsageCache.ResetCache(): " + ex.Message);
            }
        }

        public static double GetPriority(string dotnetName, bool incrementShowCount)
        {
            if (!CodeCompletionOptions.DataUsageCacheEnabled)
                return 0;
            if (s_dict == null)
            {
                LoadCache();
            }
            UsageStruct usage;
            if (!s_dict.TryGetValue(dotnetName, out usage))
                return 0;
            double priority = (double)usage.Uses / usage.ShowCount;
            if (usage.Uses < MinUsesForSave)
                priority *= 0.2;
            if (incrementShowCount)
            {
                usage.ShowCount += 1;
                s_dict[dotnetName] = usage;
            }
            return priority;
        }

        public static void IncrementUsage(string dotnetName)
        {
            if (!CodeCompletionOptions.DataUsageCacheEnabled)
                return;
            if (s_dict == null)
            {
                LoadCache();
            }
            UsageStruct usage;
            if (!s_dict.TryGetValue(dotnetName, out usage))
            {
                usage = new UsageStruct(0, 2);
            }
            usage.Uses += 1;
            s_dict[dotnetName] = usage;
        }
    }
}