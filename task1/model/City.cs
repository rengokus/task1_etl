using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace task1.model
{
    class City
    {
        public ConcurrentDictionary<string, Service> Services { get; } = new ConcurrentDictionary<string, Service>();
        public decimal Total { get; private set; }

        public void Add(string name, decimal payment, string service, long accountNumber, DateTime date)
        {
            Services.TryAdd(service, new Service(service));
            Services[service].Add(new Payer(name, payment, date, accountNumber));
        }
    }
}
