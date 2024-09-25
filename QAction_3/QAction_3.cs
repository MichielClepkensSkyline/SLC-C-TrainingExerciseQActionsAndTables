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
			var polledData = PollData();

			UpdateTransportStreamsTable(protocol, polledData);

			UpdateServicesTable(protocol, polledData);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static TransportStreamsData PollData()
	{
		string filePath = @"C:\Users\EnisAB\Desktop\Data\Data.json";

		string jsonContent = File.ReadAllText(filePath);

		// Deserialize the JSON content into TransportStreamsData object and return it
		return JsonConvert.DeserializeObject<TransportStreamsData>(jsonContent);
	}

	private static void UpdateTransportStreamsTable(SLProtocol protocol, TransportStreamsData polledData)
	{
		// Transform the polled data into rows suitable for the Transport Streams overview table
		var rowsToAdd = polledData.TransportStreams
			.Select(transportStream => new TransportstreamsoverviewQActionRow
			{
				Transportstreamsoverviewinstance_101 = Convert.ToString(transportStream.TsId),
				Transportstreamsoverviewname_102 = transportStream.TsName.Trim(),
				Transportstreamsoverviewmulticast_103 = transportStream.Multicast,
				Transportstreamsoverviewnetworkid_104 = Convert.ToString(transportStream.NetworkId),
				Transportstreamsoverviewlastpoll_105 = DateTime.Now.ToOADate(),
				Transportstreamsoverviewnumberofservices_106 = transportStream.Services.Count,
			})
			.Select(transportStreamRow => transportStreamRow.ToObjectArray())
			.ToList();

		protocol.FillArray(Parameter.Transportstreamsoverview.tablePid, rowsToAdd, NotifyProtocol.SaveOption.Full);

		var numOfTransportStreamsWithNoServices = polledData.TransportStreams.Count(ts => ts.Services.Count == 0);
		protocol.SetParameter(Parameter.numoftransportstreamswithnoservices, numOfTransportStreamsWithNoServices);
		protocol.Log("Number of transport streams with zero services:" + numOfTransportStreamsWithNoServices.ToString());
	}

	private static void UpdateServicesTable(SLProtocol protocol, TransportStreamsData polledData)
	{
		// Transform the services from each transport stream into rows for the Services overview table
		var rowsToAdd = polledData.TransportStreams
			.SelectMany(transportStream => transportStream.Services.Select(service => new ServicesoverviewQActionRow
			{
				Servicesoverviewinstance_201 = Convert.ToString(service.ServiceId),
				Servicesoverviewname_202 = service.ServiceName.Trim(),
				Servicesoverviewtype_203 = service.ServiceType,
				Servicesoverviewprovider_204 = service.ServiceProvider,
				Servicesoverviewtransportstreamfk_205 = Convert.ToString(transportStream.TsId),
			}))
			.Select(serviceRow => serviceRow.ToObjectArray())
			.ToList();

		protocol.FillArray(Parameter.Servicesoverview.tablePid, rowsToAdd, NotifyProtocol.SaveOption.Full);
	}
}