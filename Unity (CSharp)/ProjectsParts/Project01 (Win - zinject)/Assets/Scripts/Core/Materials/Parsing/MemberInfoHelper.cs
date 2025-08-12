using System.Collections.Generic;
using System.Reflection;
using Core.Common.Consts;
using Core.Materials.Attributes;

namespace Core.Materials.Parsing
{
	public class MemberInfoHelper : IMemberInfoHelper
	{
		public IEnumerable<MaterialDataMemberInfo> CollectMemberInfoList(IReflect type)
		{
			var list = new List<MaterialDataMemberInfo>();
			
			var properties = type.GetProperties(MemberBindingFlags.PublicInstanceProperty);
			var fields = type.GetFields(MemberBindingFlags.PublicInstanceField);
			
			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<MaterialDataPropertyParseAttribute>();
				
				if (attribute == null)
					continue;
				
				var memberInfo = new MaterialDataMemberInfo
				{
					JKey = string.IsNullOrEmpty(attribute.Key) ? property.Name : attribute.Key,
					Name = property.Name,
					IsProperty = true
				};
				
				list.Add(memberInfo);
			}
			
			foreach (var field in fields)
			{
				var attribute = field.GetCustomAttribute<MaterialDataPropertyParseAttribute>();
				
				if (attribute == null)
					continue;
				
				var memberInfo = new MaterialDataMemberInfo
				{
					JKey = string.IsNullOrEmpty(attribute.Key) ? field.Name : attribute.Key,
					Name = field.Name,
					IsProperty = false
				};
				
				list.Add(memberInfo);
			}
			
			return list;
		}
	}
}
