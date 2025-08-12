using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using it.UI.Elements;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(HoverAnimation))]
public class HoverAnimationEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Create animator"))
		{
		((HoverAnimation)target).CreateAnimationAssets();

		}

	}

}

#endif

namespace it.UI.Elements
{
	[RequireComponent(typeof(Animator))]
	public class HoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private string _parametr = "hover";

		private Animator _animator;

		//private void CreateAsset(){
		//EditorUtility.SaveFilePanel
		//}

		private void Start()
		{
			
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_animator == null)
				_animator = GetComponent<Animator>();

			_animator.SetBool(_parametr, true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_animator == null)
				_animator = GetComponent<Animator>();

			_animator.SetBool(_parametr, false);
		}

#if UNITY_EDITOR

		public void CreateAnimationAssets()
		{

			if (_animator == null)
				_animator = GetComponent<Animator>();

			if (_animator.runtimeAnimatorController != null)
				return;

			string pathSave = EditorUtility.SaveFilePanel("Create animator", Application.dataPath + "/Content/Animations", gameObject.name, "controller");
			string path = pathSave.Substring(Application.dataPath.Length - 6);
			string[] pathArr = path.Split('/');
			var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
			AnimationClip clip = UnityEditor.Animations.AnimatorController.AllocateAnimatorClip(_parametr);
			clip.wrapMode = WrapMode.Loop;
			AssetDatabase.CreateAsset(clip, path.Substring(0,path.Length- pathArr[pathArr.Length-1].Length) + pathArr[pathArr.Length - 1].Substring(0, pathArr[pathArr.Length - 1].Length-11) +"_"+ _parametr + ".anim");

			var rootStateMachine = controller.layers[0].stateMachine;
			var empty = rootStateMachine.AddState("Empty");
			var anim = rootStateMachine.AddState("Animation");
			anim.motion = clip;
			var tr = rootStateMachine.AddAnyStateTransition(anim);
			tr.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, _parametr);
			tr.duration = 0.1f;
			tr.canTransitionToSelf = false;

			var exAnim = anim.AddTransition(empty);
			exAnim.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, _parametr);
			exAnim.hasExitTime = false;

			controller.AddParameter(_parametr, AnimatorControllerParameterType.Bool);

			GetComponent<Animator>().runtimeAnimatorController = controller;
		}

#endif
	}
}