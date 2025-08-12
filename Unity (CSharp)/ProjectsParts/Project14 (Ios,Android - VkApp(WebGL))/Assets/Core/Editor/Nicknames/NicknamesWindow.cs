using UnityEditor;
using UnityEngine;

namespace Core.Editor.Nicknames
{
	public class NicknamesWindow :EditorWindow
	{
		[MenuItem("App/Nicknames list")]
		public static void OpenSetting()
		{
			var w = ScriptableObject.CreateInstance<NicknamesWindow>();
			w.Show();
		}
	}
}
