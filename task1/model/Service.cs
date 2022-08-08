using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace task1.model
{
    class Service
    {
        public string Name { get; private set; }
        public List<Payer> Payers { get; } = new List<Payer>();
        public decimal Total { get; private set; }

        public Service() { }

        public Service(string name)
        {
            Name = name;
        }

        public void Add(Payer payer)
        {
            Total += payer.Payment;
            Payers.Add(payer);
        }
    }
}
