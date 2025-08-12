using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AnimationPacker))]
public class AnimationPackerEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Pack")) {
			((AnimationPacker)target).Packet();
		}

		if(GUILayout.Button("RemoveChildren")) {
			((AnimationPacker)target).Packet();
		}

	}

}

#endif


public class AnimationPacker : MonoBehaviour {

	public void Packet() {

#if UNITY_EDITOR

		RuntimeAnimatorController cont = GetComponent<Animator>().runtimeAnimatorController;

		AnimationClip[] animClip = cont.animationClips;

		for(int i = 0; i < animClip.Length; i++) {
			AssetDatabase.AddObjectToAsset(Instantiate(animClip[i]), cont);
		}

#endif

	}

	public void RemoveChildren() {
	}

}
