using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.GameQuests
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class GameQuestTypeAttribute : Attribute
	{
		public string Name { get; private set; }

		public GameQuestTypeAttribute(string name)
		{

		}

	}
}
