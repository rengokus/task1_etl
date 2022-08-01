using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using task1.model;
using task1.model1;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace task1
{
    public class Main
    {
        private readonly FileSystemWatcher watcher;
        private readonly DirectoryInfo source;
        private readonly string[] extensions = { ".txt" };
        private readonly List<string> processedFiles;
        private readonly string pathB = ConfigurationManager.AppSettings["PathB"];

        public Main()
        {
            processedFiles = new List<string>();
            source = new DirectoryInfo(ConfigurationManager.AppSettings["PathA"]);
            watcher = new FileSystemWatcher()
            {
                Path = source.FullName,
                Filter = "*.*"
            };
            watcher.Created += WatcherCreated;
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            var files = source.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower()) && !processedFiles.Contains(f.Name)).ToList();
            

            files.ForEach(f =>
            {
                var results = new ConcurrentDictionary<string, City>();
                Parallel.ForEach(File.ReadLines(f.FullName), line =>
                {
                    var strings = line.Split(',').Select(s => s.Trim()).ToList();
                    string city = strings[2];
                    if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'"))
                    {
                        city = city.Remove(0, 1);
                        results.TryAdd(city, new City());
                        results[city].Add(strings);
                    }
                });
                
                string now = DateTime.Now.ToString("MM-dd-yyyy");
                var di = Directory.CreateDirectory($"{pathB}\\{now}");
                var res = results.Select(r => new 
                { 
                    city = r.Key, 
                    services = r.Value.Services.Values, 
                    total = CalculateTotalForCity(r.Value.Services.Values) 
                });

                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    }
                };
                string json = JsonConvert.SerializeObject(res, new JsonSerializerSettings 
                { 
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
                using (var sw = File.CreateText($"{di.FullName}\\output{di.GetFiles().Length}.json"))
                {
                    sw.WriteLine(json);
                }
            });
            processedFiles.AddRange(files.Select(f => f.Name));
        }

        private decimal CalculateTotalForCity(ICollection<Service1> services)
        {
            decimal res = 0;
            services.ToList().ForEach(s => res += s.Total);
            return res;
        }

        private void WatcherCreated3(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(processedFiles.Count);
            var files = source.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower()) && !processedFiles.Contains(f.Name)).ToList();
            Console.WriteLine(files.Count);
            

            foreach (var file in files)
            {
                var results = new ConcurrentDictionary<string, City>();
                foreach (var line in File.ReadLines(file.FullName))
                {
                    var strings = line.Split(',').Select(s => s.Trim()).ToList();
                    string city = strings[2];
                    if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'"))
                    {
                        city = city.Remove(0, 1);
                        results.TryAdd(city, new City());
                        results[city].Add(strings);
                    }
                }
                string now = DateTime.Now.ToString("MM-dd-yyyy");
                var di = Directory.CreateDirectory($"{pathB}\\{now}");
                //string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
                string json = JsonConvert.SerializeObject(results, Formatting.Indented);

                using (var sw = File.CreateText($"{di.FullName}\\output{di.GetFiles().Length}.json"))
                {
                    sw.WriteLine(json);
                }
            }
            processedFiles.AddRange(files.Select(f => f.Name));

            processedFiles.ForEach(Console.WriteLine);
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }
    }
}
