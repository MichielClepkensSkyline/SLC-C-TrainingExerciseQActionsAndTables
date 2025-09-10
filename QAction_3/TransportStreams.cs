using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static Skyline.DataMiner.Scripting.Parameter;

namespace QAction_3
{
	public class TransportStreams
	{
		[JsonPropertyName("transport_streams")]
		public List<Transport_Stream> transport_streams { get; set; }
	}

	public class Transport_Stream
	{
		[JsonPropertyName("ts_id")]
		public int ts_id { get; set; }

		[JsonPropertyName("ts_name")]
		public string ts_name { get; set; }

		[JsonPropertyName("multicast")]
		public string multicast { get; set; }

		[JsonPropertyName("sourceIp")]
		public string sourceIp { get; set; }

		[JsonPropertyName("network_id")]
		public int network_id { get; set; }

		[JsonPropertyName("services")]
		public List<Service> services { get; set; }
	}

	public class Service
	{
		[JsonPropertyName("service_id")] 
		public int service_id { get; set; }

		[JsonPropertyName("service_name")]
		public string service_name { get; set; }

		[JsonPropertyName("service_type")]
		public string service_type { get; set; }

		[JsonPropertyName("service_provider")]
		public string service_provider { get; set; }
	}

}
