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
    public static void Run(SLProtocol protocol)
    {

        try
        {
            string datapath = "C:\\Skyline DataMiner\\Documents\\SLC-C-TrainingExerciseQActionsAndTables\\Data.json";

            var data = File.ReadAllText(datapath);
            string dataFromJSON = Convert.ToString(data);
            TransportStreams deserializedTransportStreamsData = SecureNewtonsoftDeserialization.DeserializeObject<TransportStreams>(dataFromJSON);
            FillTables(protocol, deserializedTransportStreamsData);
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    public static void FillTables(SLProtocol protocol, TransportStreams transportStreams)
    {
        foreach (TransportStream transportStream in transportStreams.TransportStreamsList)
        {
            var newTransportStreamRow = new TransportstreamsQActionRow
            {
                Transportstreamsid= transportStream.TransportStreamId,
                Transportstreamsname =transportStream.Name,
                Transportstreamsmulticast= transportStream.Multicast,
                Transportstreamsnetworkid= transportStream.NetworkId,
                Transportstreamssourceip= transportStream.SourceIp,
                Transportstreamslastpolltime=DateTime.Now.ToOADate(),
            };
            protocol.AddRow(Transportstreams.tablePid, newTransportStreamRow);

            foreach (Service service in transportStream.Services)
            {
                var newServicesRow = new ServicesQActionRow
                {
                    Servicesid = service.ServiceId,
                    Servicesname = service.ServiceName,
                    Servicesprovider= service.ServiceProvider,
                    Servicestype= service.ServiceType,
                    Servicestransportstreamid=transportStream.TransportStreamId,
                    Serviceslastpolltime=DateTime.Now.ToOADate(),
                };
                protocol.AddRow(Transportstreams.tablePid, newServicesRow);
            }
        }
    }
}