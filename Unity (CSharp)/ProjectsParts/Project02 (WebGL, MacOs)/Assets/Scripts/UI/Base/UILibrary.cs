using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sett = it.Settings;
using it.Popups;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace it.UI
{
	/// <summary>
	/// Бибилиотека UI
	/// </summary>
	[CreateAssetMenu(fileName = "UILibrary", menuName = "ScriptableObject/UILibrary", order = 0)]
	public class UILibrary : ScriptableObject
	{
		public List<UIPanel> DialogLibrary { get { return _dialogLibrary; } }
		public List<PopupBase> PopupLibrary { get { return _popupsLibrary; } }


		[SerializeField] private List<UIPanel> _dialogLibrary;
		[SerializeField] private List<PopupBase> _popupsLibrary;

#if UNITY_EDITOR

		//[MenuItem("Assets/Custom/UiLibrary")]

		[ContextMenu("Find UiLibrary")]
		public void FindObjects()
		{
			UIPanel[] dialogs = Garilla.ResourceManager.GetResourceAll<UIPanel>(Sett.AppSettings.Folders.UIPanels);
			_dialogLibrary = dialogs.ToList();
			//_dialogLibrary = EditorUtil.GetAllPrefabsOfType<GuiDialog>();
		}


#endif


	}

}