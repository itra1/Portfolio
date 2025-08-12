using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BundleImageLoader : MonoBehaviour {

	public string imageName;

	private Image _image;

	private Image image {
		get {

			if (_image == null)
				_image = GetComponent<Image>();

			return _image;
		}
	}

	private void Start() {
		try {
			image.sprite = GraphicManager.Instance.link.singleImages.Find(x => x.name == imageName);
		} catch {
			Debug.LogError("No exists sprite asset " + imageName);
		}
	}
}
