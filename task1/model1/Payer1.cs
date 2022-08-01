using System;
using System.Collections.Generic;
using System.Text;

namespace task1.model1
{
    class Payer1
    {
        public string Name { get; private set; }
        public decimal Payment { get; private set; }
        public DateTime Date { get; private set; }
        public long AccountNumber { get; private set; }

        public Payer1(string name, decimal payment, DateTime date, long accountNumber)
        {
            Name = name;
            Payment = payment;
            Date = date;
            AccountNumber = accountNumber;
        }

        public Payer1() { }

    }
}
