using System;
using System.Collections;
using System.Globalization;
using Core.Common.Consts;
using Core.Materials.Parsing;
using Leguar.TotalJSON;
using Debug = Core.Logging.Debug;

namespace Editor.Materials.Parsing
{
	public class MaterialDataSerializeHelper : IMaterialDataSerializeHelper
	{
		private readonly IMemberInfoHelper _memberInfoHelper;
		
		public MaterialDataSerializeHelper(IMemberInfoHelper memberInfoHelper) => 
			_memberInfoHelper = memberInfoHelper;
		
		public string Serialize(object source) => SerializeValue(source).ToString();
		
		private object SerializeValue(object source, bool isString = true)
		{
			if (source == null)
				return null;
			
			var type = source.GetType();
			
			try
			{
				if (type == typeof(int) || type == typeof(int?)) 
					return (int) source;
				if (type == typeof(bool) || type == typeof(bool?)) 
					return (bool) source;
				if (type == typeof(double) || type == typeof(double?)) 
					return (double) source;
				if (type == typeof(decimal) || type == typeof(decimal?)) 
					return (decimal) source;
				if (type == typeof(float) || type == typeof(float?)) 
					return (float) source;
				if (type == typeof(short) || type == typeof(short?)) 
					return (short) source;
				if (type == typeof(long) || type == typeof(long?)) 
					return (long) source;
				if (type == typeof(ulong) || type == typeof(ulong?)) 
					return (ulong) source;
				if (type == typeof(DateTime)) 
					return ((DateTime) source).ToString(CultureInfo.CurrentCulture);
				if (type == typeof(JSON)) 
					return ((JSON) source).CreateString();
				if (type == typeof(JArray)) 
					return ((JArray) source).CreateString();
				if (type == typeof(string)) 
					return (string) source;
				if (type.IsArray) 
					return source;
				
				if (type.IsGenericType)
				{
					var list = (IList)source;
					var obj = new JArray();
					
					for (var i = 0; i < list.Count; i++)
					{
						var item = SerializeValue(list[i], false);
						obj.Add(item);
					}
					
					return obj;
				}
				
				//TODO: Add serialization of arrays of objects if necessary
				
				if (type.IsClass)
				{
					JSON obj = new();
					
					var infoList = _memberInfoHelper.CollectMemberInfoList(source.GetType());
					
					foreach (var info in infoList)
					{
						try
						{
							if (info.IsProperty)
							{
								var property = type.GetProperty(info.Name, MemberBindingFlags.PublicInstanceProperty);
								obj.Add(info.JKeyPath[0], SerializeValue(property.GetValue(source)));
							}
							else
							{
								var field = type.GetField(info.Name, MemberBindingFlags.PublicInstanceProperty);
								obj.Add(info.JKeyPath[0], SerializeValue(field.GetValue(source)));
							}
						}
						catch (Exception exception)
						{
							Debug.LogError($"Unable to serialize json with type {type.Name} due to error: {exception.Message}");
						}
					}
					
					return isString ? obj.CreateString().Replace("\\", "") : obj;
				}
			}
			catch (Exception exception)
			{
				Debug.LogError($"Unable to serialize json with type {type.Name} due to error: {exception.Message}");
			}
			
			return null;
		}
	}
}
