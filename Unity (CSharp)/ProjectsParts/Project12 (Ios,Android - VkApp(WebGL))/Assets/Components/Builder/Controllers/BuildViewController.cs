using Builder.Common;
using Builder.Views;

namespace Builder.Controllers
{
	public class BuildViewController : ViewController<BuildView>
	{
		public BuildViewController(BuildSession session) : base(session)
		{
		}
	}
}
