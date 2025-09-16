using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using QAction_3;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
using static Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class: Parse Data Into Tables.
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
            SecurePath dataPath = SecurePath.CreateSecurePath(@"C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json");

            var readDataFromFile = File.ReadAllText(dataPath);
            string dataToString = Convert.ToString(readDataFromFile);
            TransportStreams deserializedTransportStreamsData = SecureNewtonsoftDeserialization.DeserializeObject<TransportStreams>(dataToString);
            FillTables(protocol, deserializedTransportStreamsData);
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    public static void FillTables(SLProtocolExt protocol, TransportStreams transportStreams)
    {
        foreach (TransportStream transportStream in transportStreams.TransportStreamsList)
        {
            var newTransportStreamRow = new TransportstreamsQActionRow
            {
                Transportstreamsid_101= transportStream.TransportStreamId.ToString(),
                Transportstreamsname_102 =transportStream.Name,
                Transportstreamsmulticast_103= transportStream.Multicast,
                Transportstreamsnetworkid_105= transportStream.NetworkId.ToString(),
                Transportstreamssourceip_104= transportStream.SourceIp,
                Transportstreamslastpolltime_106=DateTime.Now.ToOADate(),
            };
            protocol.transportstreams.SetRow(newTransportStreamRow, true);

            foreach (Service service in transportStream.Services)
            {
                var newServicesRow = new ServicesQActionRow
                {
                    Servicesid_111 = service.ServiceId.ToString(),
                    Servicesname_112 = service.ServiceName,
                    Servicesprovider_114= service.ServiceProvider,
                    Servicestype_113= service.ServiceType,
                    Servicestransportstreamid_115=transportStream.TransportStreamId.ToString(),
                    Serviceslastpolltime_116=DateTime.Now.ToOADate(),
                };
                protocol.services.SetRow(newServicesRow, true);
            }
        }
    }
}