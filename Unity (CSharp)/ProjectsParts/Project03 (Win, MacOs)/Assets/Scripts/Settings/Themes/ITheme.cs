using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Settings.Themes
{
	public interface ITheme
	{
		public Color StandartColor { get; }
		public Color FocusColor { get; }
		public Color ErrorColor { get; }
		public Color PlaceholderColor { get; }
	}
}
