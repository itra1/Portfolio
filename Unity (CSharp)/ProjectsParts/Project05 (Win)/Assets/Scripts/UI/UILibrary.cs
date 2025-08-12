using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace it.UI
{
  /// <summary>
  /// Бибилиотека UI
  /// </summary>
  [CreateAssetMenu(fileName = "UILibrary", menuName = "ScriptableObject/GuiLibrary", order = 0)]
  public class UILibrary : ScriptableObject
  {
	 [SerializeField]
	 private List<UIDialog> _dialogLibrary;
	 public List<UIDialog> DialogLibrary { get { return _dialogLibrary; } }

#if UNITY_EDITOR

	 [MenuItem("Assets/Custom/UiLibrary")]

	 [ContextMenu("Find UiLibrary")]
	 public void FindObjects()
	 {
		UIDialog[] dialogs = Resources.LoadAll<UIDialog>(it.Game.ProjectSettings.GuiFolder);
		_dialogLibrary = dialogs.ToList();
		//_dialogLibrary = EditorUtil.GetAllPrefabsOfType<GuiDialog>();
	 }

	 [ContextMenu("Save")]
	 public void Save()
	 {
		EditorUtility.SetDirty(this);
	 }

#endif


  }

}