using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Skyline.DataMiner.Scripting.Parameter;

namespace QAction_3
{
	public class TransportStreams
	{
		public List<Transport_Stream> transport_streams { get; set; }
	}

	public class Transport_Stream
	{
		public int ts_id { get; set; }

		public string ts_name { get; set; }

		public string multicast { get; set; }

		public string sourceIp { get; set; }

		public int network_id { get; set; }

		public List<Service> services { get; set; }
	}

	public class Service
	{
		public int service_id { get; set; }

		public string service_name { get; set; }

		public string service_type { get; set; }

		public string service_provider { get; set; }
	}

}
