using EnergyConsumption.Repository;
using EnergyConsumption.Repository.Entities;
using EnergyConsumption.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyConsumption.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergyConsumptionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public EnergyConsumptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("meter-reading-uploads")]
        public IActionResult ImportMeterReadings([FromForm] IFormFile file)
        {
            try
            {
                ValidInvalidReadings meterReadings = new ValidInvalidReadings();

                List<MeterReading> readings = new List<MeterReading>();

                if (file.Length > 0)
                {
                    var stream = file.OpenReadStream();
                    using (var reader = new StreamReader(stream))

                    {
                        meterReadings = _unitOfWork.Readings.ProcessMeterReadings(reader);

                        foreach (MeterReading r in meterReadings.validReading)
                        {
                            _unitOfWork.Readings.Add(r);
                            _unitOfWork.Complete();

                        }

                    }
                }

                return Ok(meterReadings);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");

            }
        }

    }

}

