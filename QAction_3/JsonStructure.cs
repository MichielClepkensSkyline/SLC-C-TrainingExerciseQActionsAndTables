namespace QAction_3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class JsonStructure
    {
        [JsonProperty("transport_streams")]
        public List<Transportstream> Transportstreams { get; set; }
    }

    public class Service
    {
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("service_type")]
        public string ServiceType { get; set; }

        [JsonProperty("service_provider")]
        public string ServiceProvider { get; set; }
    }

    public class Transportstream
    {
        [JsonProperty("ts_id")]
        public string TransportstreamId { get; set; }

        [JsonProperty("ts_name")]
        public string TransportstreamName { get; set; }

        [JsonProperty("multicast")]
        public string Multicast { get; set; }

        [JsonProperty("sourceIp")]
        public string SourceIp { get; set; }

        [JsonProperty("network_id")]
        public string NetworkId { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }
    }
}
