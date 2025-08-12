using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class EnemyInfo : MonoBehaviour {

	public Sprite icon;
	public EnemyType type;
	public SkeletonDataAsset skeletonDataAsset;

	[SpineAnimation(dataField: "skeletonDataAsset")]
	public string runAnim = "";     // Анимация хотьбы

}
