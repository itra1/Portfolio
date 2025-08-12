using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGui.Screens.Common
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ScreenAttribute : System.Attribute
	{
		public string Name { get; private set; }

		public ScreenAttribute(string name)
		{
			Name = name;
		}
	}
}
