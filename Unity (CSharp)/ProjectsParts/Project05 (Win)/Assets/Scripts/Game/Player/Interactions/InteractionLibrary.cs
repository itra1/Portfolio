using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace it.Game.Player.Interactions
{
  [CreateAssetMenu(fileName = "InteractionLibrary", menuName = "ScriptableObject/Interaction Library", order = 0)]
  public class InteractionLibrary : ScriptableObject
  {
	 [SerializeField]
	 private List<InteractionMotion> _interactionMotionsLibrary = new List<InteractionMotion>();

	 public InteractionMotion Get(InteractionMotion.MotionType type)
	 {
		return _interactionMotionsLibrary.Find(x => x.Motion.Equals(type));
	 }

	 [ContextMenu("Find")]
	 public void Find()
	 {
		InteractionMotion[] dialogs = Resources.LoadAll<InteractionMotion>(Game.ProjectSettings.InteractionMotionsFolder);
		_interactionMotionsLibrary = dialogs.ToList();
		//_dialogLibrary = EditorUtil.GetAllPrefabsOfType<GuiDialog>();
	 }

  }
}