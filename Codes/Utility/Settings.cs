using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bamboo
{
    public class Settings
    {
        private const int MaxRecentFiles = 12;
        public static readonly Settings Default = new Settings();
        private readonly string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Bamboo), "Settings.xml");

        public Dictionary<string, string> Values { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ObservableCollection<FileSummary> RecentFiles { get; private set; } = new ObservableCollection<FileSummary>();

        public string this[string name]
        {
            get => Values.TryGetValue(name, out string value) ? value : null;
            set => Values[name] = value;
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dom = new XmlDocument();
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            dom.AppendChild(dom.CreateElement("settings"));

            var values = dom.CreateElement("values");
            dom.DocumentElement.AppendChild(values);
            foreach (var value in Values)
            {
                var valueElement = dom.CreateElement("item");
                valueElement.SetAttribute("name", value.Key);
                valueElement.InnerText = value.Value;
                values.AppendChild(valueElement);
            }

            var recentFiles = dom.CreateElement("recent_files");
            dom.DocumentElement.AppendChild(recentFiles);
            var count = 0;
            foreach (var file in RecentFiles)
            {
                var fileElement = dom.CreateElement("file");
                fileElement.SetAttribute("update_time", file.UpdateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                fileElement.InnerText = file.FileName;
                recentFiles.AppendChild(fileElement);

                count++;
                if (count >= MaxRecentFiles)
                {
                    break;
                }
            }

            using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                dom.Save(fs);
            }
        }

        public void Load()
        {
            if (File.Exists(FileName))
            {
                var dom = new XmlDocument();
                using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    dom.Load(fs);
                }

                if (dom.DocumentElement?.Name == "settings")
                {
                    foreach (var node in dom.DocumentElement.SelectNodes("values/item").OfType<XmlElement>())
                    {
                        var name = node.GetAttribute("name");
                        if (name != null)
                        {
                            Values[name] = node.InnerText;
                        }
                    }

                    foreach (var node in dom.DocumentElement.SelectNodes("recent_files/file").OfType<XmlElement>())
                    {
                        var recentFile = new FileSummary
                        {
                            FileName = node.InnerText
                        };
                        if (DateTime.TryParse(node.GetAttribute("update_time"), out DateTime updateTime))
                        {
                            recentFile.UpdateTime = updateTime;
                        }
                        else
                        {
                            recentFile.UpdateTime = DateTime.Now;
                        }

                        if (!string.IsNullOrEmpty(recentFile.FileName))
                        {
                            RecentFiles.Add(recentFile);
                        }
                    }
                }
            }
        }

        public void AddRecentFile(FileSummary swatchFile)
        {
            if (swatchFile == null && !string.IsNullOrEmpty(swatchFile.FileName))
            {
                return;
            }

            foreach (var old in RecentFiles.Where(rf => string.Equals(rf.FileName, swatchFile.FileName, StringComparison.OrdinalIgnoreCase)).ToList())
            {
                RecentFiles.Remove(old);
            }

            while (RecentFiles.Count >= MaxRecentFiles)
            {
                RecentFiles.RemoveAt(0);
            }

            RecentFiles.Add(swatchFile);
        }

        public void Remove(string item)
        {
            if (item != null)
            {
                Values.Remove(item);
            }
        }

        public string Get(string name, string defaultValue = null)
        {
            return Values.TryGetValue(name, out string value) ? value : defaultValue;
        }

        public void Set(string name, string value)
        {
            if (name != null)
            {
                Values[name] = value;
            }
        }
    }
}
