using System.Collections.Generic;

namespace Settings.SymbolOptions.Base {
	public interface IDropdownDefine : IDefine {
		public Dictionary<string, string> DefineDict { get; }

		bool MayByNone { get; }
		void AfterChange(string value);
	}
}
