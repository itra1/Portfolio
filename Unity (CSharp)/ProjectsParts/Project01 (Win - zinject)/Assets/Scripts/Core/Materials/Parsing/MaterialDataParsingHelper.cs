using System;
using System.Collections;
using System.Globalization;
using Core.Common.Consts;
using Leguar.TotalJSON;

namespace Core.Materials.Parsing
{
	/// <summary>
	/// Устаревшее название - "ParserHelper"
	/// </summary>
	public class MaterialDataParsingHelper : IMaterialDataParsingHelper
	{
		private readonly IMemberInfoHelper _memberInfoHelper;
		
		public MaterialDataParsingHelper(IMemberInfoHelper memberInfoHelper) => 
			_memberInfoHelper = memberInfoHelper;
		
		public TMaterialData Parse<TMaterialData>(string rawData) where TMaterialData : class
		{
			JValue jValue = JSON.ParseString(rawData);
			return Parse(typeof(TMaterialData), jValue) as TMaterialData;
		}
		
		public TMaterialData Parse<TMaterialData>(JValue jValue) where TMaterialData : class => 
			Parse(typeof(TMaterialData), jValue) as TMaterialData;
		
		public object Parse(Type type, JValue jValue) => 
			GetValue(type, jValue);

		public TMaterialData ParseClass<TMaterialData>(JSON json) where TMaterialData : class =>
			ParseClass(typeof(TMaterialData), json) as TMaterialData;

		public object ParseClass(Type type, JSON json)
		{
			if (json == null)
				return null;
			
			var material = Activator.CreateInstance(type);
			
			var infoList = _memberInfoHelper.CollectMemberInfoList(type);
			
			foreach (var memberInfo in infoList)
			{
				if (!json.ContainsKey(memberInfo.JKeyPath[0]))
					continue;
				
				var jValue = json.Get(memberInfo.JKeyPath[0]);
				
				if (memberInfo.IsProperty)
				{
					var property = type.GetProperty(memberInfo.Name, MemberBindingFlags.PublicInstanceProperty);
					
					if (property == null)
						continue;
					
					var value = GetValue(property.PropertyType, jValue, memberInfo);
					
					property.SetValue(material, value, MemberBindingFlags.PublicInstanceProperty, null, null, CultureInfo.CurrentCulture);
				}
				else
				{
					var field = type.GetField(memberInfo.Name, MemberBindingFlags.PublicInstanceField);
					
					if (field == null)
						continue;
					
					var value = GetValue(field.FieldType, jValue, memberInfo);
					
					field.SetValue(material, value, MemberBindingFlags.PublicInstanceField, null, CultureInfo.CurrentCulture);
				}
			}
			
			return material;
		}
		
		private JValue GetJValueLast(JValue jValue, MaterialDataMemberInfo memberInfo = null) 
			=> memberInfo is not { JKeyPath: { Length: > 1 }} ? jValue : memberInfo.GetJValue(jValue, true);
		
		private object GetValue(Type type, JValue jValue, MaterialDataMemberInfo memberInfo = null)
		{
			try
			{
				if (type == typeof(string))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					return jValueLast switch
					{
						null or JNull => null,
						JString jString => jString.AsString(),
						JNumber jNumber => jNumber.AsInt().ToString(),
						_ => null
					};
				}
				
				if (type == typeof(bool))
				{
					if (jValue is null or JNull)
						return false;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					return jValueLast is not (null or JNull) && ((JBoolean) jValueLast).AsBool();
				}
				
				if (type == typeof(bool?))
				{
					if (jValue is null or JNull)
						return false;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					return jValueLast is not (null or JNull) && ((JBoolean) jValueLast).AsBool();
				}
				
				if (type == typeof(short))
				{
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case JNumber jNumber:
							return jNumber.AsShort();
						case JString jString:
							return short.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(short?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsShort();
						case JString jString:
							return short.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(JSON))
				{
					if (jValue is null or JNull)
						return new JSON();
					
					return jValue as JSON;
				}
				
				if (type == typeof(JArray))
				{
					if (jValue is null or JNull or JSON or JString)
						return new JArray();
					
					return jValue as JArray;
				}
				
				if (type == typeof(int))
				{
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case JNumber jNumber:
							return jNumber.AsInt();
						case JString jString:
							return int.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(int?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsInt();
						case JString jString:
							return int.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(long))
				{
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case JString jString:
							return long.Parse(jString.AsString());
						case JNumber jNumber:
							return jNumber.AsLong();
					}
				}
				
				if (type == typeof(long?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsLong();
						case JString jString:
							return long.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(ulong))
				{
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case JString jString:
							return ulong.Parse(jString.AsString());
						case JNumber jNumber:
							return jNumber.AsULong();
					}
				}
				
				if (type == typeof(ulong?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsULong();
						case JString jString:
							return ulong.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(float))
				{
					var jVal = GetJValueLast(jValue, memberInfo);
					
					if (jVal is JNumber jNumber)
						return jNumber.AsFloat();
					
					return 0;
				}
				
				if (type == typeof(float?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsFloat();
						case JString jString:
							return float.Parse(jString.AsString());
					}
				}
				
				if (type == typeof(double))
				{
					var jVal = GetJValueLast(jValue, memberInfo);
					
					if (jVal is JNumber jNumber)
						return jNumber.AsDouble();
					
					return 0;
				}
				
				if (type == typeof(double?))
				{
					if (jValue is null or JNull)
						return null;
					
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					switch (jValueLast)
					{
						case null or JNull:
							return null;
						case JNumber jNumber:
							return jNumber.AsDouble();
						case JString jString:
							return double.Parse(jString.AsString());
					}
				}
				
				if (type.IsGenericType)
				{
					if (jValue is not JArray jArray)
						return null;
					
					var t = type.GetGenericArguments()[0];
					
					var resArr = Activator.CreateInstance(type);
					
					for (var i = 0; i < jArray.Length; i++)
					{
						var item = jArray.Get(i);
						
						if (memberInfo is { JKeyPath: { Length: > 1 } })
						{
							item = memberInfo.GetJValue(item, true);

							if (item == null)
								return null;
						}
						
						var obj = GetValue(t, item, memberInfo);
						
						if (obj != null)
							((IList) resArr).Add(Convert.ChangeType(obj, t));
					}
					
					return resArr;
				}
				
				if (type.IsArray)
				{
					var jValueLast = GetJValueLast(jValue, memberInfo);
					
					if (jValueLast is not JArray jArray)
						return null;
					
					var t = type.GetElementType();
					
					var resArr = Array.CreateInstance(t, jArray.Length);
					
					for (var i = 0; i < jArray.Length; i++)
					{
						var item = jArray.Get(i);
						
						if (memberInfo is { JKeyPath: { Length: > 1 } })
						{
							item = memberInfo.GetJValue(item, true);
							
							if (item == null)
								return null;
						}
						
						resArr.SetValue(GetValue(t, item, memberInfo), i);
					}
					
					return resArr;
				}
				
				if (type.IsClass)
				{
					var jVal = GetJValueLast(jValue, memberInfo);
					return jVal is null ? null : ParseClass(type, jVal as JSON);
				}
			}
			catch (Exception exception)
			{
				throw new Exception($"Parsing error with type {type}. Error: {exception.Message}");
			}

			return null;
		}
	}
}