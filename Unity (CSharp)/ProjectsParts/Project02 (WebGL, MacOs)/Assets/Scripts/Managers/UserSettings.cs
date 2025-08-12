using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace it.Settings
{
	[CreateAssetMenu(fileName = "UserSettings", menuName = "Tools/Create user settings", order = 1)]
	public class UserSettings : ScriptableObject
	{
//#if UNITY_WEBGL
//		public static UserSettings Instance => Garilla.WebGL.WebGLResources.UserSettings;
//#else
		private static UserSettings _instance;
		public static UserSettings Instance
		{
			get
			{
				if (_instance == null)
					_instance = (UserSettings)Garilla.ResourceManager.GetResource<UserSettings>("UserSettings");
				//_instance = Resources.Load<UserSettings>("UserSettings");

				return _instance;
			}
		}
//#endif

		public static List<RenkData> Ranks => Instance._ranks;

		[SerializeField] private List<RenkData> _ranks;


		[System.Serializable]
		public class RenkData
		{
			public string Title;
			public ulong Id;
			public Sprite Card;
			public string LocalTitle;
		}

	}
}