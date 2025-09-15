using Newtonsoft.Json;
using QAction_3;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
            string datapath = @"C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";


            if (datapath.IsPathValid())
            {
                var data = File.ReadAllText(datapath);
                string dataFromJSON = Convert.ToString(data);
                TransportStreams deserializedTransportStreamsData = SecureNewtonsoftDeserialization.DeserializeObject<TransportStreams>(dataFromJSON);
                FillTables(protocol, deserializedTransportStreamsData);
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|File path is not valid ${datapath}", LogType.Error, LogLevel.NoLogging);
            }
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