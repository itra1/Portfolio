using System;
using UnityEngine;
using UnityEngine.UI;
//using Vectrosity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RoadPoint))]
public class RoadPointEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
    
	}
}

#endif

public class RoadPoint : MonoBehaviour {

	public Action OnClick;
  
	public enum Status {
		done, active, ready
	}

	private Status status;

	[Range(0,1)]
	public float positingPoint;
	public GameObject pathObject;

	public GameObject doneGraphic;
	public GameObject activeGraphic;
	public GameObject readyGraphic;

  public Text textNum;

  public void SetData(int num, Status status) {
    textNum.text = num.ToString();
  }
  
	public void SetStatus(Status status) {
		this.status = status;

		doneGraphic.SetActive(this.status == Status.done);
		activeGraphic.SetActive(this.status == Status.active);
		readyGraphic.SetActive(this.status == Status.ready);

	}

	public void OnClickButton() {
		if (OnClick != null)
			OnClick();
	}
	
}
