using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Topshelf;
using System.Text.RegularExpressions;
using task1.model;
using System.Globalization;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Main>(s =>
                {
                    s.ConstructUsing(heartbeat => new Main());
                    s.WhenStarted(heartbeat => heartbeat.Start());
                    s.WhenStopped(heartbeat => heartbeat.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("HeartbeatService");
                x.SetDisplayName("Heartbeat Service");
                x.SetDescription("this is the very first try");
            });

            int exitCodeValue = (int) Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;

//            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["PathA"]);
//            string[] extensions = { ".txt" };
//            string pattern = "[a-zA-Z]+, [a-zA-Z]+, [“'\"][a-zA-Z]+, [a-zA-Z]+ \\d{1,3}, \\d\\[“'\"], \\d*\\.?\\d+, \\d{4}-\\d{2}-\\d{2}, \\d{7}, [a-zA-Z]+";
//            var files = di.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
//            files.ForEach(f => Console.WriteLine(f.Name));

//            var watch = new System.Diagnostics.Stopwatch();
//            var results = new List<Result>();
//            watch.Start();

//            foreach (var file in files)
//            {
//                foreach (var line in File.ReadLines(file.FullName))
//                {
//                    try
//                    {
//                        var strings = line.Split(',').Select(s => s.Trim()).ToList();
//                        string city = strings[2];
//                        if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'"))
//                        {
//                            city = city.Remove(0, 1);
//                            int index = results.FindIndex(r => city.Equals(r.City));
//                            if (index == -1)
//                            {
//                                results.Add(new Result(strings, city));
//                            }
//                            else
//                            {
//                                results[index].Add(strings);
//                            }
//                        }
//                        else
//                        {
//                            Console.WriteLine("sada");
//                        }
                        
//                    }
//                    catch (FormatException ex)
//                    {
//                        Console.WriteLine("cringe");
//                        Console.WriteLine(results.Count);
//                    }
//                }
//            }
//            //files.ForEach(f => File.ReadLines(f.FullName).AsParallel().ForAll(line =>
//            //    {
//            //        line.LastIndexOf('"')
//            //        line.Split(", ").ToList().ForEach(v => Console.WriteLine($"'{v.Trim()}'"));
//            //    })
//            //);
//            //File.ReadLines(files[1].FullName).AsParallel().WithDegreeOfParallelism(6).ForAll(line =>
//            //{
//            //    //Console.WriteLine(line);
//            //});
//            watch.Stop();
//            Console.WriteLine($"Time: {watch.ElapsedMilliseconds} ms");
//            results.ForEach(r => Console.WriteLine(r.City));
//            string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
//            using (var sw = File.CreateText(ConfigurationManager.AppSettings["PathB"]))
//            {
//                sw.WriteLine(json);
//            }
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using Topshelf;
//using System.Text.RegularExpressions;
//using task1.model;
//using System.Globalization;
//using System.Collections.Concurrent;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace task1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            //var exitCode = HostFactory.Run(x =>
//            //{
//            //    x.Service<HeartBeat>(s =>
//            //    {
//            //        s.ConstructUsing(heartbeat => new HeartBeat());
//            //        s.WhenStarted(heartbeat => heartbeat.Start());
//            //        s.WhenStopped(heartbeat => heartbeat.Stop());
//            //    });

//            //    x.RunAsLocalSystem();

//            //    x.SetServiceName("HeartbeatService");
//            //    x.SetDisplayName("Heartbeat Service");
//            //    x.SetDescription("this is the very first try");
//            //});

//            //int exitCodeValue = (int) Convert.ChangeType(exitCode, exitCode.GetTypeCode());
//            //Environment.ExitCode = exitCodeValue;
//            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["PathA"]);
//            string[] extensions = { ".txt" };
//            string pattern = "[a-zA-Z]+, [a-zA-Z]+, [“'\"][a-zA-Z]+, [a-zA-Z]+ \\d{1,3}, \\d\\[“'\"], \\d*\\.?\\d+, \\d{4}-\\d{2}-\\d{2}, \\d{7}, [a-zA-Z]+";
//            var files = di.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();
//            files.ForEach(f => Console.WriteLine(f.Name));

//            var watch = new System.Diagnostics.Stopwatch();
//            var results = new ConcurrentDictionary<string, Result>();
//            watch.Start();

//            foreach (var file in files)
//            {
//                foreach (var line in File.ReadLines(file.FullName))
//                {
//                    try
//                    {
//                        var strings = line.Split(',').Select(s => s.Trim()).ToList();
//                        string city = strings[2];
//                        if ((city.StartsWith('"') || city.StartsWith("'")) && !city.EndsWith('"') && !city.EndsWith("'"))
//                        {
//                            city = city.Remove(0, 1);
//                            results.TryAdd(city, new Result());
//                            results[city].Add(strings);
//                        }
//                        else
//                        {
//                            Console.WriteLine("sada");
//                        }

//                    }
//                    catch (FormatException ex)
//                    {
//                        Console.WriteLine("cringe");
//                        Console.WriteLine(results.Count);
//                    }
//                }
//            }
//            //files.ForEach(f => File.ReadLines(f.FullName).AsParallel().ForAll(line =>
//            //    {
//            //        line.LastIndexOf('"')
//            //        line.Split(", ").ToList().ForEach(v => Console.WriteLine($"'{v.Trim()}'"));
//            //    })
//            //);
//            //File.ReadLines(files[1].FullName).AsParallel().WithDegreeOfParallelism(6).ForAll(line =>
//            //{
//            //    //Console.WriteLine(line);
//            //});
//            watch.Stop();
//            Console.WriteLine($"Time: {watch.ElapsedMilliseconds} ms");
//            results.Keys.ToList().ForEach(Console.WriteLine);
//            //DateTime date = DateTime.Parse("2022-12-07");
//            //Console.WriteLine(date);
//            string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
//            using (var sw = File.CreateText(ConfigurationManager.AppSettings["PathB"]))
//            {
//                sw.WriteLine(json);
//            }
//        }
//    }
//}