using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Core.Engine.uGUI.Popups
{
	public interface IPopupSettings
	{
		public string PopupFolder { get; }
		public AnimationCurve PopupCurveAnimation { get; }
	}
}
