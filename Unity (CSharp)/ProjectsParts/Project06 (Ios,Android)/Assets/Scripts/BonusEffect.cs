using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusEffect : MonoBehaviour {

	public TextMesh textValue;

	public void SetValue(int value) {
		textValue.text = "+" + value.ToString();
		textValue.GetComponent<MeshRenderer>().material.renderQueue = 10010;
	}
}
