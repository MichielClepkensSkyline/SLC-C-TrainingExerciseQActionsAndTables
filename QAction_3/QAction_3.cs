using System;
using System.Collections.Generic;
using System.IO;
using QAction_3;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

/// <summary>
/// DataMiner QAction Class: Poll Data.
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
			string dataFilePath = @"C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";
			var json = File.ReadAllText(SecurePath.ConstructSecurePath(dataFilePath));
			string jsonData = Convert.ToString(json);
			TransportStreams deserializedTransportStreams = SecureNewtonsoftDeserialization.DeserializeObject<TransportStreams>(jsonData);
			FillTables(protocol, deserializedTransportStreams);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void FillTables(SLProtocol protocol, TransportStreams deserializedTransportStreams)
	{
		try
		{
			List<object[]> transportStreamsTableContent = new List<object[]>();
			List<object[]> servicesTableContent = new List<object[]>();
			foreach (Transport_Stream transport_stream in deserializedTransportStreams.transport_streams)
			{
				transportStreamsTableContent.Add(new TransportstreamsQActionRow
				{
					Transportstreamsid_1001 = transport_stream.ts_id,
					Transportstreamsname_1002 = transport_stream.ts_name,
					Transportstreamsmulticast_1003 = transport_stream.multicast,
					Transportstreamssourceipaddress_1004 = transport_stream.sourceIp,
					Transportstreamsnetworkid_1005 = transport_stream.network_id,
					Transportstreamslastpolltime_1006 = DateTime.Now,
				}.ToObjectArray());

				foreach (Service service in transport_stream.services)
				{
					servicesTableContent.Add(new ServicesQActionRow
					{
						Servicesid_2001 = service.service_id,
						Servicesname_2002 = service.service_name,
						Servicestype_2003 = service.service_type,
						Servicesprovider_2004 = service.service_provider,
						Serviceslastpolltime_2005 = DateTime.Now,
					}.ToObjectArray());
				}
			}

			protocol.FillArray(Parameter.Transportstreams.tablePid, transportStreamsTableContent, NotifyProtocol.SaveOption.Full);
			protocol.FillArray(Parameter.Services.tablePid, servicesTableContent, NotifyProtocol.SaveOption.Full);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
