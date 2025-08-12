using Builder.Common;
using Builder.Views;

namespace Builder.Controllers
{
	public class ReleasesViewController : ViewController<ReleasesView>
	{
		public ReleasesViewController(BuildSession session) : base(session)
		{
		}
	}
}
