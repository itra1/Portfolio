using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BundleSpriteLoader : MonoBehaviour {

	public string imageName;

	private SpriteRenderer _spriteRenderer;

	private SpriteRenderer spriteRenderer {
		get {

			if (_spriteRenderer == null)
				_spriteRenderer = GetComponent<SpriteRenderer>();

			return _spriteRenderer;
		}
	}

	private void Start() {
		try {
			spriteRenderer.sprite = GraphicManager.Instance.link.singleImages.Find(x => x.name == imageName);
		}
		catch {
			Debug.LogError("No exists sprite asset " + imageName);
		}
	}

}
