using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Data;
using System.Globalization;

namespace ArduinoDatalogger
{
    public static class ArduinoDatalogger
    {
        [FunctionName("ArduinoDatalogger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ArduinoLog data = JsonConvert.DeserializeObject<ArduinoLog>(requestBody);

            _=WriteToDatabase(data);

            return new OkObjectResult("Completed");
        }

        
        public static Task WriteToDatabase(ArduinoLog ALog)
        {
            using (SqlConnection connection = new(Environment.GetEnvironmentVariable("ArduinoDatalogger_sqldb_connection")))
            {
                connection.Open();
                SqlCommand command = new(null, connection)
                {
                    // Create and prepare an SQL statement.
                    CommandText =
                    "INSERT INTO LOGS (DeviceID, LogId, Timestamp, CurrentA, CurrentB, CurrentC, VoltageAB, VoltageBC, VoltageCA, VoltageAN, VoltageBN, VoltageCN, ActivePowerA, ActivePowerB, ActivePowerC, ActivePowerTotal, ReactivePowerA, ReactivePowerB, ReactivePowerC, ReactivePowerTotal, Frequency, PowerFactorTotal) " +
                    "VALUES (@DeviceID, @LogId, @Timestamp, @CurrentA, @CurrentB, @CurrentC, @VoltageAB, @VoltageBC, @VoltageCA, @VoltageAN, @VoltageBN, @VoltageCN, @ActivePowerA, @ActivePowerB, @ActivePowerC, @ActivePowerTotal, @ReactivePowerA, @ReactivePowerB, @ReactivePowerC, @ReactivePowerTotal, @Frequency, @PowerFactorTotal)"
                };

                command.Parameters.Add("@DeviceID", SqlDbType.VarChar,10);
                command.Parameters["@DeviceID"].Value = ALog.DeviceID;

                command.Parameters.Add("@LogId", SqlDbType.UniqueIdentifier);
                command.Parameters["@LogId"].Value = Guid.NewGuid();

                command.Parameters.Add("@Timestamp", SqlDbType.DateTime,20);
                command.Parameters["@Timestamp"].Value = DateTime.Parse(ALog.Timestamp);

                command.Parameters.Add("@CurrentA", SqlDbType.VarChar, 10);
                command.Parameters["@CurrentA"].Value = ALog.CurrentA;

                command.Parameters.Add("@CurrentB", SqlDbType.VarChar, 10);
                command.Parameters["@CurrentB"].Value = ALog.CurrentB;

                command.Parameters.Add("@CurrentC", SqlDbType.VarChar, 10);
                command.Parameters["@CurrentC"].Value = ALog.CurrentC;

                command.Parameters.Add("@VoltageAB", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageAB"].Value = ALog.VoltageAB;

                command.Parameters.Add("@VoltageBC", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageBC"].Value = ALog.VoltageBC;

                command.Parameters.Add("@VoltageCA", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageCA"].Value = ALog.VoltageCA;

                command.Parameters.Add("@VoltageAN", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageAN"].Value = ALog.VoltageAN;

                command.Parameters.Add("@VoltageBN", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageBN"].Value = ALog.VoltageBN;

                command.Parameters.Add("@VoltageCN", SqlDbType.VarChar, 10);
                command.Parameters["@VoltageCN"].Value = ALog.VoltageCN;

                command.Parameters.Add("@ActivePowerA", SqlDbType.VarChar, 10);
                command.Parameters["@ActivePowerA"].Value = ALog.ActivePowerA;

                command.Parameters.Add("@ActivePowerB", SqlDbType.VarChar, 10);
                command.Parameters["@ActivePowerB"].Value = ALog.ActivePowerB;

                command.Parameters.Add("@ActivePowerC", SqlDbType.VarChar, 10);
                command.Parameters["@ActivePowerC"].Value = ALog.ActivePowerC;

                command.Parameters.Add("@ActivePowerTotal", SqlDbType.VarChar, 10);
                command.Parameters["@ActivePowerTotal"].Value = ALog.ActivePowerTotal;

                command.Parameters.Add("@ReactivePowerA", SqlDbType.VarChar, 10);
                command.Parameters["@ReactivePowerA"].Value = ALog.ReactivePowerA;

                command.Parameters.Add("@ReactivePowerB", SqlDbType.VarChar, 10);
                command.Parameters["@ReactivePowerB"].Value = ALog.ReactivePowerB;

                command.Parameters.Add("@ReactivePowerC", SqlDbType.VarChar, 10);
                command.Parameters["@ReactivePowerC"].Value = ALog.ReactivePowerC;

                command.Parameters.Add("@ReactivePowerTotal", SqlDbType.VarChar, 10);
                command.Parameters["@ReactivePowerTotal"].Value = ALog.ReactivePowerTotal;

                command.Parameters.Add("@Frequency", SqlDbType.VarChar, 10);
                command.Parameters["@Frequency"].Value = ALog.Frequency;

                command.Parameters.Add("@PowerFactorTotal", SqlDbType.VarChar, 10);
                command.Parameters["@PowerFactorTotal"].Value = ALog.PowerFactorTotal;

                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                command.ExecuteNonQuery();
            }
            return Task.CompletedTask;
        }
    }
    public class ArduinoLog
    {
        public int DeviceID { get; set; }
        public string Timestamp { get; set; }
        public float CurrentA { get; set; }
        public float CurrentB { get; set; }
        public float CurrentC { get; set; }
        public float VoltageAB { get; set; }
        public float VoltageBC { get; set; }
        public float VoltageCA { get; set; }
        public float VoltageAN { get; set; }
        public float VoltageBN { get; set; }
        public float VoltageCN { get; set; }
        public float ActivePowerA { get; set; }
        public float ActivePowerB { get; set; }
        public float ActivePowerC { get; set; }
        public float ActivePowerTotal { get; set; }
        public float ReactivePowerA { get; set; }
        public float ReactivePowerB { get; set; }
        public float ReactivePowerC { get; set; }
        public float ReactivePowerTotal { get; set; }
        public float Frequency { get; set; }
        public float PowerFactorTotal { get; set; }
    }
}
