using UnityEngine;
using System.Collections;

public class MagicHitHelper : MonoBehaviour {
	public delegate void OnHitDelegate();    
	public OnHitDelegate Hit;

	void OnHit(){
		if (Hit != null)
			Hit();
	}
}
