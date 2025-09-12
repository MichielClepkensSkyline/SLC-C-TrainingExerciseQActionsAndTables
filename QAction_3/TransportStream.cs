using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QAction_3
{
    public class TransportStreams
    {
        public List<TransportStream> TransportStreamsList { get; set; }
    }


    public class TransportStream
    {
        [JsonProperty("ts_id")]
        public int TransportStreamId { get; set; }

        [JsonProperty("ts_name")]
        public string Name { get; set; }

        [JsonProperty("multicast")]
        public string Multicast { get; set; }

        [JsonProperty("sourceIp")]
        public string SourceIp { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }
    }
}
