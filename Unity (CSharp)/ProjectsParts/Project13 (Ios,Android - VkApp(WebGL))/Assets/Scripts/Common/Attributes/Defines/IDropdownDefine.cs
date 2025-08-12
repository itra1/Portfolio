using System.Collections.Generic;

namespace Game.Common.Attributes.Defines {
	public interface IDropdownDefine :IDefine {
		public Dictionary<string, string> DefineDict { get; }
		bool MayByNone { get; }
	}
}
