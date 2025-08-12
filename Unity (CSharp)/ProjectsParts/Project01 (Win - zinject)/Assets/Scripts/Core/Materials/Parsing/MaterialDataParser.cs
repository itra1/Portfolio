using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Core.Base;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Materials.Loading.AutoPreloader.Info;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Cysharp.Threading.Tasks;
using Leguar.TotalJSON;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Parsing
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает только парсинг материалов
	/// </summary>
	public class MaterialDataParser : IMaterialDataParser, ILateInitialized, IDisposable
	{
		private const string ModelKey = "model";
		
		private readonly DiContainer _container;
		private readonly IMaterialDataParsingHelper _parsingHelper;
		private readonly IMaterialDataStorage _materials;
		private readonly IDictionary<Type, IElementMaterialDataParser> _elementMaterialDataParsersByMaterialType;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		public bool IsInitialized { get; private set; }

		public MaterialDataParser(DiContainer container, IMaterialDataParsingHelper parsingHelper, IMaterialDataStorage materials)
		{
			_container = container;
			_parsingHelper = parsingHelper;
			_materials = materials;
			_elementMaterialDataParsersByMaterialType = new Dictionary<Type, IElementMaterialDataParser>();
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			CollectElementMaterialDataParsersAsync().Forget();
		}

		public void Parse(MaterialDataTypeLoadingInfo info, string rawData)
		{
			var jList = new List<JSON>();

			try
			{
				var jArray = JArray.ParseString(rawData);
				
				for (var i = 0; i < jArray.Length; i++)
					jList.Add(jArray.GetJSON(i));
			}
			catch
			{
				jList.Add(JSON.ParseString(rawData));
			}
			
			foreach (var element in jList)
			{
				var material = ParseOne(info.Type, element);
				
				if (material == null) 
					continue;
				
				_materials.UpdateOrAdd(material);
			}
		}
		
		public IList<MaterialData> Parse(MaterialDataLoadingInfo info, string rawData)
		{
			var materials = _elementMaterialDataParsersByMaterialType[info.Type].Parse(rawData);
			
			for (var i = 0; i < materials.Count; i++)
				materials[i] = _materials.UpdateOrAdd(materials[i]);
			
			return materials;
		}

		public MaterialData ParseOne(Type type, string rawData)
		{
			return _parsingHelper.Parse(type, JSON.ParseString(rawData)) as MaterialData;
		}

		public MaterialData ParseOne(Type type, JSON json)
		{
			return _parsingHelper.Parse(type, json) as MaterialData;
		}
		
		public MaterialData ParseOne<TMaterialData>(string rawData) where TMaterialData : MaterialData
		{
			return _parsingHelper.Parse<TMaterialData>(JSON.ParseString(rawData));
		}
		
		public TMaterialData ParseOne<TMaterialData>(JSON json) where TMaterialData : MaterialData
		{
			return _parsingHelper.Parse<TMaterialData>(json);
		}

		public MaterialData ParseOne(JSON json)
		{
			if (json == null)
				return null;
			
			var model = json.GetString(ModelKey);
			var type = _materials.TypesByModel[model];
			
			return ParseOne(type, json);
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_elementMaterialDataParsersByMaterialType.Clear();
		}
		
		private async UniTaskVoid CollectElementMaterialDataParsersAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					var materialDataTypeBase = typeof(MaterialData);
					var elementMaterialDataParserInterface = typeof(IElementMaterialDataParser);
					
					foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
					{
						cancellationToken.ThrowIfCancellationRequested();
						
						if (!type.IsClass || !materialDataTypeBase.IsAssignableFrom(type))
							continue;
						
						var attribute = type.GetCustomAttribute<MaterialDataParserAttribute>(false);
						
						if (attribute == null)
							continue;
						
						var elementMaterialDataParserType = attribute.Type;
						
						if (!elementMaterialDataParserInterface.IsAssignableFrom(elementMaterialDataParserType))
							continue;

						var parser = (IElementMaterialDataParser) Activator.CreateInstance(elementMaterialDataParserType);
						
						_container.Inject(parser);
						
						_elementMaterialDataParsersByMaterialType.Add(type, parser);
					}
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
				
				IsInitialized = true;
			}
		}
	}
}