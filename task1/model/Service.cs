using System;
using System.Collections.Generic;
using System.Text;

namespace task1.model
{
    class Service
    {
        public string Name { get; private set; }
        public List<Payer> payers { get; private set; } = new List<Payer>();
        public decimal Total { get; private set; }

        public Service() { }

        public Service(string name, Payer payer)
        {
            Name = name;
            payers.Add(payer);
        }
    }
}
