using System.Collections.Generic;

namespace Editor.Settings.Base
{
	public interface IDropdownDefine : IDefine
	{
		public Dictionary<string,string> Defines { get; }

		bool MayByNone { get; }
	}
}
