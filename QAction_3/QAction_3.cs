using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            string filePath= @"C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";

            SecurePath dataPath = SecurePath.CreateSecurePath(filePath);

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
        Dictionary<string, object[]> transportStreamsDictionary = new Dictionary<string, object[]>();
        Dictionary<string, object[]> servicesDictionary = new Dictionary<string, object[]>();

        foreach (TransportStream transportStream in transportStreams.TransportStreamsList)
        {
            if(String.IsNullOrWhiteSpace(transportStream.TransportStreamId))
            {
                protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|No primary key found for traansport stream{Environment.NewLine}", LogType.Error, LogLevel.NoLogging);
            }

            transportStreamsDictionary[transportStream.TransportStreamId]= new TransportstreamsQActionRow {
                Transportstreamsid_101= transportStream.TransportStreamId,
                Transportstreamsname_102 =transportStream.Name,
                Transportstreamsmulticast_103= transportStream.Multicast,
                Transportstreamsnetworkid_105= transportStream.NetworkId.ToString(),
                Transportstreamssourceip_104= transportStream.SourceIp,
                Transportstreamslastpolltime_106=DateTime.Now.ToOADate(),
            }.ToObjectArray();

            foreach (Service service in transportStream.Services)
            {
                if (String.IsNullOrWhiteSpace(service.ServiceId)) 
                {
                    protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|No primary key found for the service {Environment.NewLine}", LogType.Error, LogLevel.NoLogging);
                }

                servicesDictionary[service.ServiceId]= new ServicesQActionRow
                {
                    Servicesid_111 = service.ServiceId,
                    Servicesname_112 = service.ServiceName,
                    Servicesprovider_114= service.ServiceProvider,
                    Servicestype_113= service.ServiceType,
                    Servicestransportstreamid_115=transportStream.TransportStreamId,
                    Serviceslastpolltime_116=DateTime.Now.ToOADate(),
                }.ToObjectArray();
            }
        }

        protocol.FillArray(Transportstreams.tablePid, transportStreamsDictionary.Values.ToList(), NotifyProtocol.SaveOption.Full);
        protocol.FillArray(Services.tablePid, servicesDictionary.Values.ToList(), NotifyProtocol.SaveOption.Full);
    }
}