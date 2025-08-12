namespace Core.FileResources.Customizing.Destructors.Base
{
	public interface IResourceDestructor<in TResource>
	{
		void Destruct(TResource resource);
	}
}