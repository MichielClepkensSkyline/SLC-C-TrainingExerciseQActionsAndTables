using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static Skyline.DataMiner.Scripting.Parameter;




/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    public class Service
	{
        public string service_id { get; set; }
        public string service_name { get; set; }
        public string service_type { get; set; }
        public string service_provider { get; set; }
    }

	public class Transportstream
	{
        public string ts_id;
        public string ts_name { get; set; }
        public string multicast { get; set; }
        public string sourceIp { get; set; }
        public int network_id { get; set; }
        public List<Service> services { get; set; }
    }

    public class JsonStructure
    {
        public List<Transportstream> transport_streams { get; set; }
    }
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
	{
		bool useProtocolExtended = false;
		try
		{
            string fileName = "C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";
            string jsonString = File.ReadAllText(fileName);
            JsonStructure json = JsonConvert.DeserializeObject<JsonStructure>(jsonString);
            // protocol.Log("JSON file read");

            foreach (Transportstream ts in json.transport_streams)
            {
                TransportstreamsQActionRow transportstreamRow = new TransportstreamsQActionRow
                {
                    Transportstreamsid = ts.ts_id,
                    Transportstreamsname = ts.ts_name, //wat zijn exact deze namen?
                    Transportstreamsmulticast = ts.multicast,
                    Transportstreamssourceip = ts.sourceIp,
                    Transportstreamsnetworkid = ts.network_id,
                    Transportstreamslastpolledtime = DateTime.Now.ToOADate(),
                };
                // This method checks automatically if the row exists, in case not a new one is created
                protocol.transportstreams.SetRow(transportstreamRow, true);
                // protocol.Log("Transportstream added or updated");

                /* This is the old school version in which it is checked by an if else
                if (protocol.transportstreams.Exists(transportstreamRow.Transportstreamsid.ToString()))
                {
                    protocol.transportstreams.SetRow(transportstreamRow);
                }
                else
                {
                    protocol.transportstreams.AddRow(transportstreamRow);
                }
                */

                /* This version is used when de extension of the protocol is not available (also need of an if else)
                protocol.AddRow(Parameter.Transportstreams.tablePid, transportstreamRow.ToObjectArray());
                */

                foreach (Service service in ts.services)
                {
                    ServicesQActionRow serviceRow = new ServicesQActionRow
                    {
                        Servicesid = service.service_id,
                        Servicesname = service.service_name,
                        Servicestype = service.service_type,
                        Servicesprovider = service.service_provider,
                        Serviceslastpolledtime = DateTime.Now.ToOADate(),
                    };
                    protocol.services.SetRow(serviceRow, true);
                    //protocol.Log("Service added or updated");
                }
            }

        }
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
