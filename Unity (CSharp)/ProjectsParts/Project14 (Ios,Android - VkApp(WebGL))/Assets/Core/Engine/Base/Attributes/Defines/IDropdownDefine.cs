using System.Collections.Generic;

namespace Core.Engine.App.Base.Attributes.Defines
{
	public interface IDropdownDefine :IDefine
	{
		public Dictionary<string,string> DefineDict { get; }

		bool MayByNone { get; }
	}
}
