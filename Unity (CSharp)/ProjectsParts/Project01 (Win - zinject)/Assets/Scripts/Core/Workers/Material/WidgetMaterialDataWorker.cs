using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;
using Zenject;

namespace Core.Workers.Material
{
	public class WidgetMaterialDataWorker : IDefiningConcreteType, IAfterAddingToStorage
	{
		private const string TypeKey = "type";
		
		private const string WidgetTypeValue = "Widget";
		
		private readonly Type _defaultType = default;
		
		private IMaterialDataParsingHelper _parsingHelper;
		private IDictionary<string, Type> _widgetTypes;
		
		[Inject]
		private void Initialize(IMaterialDataParsingHelper parsingHelper)
		{
			_parsingHelper = parsingHelper;
			_widgetTypes = new Dictionary<string, Type>();
			
			CollectWidgetTypes();
		}
		
		public Type DefineConcreteTypeFrom(JSON json)
		{
			if (!json.ContainsKey(TypeKey))
				return _defaultType;
            
			var typeValue = json.GetString(TypeKey);
			
			return typeValue == WidgetTypeValue ? typeof(WidgetMaterialData) : _defaultType;
		}
		
		public void PerformActionAfterAddingToStorageOf(MaterialData material)
		{
			if (material is not WidgetMaterialData widgetMaterial)
				return;
			
			if (!_widgetTypes.TryGetValue(widgetMaterial.WidgetType, out var widgetType))
				return;
			
			var widgetData = _parsingHelper.Parse(widgetType, widgetMaterial.WidgetDataJson) as WidgetDataBase;
			
			if (widgetData is ISelfDeserializable data)
				data.Deserialize();
			
			widgetMaterial.WidgetData = widgetData;
		}

		private void CollectWidgetTypes()
		{
			var widgetDataTypeBase = typeof(WidgetDataBase);
			
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsClass || type.IsAbstract || !widgetDataTypeBase.IsAssignableFrom(type))
					continue;
				
				var attribute = type.GetCustomAttribute<WidgetDataTypeKeyAttribute>();
				
				if (attribute == null)
					continue;
				
				_widgetTypes.Add(attribute.Key, type);
			}
		}
	}
}