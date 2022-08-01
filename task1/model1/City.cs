using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace task1.model1
{
    class City
    {
        public ConcurrentDictionary<string, Service1> Services { get; } = new ConcurrentDictionary<string, Service1>();
        public decimal Total { get; private set; }

        public City() { }

        public void Add(List<string> strings)
        {
            string name = $"{strings[0]} {strings[1]}";
            decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
            string service = strings[^1];
            if (service.Equals(""))
            {
                throw new FormatException();
            }
            long accountNumber = long.Parse(strings[^2]);
            DateTime date = DateTime.ParseExact(strings[^3], "yyyy-dd-MM", null);
            Services.TryAdd(service, new Service1(service));
            Services[service].Add(new Payer1(name, payment, date, accountNumber));
        }
    }
}
