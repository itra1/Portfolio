using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.uGUI.Popups
{
	public interface IPopupProvider
	{
		Popup OpenPopup(string name, bool autoShow = true);
	}
}
