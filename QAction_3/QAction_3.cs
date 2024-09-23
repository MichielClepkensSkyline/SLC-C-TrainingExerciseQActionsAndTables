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
		// File path to Data.json
		string filePath = @"C:\Users\EnisAB\Desktop\Data\Data.json";

		try
		{
			string jsonContent = File.ReadAllText(filePath);

			// Deserialize the JSON string into TransportStreamsData model
			TransportStreamsData data = JsonConvert.DeserializeObject<TransportStreamsData>(jsonContent);

			// Display the parsed data
			foreach (var ts in data.TransportStreams)
			{
				protocol.Log($"Transport Stream ID: {ts.TsId}, Name: {ts.TsName}, Multicast: {ts.Multicast}, Source IP: {ts.SourceIp}, Network ID: {ts.NetworkId}");
				foreach (var service in ts.Services)
				{
					protocol.Log($"\tService ID: {service.ServiceId}, Name: {service.ServiceName}, Type: {service.ServiceType}, Provider: {service.ServiceProvider}");
				}
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}