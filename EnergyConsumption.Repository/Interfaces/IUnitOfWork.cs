using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyConsumption.Repository.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        IReadingRepository Readings { get; }
        int Complete();
    }
}
