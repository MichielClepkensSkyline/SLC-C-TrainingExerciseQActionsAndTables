using System;
using System.IO;
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
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static TransportStreamsData PollData()
	{
		// File path to Data.json
		string filePath = @"C:\Users\EnisAB\Desktop\Data\Data.json";

		string jsonContent = File.ReadAllText(filePath);

		// Deserialize the JSON string into TransportStreamsData model
		return JsonConvert.DeserializeObject<TransportStreamsData>(jsonContent);
	}

	private static void UpdateTransportStreamsTable(SLProtocol protocol, TransportStreamsData polledData)
	{
		foreach (TransportStream transportStream in polledData.TransportStreams)
		{
			TransportstreamsoverviewQActionRow transportStreamRow = new TransportstreamsoverviewQActionRow
			{
				Transportstreamsoverviewinstance_101 = transportStream.TsId.ToString(),
				Transportstreamsoverviewname_102 = transportStream.TsName.Trim(),
				Transportstreamsoverviewmulticast_103 = transportStream.Multicast,
				Transportstreamsoverviewnetworkid_104 = transportStream.NetworkId.ToString(),
				Transportstreamsoverviewnumberofservices_105 = transportStream.Services.Count,
				Transportstreamsoverviewlastpoll_106 = DateTime.Now.ToOADate(),
			};

			if (!protocol.Exists(Parameter.Transportstreamsoverview.tablePid, transportStreamRow.Key))
			{
				protocol.AddRow(Parameter.Transportstreamsoverview.tablePid, transportStreamRow.ToObjectArray());
			}
			else
			{
				protocol.SetRow(Parameter.Transportstreamsoverview.tablePid, transportStreamRow.Key, transportStreamRow.ToObjectArray());
			}
		}
	}
}