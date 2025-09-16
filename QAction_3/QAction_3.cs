using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QAction_3;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
	{
		try
		{
            Dictionary<string, TransportstreamsQActionRow> transportstreamRows = new Dictionary<string, TransportstreamsQActionRow>();
            Dictionary<string, ServicesQActionRow> servicesRows = new Dictionary<string, ServicesQActionRow>();

            string fileName = "C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";
            SecurePath secureFullPath = SecurePath.CreateSecurePath(fileName);

            string jsonString = File.ReadAllText(secureFullPath);
            JsonStructure json = SecureNewtonsoftDeserialization.DeserializeObject<JsonStructure>(jsonString);

            FillTransportstreamRows(protocol, json, transportstreamRows, servicesRows);

            // Convert rows to columns
            object[] transportstreamColumns = protocol.transportstreams.QActionRowsToObjectFillArray(transportstreamRows.Values.ToArray());
            object[] servicesColumns = protocol.services.QActionRowsToObjectFillArray(servicesRows.Values.ToArray());

            protocol.transportstreams.FillArray(transportstreamColumns);
            protocol.services.FillArray(servicesColumns);
        }
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

    private static void FillTransportstreamRows(SLProtocolExt protocol, JsonStructure json, Dictionary<string, TransportstreamsQActionRow> transportstreamRows, Dictionary<string, ServicesQActionRow> servicesRows)
    {
        foreach (Transportstream transportstream in json.Transportstreams)
        {
            if (!String.IsNullOrWhiteSpace(transportstream.TransportstreamId))
            {
                TransportstreamsQActionRow transportstreamRow = new TransportstreamsQActionRow
                {
                    Transportstreamsid = transportstream.TransportstreamId,
                    Transportstreamsname = transportstream.TransportstreamName,
                    Transportstreamsmulticast = transportstream.Multicast,
                    Transportstreamssourceip = transportstream.SourceIp,
                    Transportstreamsnetworkid = transportstream.NetworkId,
                    Transportstreamslastpolledtime = DateTime.Now.ToOADate(),
                };
                transportstreamRows.Add(transportstream.TransportstreamId,transportstreamRow);
                FillServicesRows(protocol, transportstream, servicesRows);
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|FillTransportstreamRows|Transportstream Id is missing", LogType.Error, LogLevel.NoLogging);
            }
        }
    }

    private static void FillServicesRows(SLProtocolExt protocol, Transportstream transportstream, Dictionary<string, ServicesQActionRow> servicesRows)
    {
        foreach (Service service in transportstream.Services)
        {
            if (!String.IsNullOrWhiteSpace(service.ServiceId))
            {
                ServicesQActionRow serviceRow = new ServicesQActionRow
                {
                    Servicesid = service.ServiceId,
                    Servicesname = service.ServiceName,
                    Servicestype = service.ServiceType,
                    Servicesprovider = service.ServiceProvider,
                    Serviceslastpolledtime = DateTime.Now.ToOADate(),
                    Servicestransportstreamidfk = transportstream.TransportstreamId,
                    Servicestransportstreamname = transportstream.TransportstreamName,
                };
                servicesRows.Add(service.ServiceId, serviceRow);
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|FillServicesRows|Service Id is missing", LogType.Error, LogLevel.NoLogging);
            }
        }
    }
}
