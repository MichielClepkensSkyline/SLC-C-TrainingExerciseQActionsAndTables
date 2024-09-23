namespace Skyline.Protocol.QActionsAndTables
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class TransportStreamsData
	{
		[JsonProperty("transport_streams")]
		public List<TransportStream> TransportStreams { get; set; }
	}
}
