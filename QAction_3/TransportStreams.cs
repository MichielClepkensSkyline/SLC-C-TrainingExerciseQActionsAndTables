namespace QAction_3
{
	using System.Collections.Generic;
	using System.Text.Json.Serialization;

	public class TransportStreams
	{
		[JsonPropertyName("transport_streams")]
		public List<Transport_Stream> Transport_streams { get; set; }
	}

	public class Transport_Stream
	{
		[JsonPropertyName("ts_id")]
		public int? Ts_id { get; set; }

		[JsonPropertyName("ts_name")]
		public string Ts_name { get; set; }

		[JsonPropertyName("multicast")]
		public string Multicast { get; set; }

		[JsonPropertyName("sourceIp")]
		public string SourceIp { get; set; }

		[JsonPropertyName("network_id")]
		public int Network_id { get; set; }

		[JsonPropertyName("services")]
		public List<Service> Services { get; set; }
	}

	public class Service
	{
		[JsonPropertyName("service_id")] 
		public int? Service_id { get; set; }

		[JsonPropertyName("service_name")]
		public string Service_name { get; set; }

		[JsonPropertyName("service_type")]
		public string Service_type { get; set; }

		[JsonPropertyName("service_provider")]
		public string Service_provider { get; set; }
	}

}
