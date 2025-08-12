using System.Collections.Generic;
using Editor.Settings.Base;

namespace Editor.Settings.Defines
{
	public class Servers : IDropdownDefine
	{
		public Dictionary<string, string> Defines => new()
		{
			{ "Local", "SERVER_LOCAL" },
			{ "Development 1", "SERVER_DEV" },
			{ "Development 2", "SERVER_DEV_2" },
			{ "Stage", "SERVER_STAGE" },
			{ "Product", "SERVER_PRODUCT" },
			{ "Sum expo", "SERVER_SUMEXPO" },
			{ "Sum expo mini", "SERVER_SUMEXPOMINI" },
			{ "Syn", "SERVER_SYN" }
		};
		
		public bool MayByNone => false;
		
		public string Description => "Server";
		
		public void AfterDisable() { }
		
		public void AfterEnable() { }
	}
}
