namespace Core.FileResources.Customizing.Validators.Base
{
	public interface IResourceValidator<in TResource>
	{
		bool IsValid(TResource resource);
	}
}