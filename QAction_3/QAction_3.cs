using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;

using QAction_3;
using Skyline.DataMiner.Scripting;

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
			string source = "{\"transport_streams\":[{\"ts_id\":1,\"ts_name\":\"RTLHD\",\"multicast\":\"232.101.1.1\",\"sourceIp\":\"10.15.1.1\",\"network_id\":1,\"services\":[{\"service_id\":52006,\"service_name\":\"Service1\",\"service_type\":\"digital_television\",\"service_provider\":\"ProviderA\"},{\"service_id\":52007,\"service_name\":\"Service2\",\"service_type\":\"digital_television\",\"service_provider\":\"ProviderA\"}]},{\"ts_id\":2,\"ts_name\":\"DasErsteSD\",\"multicast\":\"232.101.1.2\",\"sourceIp\":\"10.15.1.2\",\"network_id\":1,\"services\":[{\"service_id\":101,\"service_name\":\"Service3\",\"service_type\":\"digital_television\",\"service_provider\":\"ProviderB\"},{\"service_id\":102,\"service_name\":\"Service4\",\"service_type\":\"digital_radio\",\"service_provider\":\"ProviderB\"}]},{\"ts_id\":3,\"ts_name\":\"ComedyCentralHD\",\"multicast\":\"232.101.1.3\",\"sourceIp\":\"10.15.1.3\",\"network_id\":2,\"services\":[{\"service_id\":2003,\"service_name\":\"Service5\",\"service_type\":\"digital_television\",\"service_provider\":\"ProviderC\"},{\"service_id\":2004,\"service_name\":\"Service6\",\"service_type\":\"digital_radio\",\"service_provider\":\"ProviderC\"}]}]}";
			TransportStreams deserializedTransportStreams = JsonConvert.DeserializeObject<TransportStreams>(source);
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
			List<object[]> transport_Streams = new List<object[]>();
			List<object[]> services = new List<object[]>();
			foreach (Transport_Stream transport_stream in deserializedTransportStreams.transport_streams)
			{
				transport_Streams.Add(new TransportstreamsQActionRow
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
					services.Add(new ServicesQActionRow
					{
						Servicesid_2001 = service.service_id,
						Servicesname_2002 = service.service_name,
						Servicestype_2003 = service.service_type,
						Servicesprovider_2004 = service.service_provider,
						Serviceslastpolltime_2005 = DateTime.Now,
					}.ToObjectArray());
				}
			}

			protocol.FillArray(Parameter.Transportstreams.tablePid, transport_Streams, NotifyProtocol.SaveOption.Full);
			protocol.FillArray(Parameter.Services.tablePid, services, NotifyProtocol.SaveOption.Full);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
