using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using task1.model;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading;
using System.Globalization;
using LINQtoCSV;

namespace task1
{
    public class Main
    {
        public FileSystemWatcher Watcher { get; init; }
        public DirectoryInfo Source { get; init; }
        public string[] Extensions { get; init; }
        public List<string> ProcessedFiles { get; init; }
        public string PathB { get; init; }
        public long ParsedLines { get; private set; }
        public long FoundErrors { get; private set; }
        public List<string> InvalidFiles { get; init; }
        public System.Timers.Timer Timer { get; init; }
        public TimeSpan TimeSpan
        {
            get => DateTime.Today.AddDays(1) - DateTime.Now;
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
            Timer.Elapsed += TimerElapsed;
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            var files = Source.GetFiles().Where(f => Extensions.Contains(f.Extension.ToLower()) && !ProcessedFiles.Contains(f.Name)).ToList();


            files.ForEach(f =>
            {
                var results = new ConcurrentDictionary<string, City>();
                if (f.Name.EndsWith(".csv"))
                {
                    var csvFileDescription = new CsvFileDescription
                    {
                        FirstLineHasColumnNames = true,
                        IgnoreUnknownColumns = true,
                        SeparatorChar = ',',
                        UseFieldIndexForReadingData = false,
                        FileCultureName = "en"
                    };
                    long count = 0;
                    try
                    {
                        var csvContext = new CsvContext();
                        var csvContent = csvContext.Read<Result>(f.FullName, csvFileDescription);
                        
                        Parallel.ForEach(csvContent, (r, state) =>
                        {
                            Interlocked.Increment(ref count);
                            DateTime date;
                            if (DateTime.TryParseExact(r.Date, "yyyy-dd-MM", null, DateTimeStyles.None, out date))
                            {
                                string city = r.Address.Substring(0, r.Address.IndexOf(','));
                                results.TryAdd(city, new City());
                                results[city].Add(r.FirstName + " " + r.LastName, r.Payment, r.Service, r.AccountNumber, date);
                            }
                            else
                            {
                                if (!InvalidFiles.Contains(f.FullName))
                                    InvalidFiles.Add(f.FullName);

                                state.Stop();
                            }
                            
                            
                        });
                    }
                    catch (AggregatedException)
                    {
                        if (!InvalidFiles.Contains(f.FullName))
                            InvalidFiles.Add(f.FullName);
                    }
                    ParsedLines += count;
                }
                else
                {
                    long count = 0;
                    Parallel.ForEach(File.ReadLines(f.FullName), (line, state) =>
                    {

                        var strings = line.Split(',').Select(s => s.Trim()).ToList();
                        string city = strings[2];
                        if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'") && strings.Count == 9)
                        {
                            try
                            {
                                city = city.Remove(0, 1);
                                results.TryAdd(city, new City());

                                string name = $"{strings[0]} {strings[1]}";
                                decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
                                string service = strings[^1];
                                if (service.Equals(""))
                                {
                                    InvalidFiles.Add(f.FullName);
                                    state.Stop();
                                }
                                long accountNumber = long.Parse(strings[^2]);
                                DateTime date = DateTime.ParseExact(strings[^3], "yyyy-dd-MM", null);

                                results[city].Add(name, payment, service, accountNumber, date);
                            }
                            catch (FormatException)
                            {
                                if (!InvalidFiles.Contains(f.FullName))
                                    InvalidFiles.Add(f.FullName);
                                state.Stop();
                            }
                        }
                        else
                        {
                            if (!InvalidFiles.Contains(f.FullName))
                                InvalidFiles.Add(f.FullName);
                            state.Stop();
                        }
                        Interlocked.Increment(ref count);
                    });
                    ParsedLines += count;
                }
                if (!InvalidFiles.Contains(f.FullName))
                {
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
                }
                else
                {
                    FoundErrors++;
                }
            });
            ProcessedFiles.AddRange(files.Select(f => f.Name));
        }

        private decimal CalculateTotalForCity(ICollection<Service> services)
        {
            decimal res = 0;
            services.ToList().ForEach(s => res += s.Total);
            return res;
        }

        public void Start()
        {
            if (File.Exists("processedFiles.txt"))
            {
                string files = File.ReadAllText("processedFiles.txt");
                ProcessedFiles.AddRange(files.Split(','));
            }
            
            Watcher.EnableRaisingEvents = true;
            Timer.Start();

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Interval = TimeSpan.TotalMilliseconds;
            string now = DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy");
            if (Directory.Exists($"{PathB}\\{now}"))
            {
                var di = Directory.CreateDirectory($"{PathB}\\{now}");
                using (var sw = File.CreateText($"{di.FullName}\\meta.log"))
                {
                    sw.WriteLine($"parsed_files: {ProcessedFiles.Count}");
                    sw.WriteLine($"parsed_lines: {ParsedLines}");
                    sw.WriteLine($"found_errors: {FoundErrors}");
                    sw.WriteLine($"invalid_files: {String.Join(", ", InvalidFiles)}");
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            using (var sw = File.CreateText("processedFiles.txt"))
            {
                sw.Write(String.Join(',', ProcessedFiles));
            }
        }
    }
}
