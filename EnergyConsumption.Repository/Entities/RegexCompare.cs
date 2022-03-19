using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyConsumption.Repository.Entities
{
    public class RegexCompare
    {
        public string columnName { get; set; }
        public string pattern { get; set; }
     
        public Boolean required { get; set; }

        public static Dictionary<string, RegexCompare> dict = new Dictionary<string, RegexCompare>() {
               { "AccountId", new RegexCompare() { columnName = "AccountId", pattern = @"[\d]+", required = true}},
               { "MeterReadingDateTime", new RegexCompare() { columnName = "MeterReadingDateTime",pattern=@"^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$", required = true}},
               { "MeterReadValue", new RegexCompare() { columnName = "MeterReadValue", pattern = @"[\d]+",required = true}},
            };

    }
}
