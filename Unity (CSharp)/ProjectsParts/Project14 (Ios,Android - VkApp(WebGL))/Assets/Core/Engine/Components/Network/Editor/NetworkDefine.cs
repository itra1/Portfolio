using System.Collections.Generic;
using Core.Engine.App.Base.Attributes.Defines;

namespace Core.Engine.Modules.Network
{
	public class NetworkDefine :IDropdownDefine
	{
		public Dictionary<string,string> DefineDict => new(){ { "��������� ������", "SERVER_LOCAL" }
																	, { "����������� ������", "SERVER_PRODUCT" }};

		public string Description => "��������� �������";

		public bool MayByNone => true;

		public void AfterDisable() { }

		public void AfterEnable() { }

	}
}