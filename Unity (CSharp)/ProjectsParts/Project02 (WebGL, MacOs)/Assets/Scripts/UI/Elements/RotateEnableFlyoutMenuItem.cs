using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.Elements
{
	public class RotateEnableFlyoutMenuItem
	{
		public RotateEnableFlyoutMenuItem()
		{
			TargetType = typeof(RotateEnableFlyoutMenuItem);
		}
		public int Id { get; set; }
		public string Title { get; set; }

		public Type TargetType { get; set; }
	}
}