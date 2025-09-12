using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;
using Skyline.DataMiner.Utils.SecureCoding;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
using static Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    public class Root
    {
        [JsonProperty("transport_streams")]
        public List<TransportStream> TransportStreams;
    }

    public class Service
    {
        [JsonProperty("service_id")]
        public string ServiceId;

        [JsonProperty("service_name")]
        public string ServiceName;

        [JsonProperty("service_type")]
        public string ServiceType;

        [JsonProperty("service_provider")]
        public string ServiceProvider;
    }

    public class TransportStream
    {
        [JsonProperty("ts_id")]
        public string TransportStreamId;

        [JsonProperty("ts_name")]
        public string TransportStreamName;

        [JsonProperty("multicast")]
        public string Multicast;

        [JsonProperty("sourceIp")]
        public string SourceIp;

        [JsonProperty("network_id")]
        public int? NetworkId;

        [JsonProperty("services")]
        public List<Service> Services;
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
            SecurePath secureFullPath = SecurePath.CreateSecurePath(fileName);

            string jsonString = File.ReadAllText(secureFullPath);
            Root json = SecureNewtonsoftDeserialization.DeserializeObject<Root>(jsonString);

            //Root json = JsonConvert.DeserializeObject<Root>(jsonString);
            //protocol.Log("JSON file read");

            foreach (TransportStream transportStream in json.TransportStreams)
            {
                TransportstreamsQActionRow transportstreamRow = new TransportstreamsQActionRow
                {
                    Transportstreamsid = transportStream.TransportStreamId,
                    Transportstreamsname = transportStream.TransportStreamName,
                    Transportstreamsmulticast = transportStream.Multicast,
                    Transportstreamssourceip = transportStream.SourceIp,
                    Transportstreamsnetworkid = transportStream.NetworkId,
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

                foreach (Service service in transportStream.Services)
                {
                    ServicesQActionRow serviceRow = new ServicesQActionRow
                    {
                        Servicesid = service.ServiceId,
                        Servicesname = service.ServiceName,
                        Servicestype = service.ServiceType,
                        Servicesprovider = service.ServiceProvider,
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
