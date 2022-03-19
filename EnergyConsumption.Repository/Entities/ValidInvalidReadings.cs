using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EnergyConsumption.Repository.Entities
{
    public  class ValidInvalidReadings
    {
        public List<MeterReading> validReading { get; set; }  
        public DataTable inValidReading { get; set; }

    }
}
