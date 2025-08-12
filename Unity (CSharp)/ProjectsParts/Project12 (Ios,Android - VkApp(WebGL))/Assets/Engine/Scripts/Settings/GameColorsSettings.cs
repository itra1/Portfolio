using System;
using System.Collections.Generic;
using Engine.Scripts.Base;
using StringDrop;
using UnityEngine;

namespace Engine.Scripts.Settings
{
	[Serializable]
	public class GameColorsSettings
	{
		[SerializeField] private List<AppColors> _appColors;

		public List<AppColors> AppColors1 => _appColors;

		[Serializable]
		public struct AppColors
		{
			[SerializeField][StringDropList(typeof(ColorTypes))] private string _type;
			[SerializeField] private Color _color;

			public string Type => _type;
			public Color Color => _color;

		}
	}
}
