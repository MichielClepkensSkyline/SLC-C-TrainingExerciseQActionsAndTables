using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.QActionsAndTables;

/// <summary>
/// DataMiner QAction Class: Poll and Parse Data.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			// Poll data from the JSON file containing transport streams and services
			var polledData = PollData();

			// Update the transport streams table with the newly polled data
			UpdateTransportStreamsTable(protocol, polledData);

			// Update the services table with the corresponding services from the polled data
			UpdateServicesTable(protocol, polledData);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static TransportStreamsData PollData()
	{
		// Define the path to the JSON data file
		string filePath = @"C:\Users\EnisAB\Desktop\Data\Data.json";

		// Read the content of the JSON file
		string jsonContent = File.ReadAllText(filePath);

		// Deserialize an return the JSON content into TransportStreamsData object
		return JsonConvert.DeserializeObject<TransportStreamsData>(jsonContent);
	}

	private static void UpdateTransportStreamsTable(SLProtocol protocol, TransportStreamsData polledData)
	{
		// Transform the polled data into rows suitable for the Transport Streams overview table
		var rowsToAdd = polledData.TransportStreams
			.Select(transportStream => new TransportstreamsoverviewQActionRow
			{
				Transportstreamsoverviewinstance_101 = transportStream.TsId.ToString(),
				Transportstreamsoverviewname_102 = transportStream.TsName.Trim(),
				Transportstreamsoverviewmulticast_103 = transportStream.Multicast,
				Transportstreamsoverviewnetworkid_104 = transportStream.NetworkId.ToString(),
				Transportstreamsoverviewnumberofservices_105 = transportStream.Services.Count,
				Transportstreamsoverviewlastpoll_106 = DateTime.Now.ToOADate(),
			})
			.Select(transportStreamRow => transportStreamRow.ToObjectArray())
			.ToList();

		// Update the transport streams table in the protocol with the newly created rows
		protocol.FillArray(Parameter.Transportstreamsoverview.tablePid, rowsToAdd, NotifyProtocol.SaveOption.Full);
	}

	private static void UpdateServicesTable(SLProtocol protocol, TransportStreamsData polledData)
	{
		// Transform the services from each transport stream into rows for the Services overview table
		var rowsToAdd = polledData.TransportStreams
			.SelectMany(transportStream => transportStream.Services.Select(service => new ServicesoverviewQActionRow
			{
				Servicesoverviewinstance_201 = service.ServiceId.ToString(),
				Servicesoverviewname_202 = service.ServiceName.Trim(),
				Servicesoverviewtype_203 = service.ServiceType,
				Servicesoverviewprovider_204 = service.ServiceProvider,
				Servicesoverviewlastpoll_205 = DateTime.Now.ToOADate(),
				Servicesoverviewtransportstreamfk_206 = Convert.ToString(transportStream.TsId),
			}))
			.Select(serviceRow => serviceRow.ToObjectArray())
			.ToList();

		// Update the services table in the protocol with the newly created rows
		protocol.FillArray(Parameter.Servicesoverview.tablePid, rowsToAdd, NotifyProtocol.SaveOption.Full);
	}
}