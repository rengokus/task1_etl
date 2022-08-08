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
                    s.ConstructUsing(service => new Main());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    
                });

                x.RunAsLocalSystem();

                x.SetServiceName("ETL service");
                x.SetDisplayName("ETL Service");
                x.SetDescription("Radency task 1");
            });

            int exitCodeValue = (int) Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}