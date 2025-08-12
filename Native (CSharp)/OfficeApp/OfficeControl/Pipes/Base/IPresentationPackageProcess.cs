using OfficeControl.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Base
{
	public interface IPresentationPackageProcess
	{
		public abstract Package Process(IPowerPoint presentation);
	}
}
