using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Common
{
	public interface IPackageProcess
	{
		public abstract Task<Package> Process();
	}
}
