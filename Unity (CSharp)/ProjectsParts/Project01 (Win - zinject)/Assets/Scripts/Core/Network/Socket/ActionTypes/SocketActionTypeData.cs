using System;

namespace Core.Network.Socket.ActionTypes
{
	public class SocketActionTypeData
	{
		public Type TargetType { get; }
		public string ReplaceableAlias { get; }
		
		public SocketActionTypeData(Type targetType, string replaceableAlias)
		{
			TargetType = targetType;
			ReplaceableAlias = replaceableAlias;
		}
	}
}
