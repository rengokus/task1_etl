using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;


namespace task1
{
    class Class1
    {
        private readonly Timer timer;
        
        public Class1()
        {
            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["PathA"]);
            FileInfo[] files = di.GetFiles("*.txt");
            string str = "";
            files.ToList().ForEach(Console.WriteLine);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
