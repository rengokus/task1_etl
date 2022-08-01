using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Concurrent;

namespace task1.model
{
    class Result
    {
        public string City { get; private set; }
        public List<Service> Services { get; } = new List<Service>();   
        public decimal Total { get; private set; }

        public Result() { }

        public Result(List<string> strings, string city)
        {
            City = city;
            string name = $"{strings[0]} {strings[1]}";
            decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
            string service = strings[^1];
            if (service.Equals(""))
            {
                throw new FormatException();
            }
            long accountNumber = long.Parse(strings[^2]);
            DateTime date = DateTime.ParseExact(strings[^3], "yyyy-dd-MM", null);
            Payer payer = new Payer(name, payment, date, accountNumber);
            int index = Services.FindIndex(s => s.Name.Equals(service));
            if (index == -1)
            {
                Services.Add(new Service(service, payer));
            }
            else
            {
                Services[index].payers.Add(payer);
            }
        }

        public void Add(List<string> strings)
        {
            string name = $"{strings[0]} {strings[1]}";
            decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
            string service = strings[^1];
            if (service.Equals(""))
            {
                Console.WriteLine("empty service");
                throw new FormatException();
            }
            long accountNumber = long.Parse(strings[^2]);
            //Console.WriteLine($"'{strings[^3]}'");
            DateTime date = DateTime.ParseExact(strings[^3], "yyyy-dd-MM", null);
            Payer payer = new Payer(name, payment, date, accountNumber);
            int index = Services.FindIndex(s => s.Name.Equals(service));
            if (index == -1)
            {
                Services.Add(new Service(service, payer));
            }
            else
            {
                Services[index].payers.Add(payer);
            }
        }

    }
}


//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//using System.Globalization;
//using System.Collections.Concurrent;

//namespace task1.model
//{
//    class Result
//    {
//        public Result() { }

//        //public Result(List<string> strings)
//        //{
//        //    string name = $"{strings[0]} {strings[1]}";
//        //    decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
//        //    string service = strings[^1];
//        //    if (service.Equals(""))
//        //    {
//        //        throw new FormatException();
//        //    }
//        //    long accountNumber = long.Parse(strings[^2]);
//        //    DateTime date = DateTime.Parse(strings[^3]);
//        //    Payer payer = new Payer(name, payment, date, accountNumber);
//        //    if (Services.ContainsKey(service))
//        //    {
//        //        Services[service].Add(payer);
//        //    }
//        //    else
//        //    {
//        //        if (Services.TryAdd(service, new List<Payer>()))
//        //        {
//        //            Services[service].Add(payer);
//        //        }

//        //    }


//        //}

//        public ConcurrentDictionary<string, List<Payer>> Services { get; } = new ConcurrentDictionary<string, List<Payer>>();
//        public decimal Total { get; private set; }

//        public void Add(List<string> strings)
//        {
//            string name = $"{strings[0]} {strings[1]}";
//            decimal payment = decimal.Parse(strings[^4].Replace('.', ','), new NumberFormatInfo { NumberDecimalSeparator = "," });
//            string service = strings[^1];
//            if (service.Equals(""))
//            {
//                Console.WriteLine("empty service");
//                throw new FormatException();
//            }
//            long accountNumber = long.Parse(strings[^2]);
//            //Console.WriteLine($"'{strings[^3]}'");
//            DateTime date = DateTime.ParseExact(strings[^3], "yyyy-dd-MM", null);
//            Payer payer = new Payer(name, payment, date, accountNumber);
//            Services.TryAdd(service, new List<Payer>());
//            Services[service].Add(payer);
//        }

//    }
//}