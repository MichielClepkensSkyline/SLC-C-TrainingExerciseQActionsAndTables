using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.IO;




/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    public class Service
	{
        public int service_id { get; set; }
        public string service_name { get; set; }
        public string service_type { get; set; }
        public string service_provider { get; set; }
    }

	public class Transportstream
	{
        public int ts_id { get; set; }
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
            JsonStructure ts = JsonConvert.DeserializeObject<JsonStructure>(jsonString);
            protocol.Log(ts.transport_streams[0].ts_id.ToString());
            protocol.Log(ts.transport_streams[0].ts_name);
        }
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
