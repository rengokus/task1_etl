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
using System.Threading;

namespace task1
{
    public class Main
    {
        public FileSystemWatcher Watcher { get; init; }
        public DirectoryInfo Source { get; init; } 
        public string[] Extensions { get; init; }
        public List<string> ProcessedFiles { get; init; }
        public string PathB { get; init; }
        public long ParsedFiles { get; private set; }
        public long ParsedLines { get; private set; }
        public long FoundErrors { get; private set; }
        public List<string> InvalidFiles { get; init; }
        public System.Timers.Timer Timer { get; init; }
        public TimeSpan TimeSpan
        {
            get => DateTime.Now.AddSeconds(5) - DateTime.Now;   
        }

        public Main()
        {
            Extensions = new string[] { ".txt", ".csv" };
            ProcessedFiles = new List<string>();
            Source = new DirectoryInfo(ConfigurationManager.AppSettings["PathA"]);
            InvalidFiles = new List<string>();
            PathB = ConfigurationManager.AppSettings["PathB"];
            Watcher = new FileSystemWatcher()
            {
                Path = Source.FullName,
                Filter = "*.*"
            };
            Watcher.Created += WatcherCreated;
            Timer = new System.Timers.Timer();
            Timer.Interval = TimeSpan.TotalMilliseconds;
            Timer.Elapsed += Timer_Elapsed;
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            var files = Source.GetFiles().Where(f => Extensions.Contains(f.Extension.ToLower()) && !ProcessedFiles.Contains(f.Name)).ToList();
            

            files.ForEach(f =>
            {
                try
                {
                    var results = new ConcurrentDictionary<string, City>();
                    long count = 0;
                    Parallel.ForEach(File.ReadLines(f.FullName), line =>
                    {
                        
                        var strings = line.Split(',').Select(s => s.Trim()).ToList();
                        string city = strings[2];
                        if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'") && strings.Count == 9)
                        {
                            city = city.Remove(0, 1);
                            results.TryAdd(city, new City());
                            results[city].Add(strings);
                        }
                        else
                        {
                            FoundErrors++;
                            InvalidFiles.Add(f.FullName);
                        }
                        Interlocked.Increment(ref count);
                    });
                    ParsedLines += count;
                    string now = DateTime.Now.ToString("MM-dd-yyyy");
                    var di = Directory.CreateDirectory($"{PathB}\\{now}");
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
                    using (var sw = File.CreateText($"{di.FullName}\\output{di.GetFiles().Length + 1}.json"))
                    {
                        sw.WriteLine(json);
                    }
                    ParsedFiles++;
                }
                catch (Exception ex)
                {
                    FoundErrors++;
                    InvalidFiles.Add(f.FullName);
                }
                
            });
            ProcessedFiles.AddRange(files.Select(f => f.Name));
        }

        private decimal CalculateTotalForCity(ICollection<Service1> services)
        {
            decimal res = 0;
            services.ToList().ForEach(s => res += s.Total);
            return res;
        }

        public void Start()
        {
            Watcher.EnableRaisingEvents = true;
            Timer.Start();
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Interval = TimeSpan.TotalMilliseconds;
            string now = DateTime.Now.ToString("MM-dd-yyyy");
            if (Directory.Exists($"{PathB}\\{now}"))
            {
                var di = Directory.CreateDirectory($"{PathB}\\{now}");
                using (var sw = File.CreateText($"{di.FullName}\\meta.log"))
                {
                    sw.WriteLine($"parsed_files: {ParsedFiles}");
                    sw.WriteLine($"parsed_lines: {ParsedLines}");
                    sw.WriteLine($"found_errors: {FoundErrors}");
                    sw.WriteLine($"invalid_files: {string.Join(", ", InvalidFiles)}");
                }
            }
        }

        public void Stop()
        {
            
        }

        
    }
}
