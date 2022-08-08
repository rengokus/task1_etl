using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1.model
{
    public class Result
    {
        [CsvColumn(Name ="first_name")]
        public string FirstName { get; set; }
        [CsvColumn(Name = "second_name")]
        public string LastName { get; set; }
        [CsvColumn(Name = "address")]
        public string Address { get; set; }
        [CsvColumn(Name = "payment")]
        public decimal Payment { get; set; }
        [CsvColumn(Name = "date")]
        public string Date { get; set; }
        [CsvColumn(Name = "account_number")]
        public long AccountNumber { get; set; }
        [CsvColumn(Name = "service")]
        public string Service { get; set; }

        public override string ToString()
        {
            return $"{FirstName} | {LastName} | {Address} | {Payment} | {Date} | {AccountNumber} | {Service}";
        }
    }
}
