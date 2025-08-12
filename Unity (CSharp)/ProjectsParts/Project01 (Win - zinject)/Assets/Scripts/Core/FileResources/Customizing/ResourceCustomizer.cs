using System;
using System.Collections.Generic;
using System.Text;
using Core.FileResources.Customizing.Category;
using Core.FileResources.Customizing.Converting.Deserializers;
using Core.FileResources.Customizing.Converting.Deserializers.Base;
using Core.FileResources.Customizing.Converting.Serializers;
using Core.FileResources.Customizing.Converting.Serializers.Base;
using Core.FileResources.Customizing.Destructors;
using Core.FileResources.Customizing.Destructors.Base;
using Core.FileResources.Customizing.Loaders;
using Core.FileResources.Customizing.Loaders.Base;
using Core.FileResources.Customizing.Requesters;
using Core.FileResources.Customizing.Requesters.Base;
using Core.FileResources.Customizing.Validators;
using Core.FileResources.Customizing.Validators.Base;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Core.FileResources.Customizing
{
	public class ResourceCustomizer : IResourceCustomizer
	{
		private readonly IDictionary<ResourceCategory, ResourceCustomizeItem> _map;
		
		public ResourceCustomizer()
		{
			_map = new Dictionary<ResourceCategory, ResourceCustomizeItem>();
			CustomizeAll();
		}
		
		public string GetDirectoryName(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) ? item.DirectoryName : null;
		
		public bool IsRequestable(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) && item.IsRequestable;
		
		public bool IsCacheable(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) && item.Loader != null;
		
		public bool IsConvertable(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) && item.IsConvertable;
		
		public bool IsValidatable(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) && item.IsValidatable;
		
		public bool IsDestructible(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) && item.IsDestructible;
		
		public IResourceRequester<TResource> GetRequester<TResource>(ResourceCategory category) =>
			IsRequestable(category) ? ResourceCustomizeItem<TResource>.Requester : null;
		
		public IResourceLoader GetLoader(ResourceCategory category) =>
			_map.TryGetValue(category, out var item) ? item.Loader : null;
		
		public IResourceSerializer<TResource> GetSerializer<TResource>(ResourceCategory category) =>
			IsConvertable(category) ? ResourceCustomizeItem<TResource>.Serializer : null;
		
		public IResourceDeserializer<TResource> GetDeserializer<TResource>(ResourceCategory category) =>
			IsConvertable(category) ? ResourceCustomizeItem<TResource>.Deserializer : null;
		
		public IResourceValidator<TResource> GetValidator<TResource>(ResourceCategory category) =>
			IsValidatable(category) ? ResourceCustomizeItem<TResource>.Validator : null;
		
		public IResourceDestructor<TResource> GetDestructor<TResource>(ResourceCategory category) =>
			IsValidatable(category) ? ResourceCustomizeItem<TResource>.Destructor : null;
		
		private void CustomizeAll()
		{
			Texture.allowThreadedTextureCreation = true;
			
			var bytesRequester = new BytesRequester();
			var bytesLoader = new BytesLoader(4096);
			var bytesSerializer = new BytesSerializer();
			var bytesDeserializer = new BytesDeserializer();
			
			CustomizeItem(ResourceCategory.File, bytesRequester, bytesLoader, bytesSerializer, bytesDeserializer);
			
			CustomizeItem(ResourceCategory.Text,
				new TextRequester(),
				bytesLoader,
				new TextSerializer(Encoding.UTF8),
				new TextDeserializer(Encoding.UTF8));
			
			CustomizeItem(ResourceCategory.Audio, bytesRequester, bytesLoader, bytesSerializer, bytesDeserializer);
			
			CustomizeItem(ResourceCategory.Video, bytesRequester, bytesLoader, bytesSerializer, bytesDeserializer);
			
			CustomizeItem(ResourceCategory.Texture2D,
				new Texture2DRequester(false),
				bytesLoader,
				new Texture2DSerializer(GraphicsFormatUtility.GetGraphicsFormat(TextureFormat.RGBA32, true),
					TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate),
				new Texture2DDeserializer(),
				new Texture2DValidator(),
				new Texture2DDestructor());
		}
		
		private void CustomizeItem<TResource>(ResourceCategory category,
			IResourceRequester<TResource> requester,
			IResourceLoader loader,
			IResourceSerializer<TResource> serializer, 
			IResourceDeserializer<TResource> deserializer,
			IResourceValidator<TResource> validator = null,
			IResourceDestructor<TResource> destructor = null)
		{
			var item = new ResourceCustomizeItem
			{
				Category = category,
				DirectoryName = Enum.GetName(typeof(ResourceCategory), category)?.ToLower()
			};

			if (requester != null)
			{
				ResourceCustomizeItem<TResource>.Requester = requester;
				item.IsRequestable = true;
			}
			
			if (serializer != null && deserializer != null)
			{
				ResourceCustomizeItem<TResource>.Serializer = serializer;
				ResourceCustomizeItem<TResource>.Deserializer = deserializer;
				item.IsConvertable = true;
			}
			
			if (loader != null)
				item.Loader = loader;
			
			if (validator != null)
			{
				ResourceCustomizeItem<TResource>.Validator = validator;
				item.IsValidatable = true;
			}

			if (destructor != null)
			{
				ResourceCustomizeItem<TResource>.Destructor = destructor;
				item.IsDestructible = true;
			}
			
			_map.Add(category, item);
		}
		
		private struct ResourceCustomizeItem
		{
			internal ResourceCategory Category { get; set; }
			internal string DirectoryName { get; set; }
			internal bool IsRequestable { get; set; }
			internal IResourceLoader Loader { get; set; }
			internal bool IsConvertable { get; set; }
			internal bool IsValidatable { get; set; }
			internal bool IsDestructible { get; set; }
		}
		
		private static class ResourceCustomizeItem<TResource>
		{
			internal static IResourceRequester<TResource> Requester { get; set; }
			internal static IResourceSerializer<TResource> Serializer { get; set; }
			internal static IResourceDeserializer<TResource> Deserializer { get; set; }
			internal static IResourceValidator<TResource> Validator { get; set; }
			internal static IResourceDestructor<TResource> Destructor { get; set; }
		}
	}
}