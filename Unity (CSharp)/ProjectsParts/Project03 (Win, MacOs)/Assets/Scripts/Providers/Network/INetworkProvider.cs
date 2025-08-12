using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Providers.Network
{
	public interface INetworkProvider
	{
		public string Server { get; }
	}
}
