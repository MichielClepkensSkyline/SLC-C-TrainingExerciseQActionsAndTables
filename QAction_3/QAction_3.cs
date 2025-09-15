using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using QAction_3;
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
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
	{
		try
		{
            string fileName = "C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";
            SecurePath secureFullPath = SecurePath.CreateSecurePath(fileName);

            string jsonString = File.ReadAllText(secureFullPath);
            JsonStructure json = SecureNewtonsoftDeserialization.DeserializeObject<JsonStructure>(jsonString);

            foreach (Transportstream transportstream in json.Transportstreams)
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
                // This method checks automatically if the row exists, in case not a new one is created
                protocol.transportstreams.SetRow(transportstreamRow, true);

                foreach (Service service in transportstream.Services)
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
                    protocol.services.SetRow(serviceRow, true);
                }
            }
        }
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
