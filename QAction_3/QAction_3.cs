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
			SecurePath securePath = SecurePath.CreateSecurePath(@"C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json");

			if (!File.Exists(securePath))
			{
				protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|: JSON file not found at path {securePath}", LogType.Error, LogLevel.NoLogging);
				return;
			}

			if (!dataFilePath.IsPathValid())
			{
				protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run| File path is not valid:{dataFilePath}", LogType.Error, LogLevel.NoLogging);
				return;
			}

			var json = File.ReadAllText(securePath);
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
			foreach (Transport_Stream transport_stream in deserializedTransportStreams.Transport_streams)
			{
				transportStreamsTableContent.Add(new TransportstreamsQActionRow
				{
					Transportstreamsid_1001 = transport_stream.Ts_id.ToString(),
					Transportstreamsname_1002 = transport_stream.Ts_name,
					Transportstreamsmulticast_1003 = transport_stream.Multicast,
					Transportstreamssourceipaddress_1004 = transport_stream.SourceIp,
					Transportstreamsnetworkid_1005 = transport_stream.Network_id,
					Transportstreamslastpolltime_1006 = DateTime.Now.ToOADate(),
				}.ToObjectArray());

				foreach (Service service in transport_stream.Services)
				{
					servicesTableContent.Add(new ServicesQActionRow
					{
						Servicesid_2001 = service.Service_id.ToString(),
						Servicesname_2002 = service.Service_name,
						Servicestype_2003 = service.Service_type,
						Servicesprovider_2004 = service.Service_provider,
						Serviceslastpolltime_2005 = DateTime.Now.ToOADate(),
						Servicestransportstreamid_2006 = transport_stream.Ts_id.ToString(),
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
