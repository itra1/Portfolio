using System.Collections.Generic;
using Core.Engine.App.Base.Attributes.Defines;

namespace Core.Engine.Modules.Network
{
	public class NetworkDefine :IDropdownDefine
	{
		public Dictionary<string,string> DefineDict => new(){ { "Локальный сервер", "SERVER_LOCAL" }
																	, { "Продуктовый сервер", "SERVER_PRODUCT" }};

		public string Description => "Настройки сервера";

		public bool MayByNone => true;

		public void AfterDisable() { }

		public void AfterEnable() { }

	}
}