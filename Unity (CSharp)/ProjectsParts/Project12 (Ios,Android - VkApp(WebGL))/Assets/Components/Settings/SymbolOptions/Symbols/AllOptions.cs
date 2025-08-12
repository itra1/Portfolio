using Settings.SymbolOptions.Base;
using System.Collections.Generic;

namespace Settings.SymbolOptions.Symbols {
	public class ServersOption : IDropdownDefine {
		public string Description => "Режимы";

		public Dictionary<string, string> DefineDict => new Dictionary<string, string>() {
		{"Стандартный","MODE_STANDART" },
		{"Биткойн","MODE_BITCOIN" }
		};

		public bool MayByNone => false;

		public void AfterChange(string value) {
		}
	}
}
