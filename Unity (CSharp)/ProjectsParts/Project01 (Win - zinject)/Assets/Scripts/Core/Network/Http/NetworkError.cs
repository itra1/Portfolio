using Core.Materials.Attributes;

namespace Core.Network.Http
{
	public class NetworkError
	{
		[MaterialDataPropertyParse("response")] 
		public Response Response { get; set; }
		[MaterialDataPropertyParse("status")] 
		public int Status { get; set; }
		[MaterialDataPropertyParse("message")] 
		public string Message { get; set; }
		[MaterialDataPropertyParse("name")] 
		public string Name { get; set; }
	}
	
	public class Response
	{
		[MaterialDataPropertyParse("error")] 
		public bool Error { get; set; }
		[MaterialDataPropertyParse("status")] 
		public int Status { get; set; }
		[MaterialDataPropertyParse("locked")] 
		public bool? IsLock { get; set; }
	}
}