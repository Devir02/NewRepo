using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Domain.Interfaces;
using EnergyConsumption.Repository;
using EnergyConsumption.Repository.Entities;

namespace EnergyConsumption.Repository.Interfaces
{
    public interface IReadingRepository : IGenericRepository<MeterReading>
    {
        ValidInvalidReadings ProcessMeterReadings(StreamReader stream);
    }
}
