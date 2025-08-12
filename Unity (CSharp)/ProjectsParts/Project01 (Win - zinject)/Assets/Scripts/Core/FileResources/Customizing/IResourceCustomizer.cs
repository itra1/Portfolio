using Core.FileResources.Customizing.Category;
using Core.FileResources.Customizing.Converting.Deserializers.Base;
using Core.FileResources.Customizing.Converting.Serializers.Base;
using Core.FileResources.Customizing.Destructors.Base;
using Core.FileResources.Customizing.Loaders.Base;
using Core.FileResources.Customizing.Requesters.Base;
using Core.FileResources.Customizing.Validators.Base;

namespace Core.FileResources.Customizing
{
	public interface IResourceCustomizer
	{
		string GetDirectoryName(ResourceCategory category);
		bool IsRequestable(ResourceCategory category);
		bool IsCacheable(ResourceCategory category);
		bool IsConvertable(ResourceCategory category);
		bool IsValidatable(ResourceCategory category);
		bool IsDestructible(ResourceCategory category);
		IResourceLoader GetLoader(ResourceCategory category);
		IResourceRequester<TResource> GetRequester<TResource>(ResourceCategory category);
		IResourceSerializer<TResource> GetSerializer<TResource>(ResourceCategory category);
		IResourceDeserializer<TResource> GetDeserializer<TResource>(ResourceCategory category);
		IResourceValidator<TResource> GetValidator<TResource>(ResourceCategory category);
		IResourceDestructor<TResource> GetDestructor<TResource>(ResourceCategory category);
	}
}