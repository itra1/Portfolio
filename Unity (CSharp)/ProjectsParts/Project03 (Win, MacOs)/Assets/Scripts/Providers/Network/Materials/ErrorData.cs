using System.Collections.Generic;

namespace Providers.Network.Materials
{
	[System.Serializable]
	public class ErrorData
	{
		public string status;
		public string message;
		public Dictionary<string, string[]> errors;
	}
}
