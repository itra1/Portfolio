using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using com.ootii.Messages;
using Core.Base;
using Core.Common.Consts;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Materials.Storage.Data;
using Core.Materials.Storage.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Storage
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает только хранение материалов с соответствующим доступом к ним
	/// </summary>
	public class MaterialDataStorage : IMaterialDataStorage, ILateInitialized, ITickable, IDisposable
	{
		private readonly Dictionary<string, Type> _typesByModel;
		private readonly Dictionary<Type, List<MaterialData>> _materialsByType;
		private readonly ConcurrentQueue<UpdatedMaterialMessage> _messages;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		public bool IsInitialized { get; private set; }
		
		public IReadOnlyDictionary<string, Type> TypesByModel => _typesByModel;
		
		public MaterialDataStorage()
		{
			_materialsByType = new Dictionary<Type, List<MaterialData>>();
			_typesByModel = new Dictionary<string, Type>();
			_messages = new ConcurrentQueue<UpdatedMaterialMessage>();
			_disposeCancellationTokenSource = new CancellationTokenSource();

			CollectMaterialTypesAsync().Forget();
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_typesByModel.Clear();
			_materialsByType.Clear();
			_messages.Clear();
		}
		
		public MaterialData Add(MaterialData material,
			Action<MaterialData> onAddCompleted = null) 
		{
			if (material == null) 
				return null;
			
			lock (_materialsByType)
			{
				var type = material.GetType();
				
				if (!_materialsByType.TryGetValue(type, out var materials))
				{
					materials = new List<MaterialData>();
					_materialsByType.Add(type, materials);
				}
				
				if (!materials.ContainsByIdAndModel(material.Id, material.Model))
				{
					materials.Add(material);
					onAddCompleted?.Invoke(material);
				}
			}
			
			return material;
		}
		
		public TMaterialData Add<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onAddCompleted = null) 
			where TMaterialData : MaterialData
		{
			if (material == null) 
				return null;
			
			lock (_materialsByType)
			{
				var type = typeof(TMaterialData);
				
				if (!_materialsByType.TryGetValue(type, out var materials))
				{
					materials = new List<MaterialData>();
					_materialsByType.Add(type, materials);
				}
				
				if (!materials.ContainsByIdAndModel(material.Id, material.Model))
				{
					materials.Add(material);
					onAddCompleted?.Invoke(material);
				}
			}
			
			return material;
		}
		
		public IList<TMaterialData> Add<TMaterialData>(IList<TMaterialData> materials, 
			Action<TMaterialData> onAddCompleted = null) 
			where TMaterialData : MaterialData
		{
			for (var i = 0; i < materials.Count; i++)
				materials[i] = Add(materials[i], onAddCompleted);
			
			return materials;
		}
		
		public MaterialData Update(MaterialData material,
			Action<MaterialData> onUpdateCompleted = null)
		{
			if (material == null) 
				return null;
			
			var m = Get(material.GetType(), material.Id);
			
			if (m == null)
				return null;
			
			CopyFromTo(material, m, onUpdateCompleted);
			return m;
		}
		
		public TMaterialData Update<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onUpdateCompleted = null)
			where TMaterialData : MaterialData
		{
			if (material == null) 
				return null;
			
			var m = Get<TMaterialData>(material.Id);
			
			if (m == null)
				return null;
			
			CopyFromTo(material, m, onUpdateCompleted);
			return m;
		}
		
		public IList<TMaterialData> Update<TMaterialData>(IList<TMaterialData> materials,
			Action<TMaterialData> onUpdateCompleted = null) 
			where TMaterialData : MaterialData
		{
			for (var i = 0; i < materials.Count; i++)
				materials[i] = Update(materials[i], onUpdateCompleted);
			
			return materials;
		}
		
		public MaterialData UpdateOrAdd(MaterialData material,
			Action<MaterialData> onUpdateOrAddCompleted = null)
		{
			if (material == null) 
				return null;
			
			var m = Get(material.GetType(), material.Id);
			
			if (m == null)
				return Add(material, onUpdateOrAddCompleted);
			
			CopyFromTo(material, m, onUpdateOrAddCompleted);
			return m;
		}
		
		public TMaterialData UpdateOrAdd<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onUpdateOrAddCompleted = null)
			where TMaterialData : MaterialData
		{
			if (material == null) 
				return null;
			
			var m = Get<TMaterialData>(material.Id);
			
			if (m == null)
				return Add(material, onUpdateOrAddCompleted);
			
			CopyFromTo(material, m, onUpdateOrAddCompleted);
			return m;
		}
		
		public IList<TMaterialData> UpdateOrAdd<TMaterialData>(IList<TMaterialData> materials,
			Action<TMaterialData> onUpdateOrAddCompleted = null) 
			where TMaterialData : MaterialData
		{
			for (var i = 0; i < materials.Count; i++)
				materials[i] = UpdateOrAdd(materials[i], onUpdateOrAddCompleted);
			
			return materials;
		}
		
		public void Remove(MaterialData material)
		{
			if (material == null) 
				return;
			
			lock (_materialsByType)
			{
				if (_materialsByType.TryGetValue(material.GetType(), out var materials))
					materials.Remove(material);
			}
		}
		
		public void Remove<TMaterialData>(IList<TMaterialData> materials) where TMaterialData : MaterialData
		{
			foreach (var material in materials)
				Remove(material);
		}

		public MaterialData Get(ulong id)
		{
			lock (_materialsByType)
			{
				foreach (var materials in _materialsByType.Values)
				{
					if (materials.TryGetById(id, out var material))
						return material;
				}
				
				return null;
			}
		}

		public MaterialData Get(Type type, ulong id)
		{
			lock (_materialsByType)
			{
				return _materialsByType.TryGetValue(type, out var materials) 
					? materials.GetById(id) 
					: _materialsByType.GetById(type, id);
			}
		}

		public TMaterialData Get<TMaterialData>(ulong id) 
			where TMaterialData : MaterialData
		{
			return Get(typeof(TMaterialData), id) as TMaterialData;
		}
		
		public TMaterialData GetByModelAndName<TMaterialData>(string model, string name) 
			where TMaterialData : MaterialData
		{
			lock (_materialsByType)
			{
				var type = typeof(TMaterialData);

				return _materialsByType.TryGetValue(type, out var materials)
					? materials.GetByModelAndName(model, name) as TMaterialData
					: _materialsByType.GetByModelAndName(type, model, name) as TMaterialData;
			}
		}
		
		public TMaterialData GetByModelAndTag<TMaterialData>(string model, string tag) 
			where TMaterialData : MaterialData
		{
			lock (_materialsByType)
			{
				var type = typeof(TMaterialData);
				
				return _materialsByType.TryGetValue(type, out var materials)
					? materials.GetByModelAndTag(model, tag) as TMaterialData 
					: _materialsByType.GetByModelAndTag(type, model, tag) as TMaterialData;
			}
		}
		
		public IReadOnlyList<MaterialData> GetList()
		{
			lock (_materialsByType)
			{
				return _materialsByType.GetList();
			}
		}

		public IReadOnlyList<MaterialData> GetList(Type type)
		{
			lock (_materialsByType)
			{
				return _materialsByType.TryGetValue(type, out var materials) 
					? materials 
					: _materialsByType.GetList(type);
			}
		}
		
		public IReadOnlyList<TMaterialData> GetList<TMaterialData>()
			where TMaterialData : MaterialData
		{
			lock (_materialsByType)
			{
				var type = typeof(TMaterialData);
				
				return _materialsByType.TryGetValue(type, out var materials)
					? materials.GetList<TMaterialData>() 
					: _materialsByType.GetList<TMaterialData>(type);
			}
		}

		public IReadOnlyList<TMaterialData> GetListByModel<TMaterialData>(string model) 
			where TMaterialData : MaterialData
		{
			lock (_materialsByType)
			{
				var type = typeof(TMaterialData);

				return _materialsByType.TryGetValue(type, out var materials)
					? materials.GetListByModel<TMaterialData>(model)
					: _materialsByType.GetListByModel<TMaterialData>(model);
			}
		}

		public void Tick()
		{
			while (!_messages.IsEmpty && _messages.TryDequeue(out var message))
			{
				var material = message.Material;
				var messageType = message.MessageType;
				var parameters = message.Parameters;
                
				MessageDispatcher.SendMessage(material, messageType, parameters, EnumMessageDelay.IMMEDIATE);
			}
		}

		private async UniTaskVoid CollectMaterialTypesAsync()
		{
			try
			{
				if (!Thread.CurrentThread.IsBackground && Application.isPlaying)
				{
					var cancellationToken = _disposeCancellationTokenSource.Token;
					
					await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
					{
						await UniTask.SwitchToThreadPool();
						cancellationToken.ThrowIfCancellationRequested();
						CollectMaterialTypes();
					}
				}
				else
				{
					CollectMaterialTypes();
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

		private void CollectMaterialTypes()
		{
			var materialDataTypeBase = typeof(MaterialData);
			
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsClass || type.IsAbstract || !materialDataTypeBase.IsAssignableFrom(type))
					continue;
				
				var attribute = type.GetCustomAttribute<MaterialModelAttribute>(false);
				
				if (attribute == null)
					continue;
				
				_typesByModel.Add(attribute.Model, type);
			}
		}
		
		private void CopyFromTo<TMaterialData>(TMaterialData materialFrom,
			TMaterialData materialTo,
			Action<TMaterialData> onCompleted = null) 
			where TMaterialData : MaterialData
		{
			var type = materialFrom.GetType();
			
			var properties = type.GetProperties(MemberBindingFlags.PublicInstanceProperty);
			var fields = type.GetFields(MemberBindingFlags.PublicInstanceField);
			
			var materialDataUpdateAttributeType = typeof(MaterialDataPropertyUpdateAttribute);
			
			ISet<string> parameters = null;
			
			foreach (var property in properties)
			{
				if (!property.IsDefined(materialDataUpdateAttributeType)) 
					continue;
				
				var oldValue = property.GetValue(materialTo);
				var newValue = property.GetValue(materialFrom);
				
				if (MaterialDataPropertyHelper.IsDeeplyEquals(oldValue, newValue))
					continue;
				
				property.SetValue(materialTo, newValue);
				
				(parameters ??= new HashSet<string>()).Add(property.Name);
			}
			
			foreach (var field in fields)
			{
				if (!field.IsDefined(materialDataUpdateAttributeType)) 
					continue;
				
				var oldValue = field.GetValue(materialTo);
				var newValue = field.GetValue(materialFrom);
				
				if (MaterialDataPropertyHelper.IsDeeplyEquals(oldValue, newValue))
					continue;
				
				field.SetValue(materialTo, newValue);
				
				(parameters ??= new HashSet<string>()).Add(field.Name);
			}
			
			if (parameters == null)
				return;
			
			var updateMessageType = materialTo.UpdateMessageType;
			
			if (string.IsNullOrEmpty(updateMessageType))
				return;
			
			onCompleted?.Invoke(materialTo);
			
			if (Thread.CurrentThread.IsBackground)
				_messages.Enqueue(new UpdatedMaterialMessage(materialTo, updateMessageType, parameters));
			else
				MessageDispatcher.SendMessage(materialTo, updateMessageType, parameters, EnumMessageDelay.IMMEDIATE);
		}
	}
}