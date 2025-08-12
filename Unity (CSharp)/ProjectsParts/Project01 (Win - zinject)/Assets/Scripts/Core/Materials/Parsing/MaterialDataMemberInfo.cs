using System;
using Leguar.TotalJSON;

namespace Core.Materials.Parsing
{
	/// <summary>
	/// Устаревшее название - "ItemLoad"
	/// </summary>
	public class MaterialDataMemberInfo
	{
		private string _jsonKey;

		public string JKey
		{
			set
			{
				_jsonKey = value;
				JKeyPath = _jsonKey.Split('/');
			}
		}
		
		public string Name { get; set; }
		public bool IsProperty { get; set; }
		public bool Optional { get; set; }
		public string[] JKeyPath { get; private set; }

		public JValue GetJValue(JValue source, bool ignoreFirst = false)
		{
			var item = source;
			
			if (JKeyPath.Length <= 1)
				return item;
			
			for (var i = ignoreFirst ? 1 : 0; i < JKeyPath.Length; i++)
			{
				switch (item)
				{
					case JString:
						return item;
					case JArray:
						throw new Exception("Array is invalid");
					case JSON json:
						return json.ContainsKey(JKeyPath[i]) ? json.Get(JKeyPath[i]) : null;
				}
			}
			
			return item;
		}
	}
}