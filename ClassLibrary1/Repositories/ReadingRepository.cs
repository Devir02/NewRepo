using EnergyConsumption.Data;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyConsumption.Repository;
using EnergyConsumption.Repository.Interfaces;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Data;
using EnergyConsumption.Repository.Entities;
using System.Text.RegularExpressions;

namespace DataAccess.EFCore.Repositories
{
    public class ReadingRepository : GenericRepository<MeterReading>, IReadingRepository
    {
        public ReadingRepository(EnergyConsumptionContext context) : base(context)
        {
        }
        public ValidInvalidReadings ProcessMeterReadings(StreamReader stream)
        {
            var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
            var dr = new CsvDataReader(csv);
            var dt = new DataTable();

            dt.Load(dr);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ValidInvalidReadings validInvalid = Get_Error_Rows(ds.Tables[0]);
            return validInvalid;
        }

        private List<MeterReading> filterReadings(DataTable dataReadings)
        {
            MeterReading reading = null;
            List<MeterReading> readings = new List<MeterReading>();
            DataTable dtFilteredReading = dataReadings.Clone();

            foreach (DataRow dr in dtFilteredReading.AsEnumerable())
            {
                reading = new MeterReading() { AccountId = Convert.ToInt32(dr.ItemArray[0]), MeterReadingDateTime = Convert.ToDateTime(dr.ItemArray[1]), MeterReadValue = Convert.ToInt32(dr.ItemArray[2]) };
                readings.Add(reading);
            }

            var finalReadings = from meterReading in readings
                                group meterReading by new { meterReading.AccountId }
                                     into mIdGroup
                                select mIdGroup.OrderBy(groupedReading => groupedReading.MeterReadingDateTime)
                                                .Last();
            return finalReadings.ToList();
        }

        public ValidInvalidReadings Get_Error_Rows(DataTable dt)
        {
            ValidInvalidReadings validInvalidReadings = new ValidInvalidReadings();
            DataTable dtError = null;
            DataTable dtData = null;
            List<DataTable> validInvalid = new List<DataTable>();

            if (dtData == null) dtData = dt.Clone();
            foreach (DataRow row in dt.AsEnumerable())
            {

                Boolean error = false;
                foreach (DataColumn col in dt.Columns.OfType<DataColumn>().Take(3))

                {

                    RegexCompare regexCompare = RegexCompare.dict[col.ColumnName];
                    object colValue = row.Field<object>(col.ColumnName);
                    if (regexCompare.required)
                    {
                        if (colValue == null)
                        {
                            error = true;
                            break;
                        }
                    }
                    else
                    {
                        if (colValue == null)
                            continue;
                    }
                    string colValueStr = colValue.ToString();
                    Match match = Regex.Match(colValueStr, regexCompare.pattern);

                    if (!match.Success)
                    {
                        error = true;
                        break;
                    }
                    if (colValueStr.Length != match.Value.Length)
                    {
                        error = true;
                        break;
                    }

                    else
                    {
                        if (match.Success)
                        {
                            error = false;

                        }
                    }

                }

                if (error)
                {
                    if (dtError == null) dtError = dt.Clone();
                    dtError.Rows.Add(row.ItemArray);

                }
                else
                {

                    dtData.Rows.Add(row.ItemArray);
                }


            }
            while (dtData.Columns.Count > 3)
            {
                dtData.Columns.RemoveAt(3);

                validInvalidReadings.validReading = filterReadings(dtData);


            }
            while (dtError.Columns.Count > 3)
            {
                dtError.Columns.RemoveAt(3);
                validInvalidReadings.inValidReading = dtError;

            }

            return validInvalidReadings;
        }
    }
}
