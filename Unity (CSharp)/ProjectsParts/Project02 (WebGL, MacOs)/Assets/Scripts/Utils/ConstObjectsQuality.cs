using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ConstObjectsQuality : MonoBehaviour {

	public string spriteSheet;

	private string qSuffix;
	
	void Awake(){
		qSuffix = GetQuality ();
		ManageQuality ();
	}

	private void Update()
	{
		var suffix = GetQuality();
		if (qSuffix != suffix)
		{
			qSuffix = suffix;
			ManageQuality();
		}
	}

	private string GetQuality(){
		int screenW = Screen.width;
		if (screenW > 1920)
			return "4x";
		if (screenW < 1400)
			return "1x";
		return "2x";
	}


	private void ManageQuality(){
			Sprite[] sprites = Garilla.ResourceManager.GetResourceAll<Sprite>(spriteSheet + "@" + qSuffix);

			if (sprites != null) {
				Image[] renderers = GetComponentsInChildren<Image>(true);
				
				if (renderers.Length > 0) {

					foreach (Image r in renderers)
					{
						if (r.name != null){
							string spriteName = r.sprite.name;
							Sprite newSprite = Array.Find(sprites, item => item.name == spriteName);
							
							if (newSprite)
								r.sprite = newSprite;
						}
					}
				}
			}

		Resources.UnloadUnusedAssets ();

	}

}