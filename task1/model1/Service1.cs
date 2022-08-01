using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace task1.model1
{
    class Service1
    {
        public string Name { get; private set; }
        public List<Payer1> Payers { get; } = new List<Payer1>();
        public decimal Total { get; private set; }

        public Service1() { }

        public Service1(string name)
        {
            Name = name;
        }

        public void Add(Payer1 payer)
        {
            Total += payer.Payment;
            Payers.Add(payer);
        }
    }
}
