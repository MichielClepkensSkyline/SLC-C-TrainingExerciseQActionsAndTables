using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAction_3
{
    public class JsonStructure
    {
        [JsonProperty("transport_streams")]
        public List<Transportstream> Transportstreams;
    }

    public class Service
    {
        [JsonProperty("service_id")]
        public string ServiceId;

        [JsonProperty("service_name")]
        public string ServiceName;

        [JsonProperty("service_type")]
        public string ServiceType;

        [JsonProperty("service_provider")]
        public string ServiceProvider;
    }

    public class Transportstream
    {
        [JsonProperty("ts_id")]
        public string TransportstreamId;

        [JsonProperty("ts_name")]
        public string TransportstreamName;

        [JsonProperty("multicast")]
        public string Multicast;

        [JsonProperty("sourceIp")]
        public string SourceIp;

        [JsonProperty("network_id")]
        public string NetworkId;

        [JsonProperty("services")]
        public List<Service> Services;
    }
}
