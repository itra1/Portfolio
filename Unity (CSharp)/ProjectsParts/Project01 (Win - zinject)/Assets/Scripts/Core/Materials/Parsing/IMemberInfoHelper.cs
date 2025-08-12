using System.Collections.Generic;
using System.Reflection;

namespace Core.Materials.Parsing
{
	public interface IMemberInfoHelper
	{
		public IEnumerable<MaterialDataMemberInfo> CollectMemberInfoList(IReflect type);
	}
}
