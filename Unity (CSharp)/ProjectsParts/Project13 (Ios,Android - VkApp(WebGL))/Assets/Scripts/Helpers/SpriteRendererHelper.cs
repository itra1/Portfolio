using UnityEngine;

public static class SpriteRendererHelper {

	/// <summary>
	/// Изменяем скейл компонента в размер ортографической камеры
	/// </summary>
	/// <param name="component">Сам компонент</param>
	public static void SpriteScaleToOrtoScreenVisible(this SpriteRenderer component) {

		if (component.sprite == null) {
			Debug.LogError("Отсутствует ссылка на картинку");
			return;
		}

		var cameraMain = Camera.main;

		if (!cameraMain.orthographic) {
			Debug.LogError("Камера должна быть ортографическая");
			return;
		}

		var screenProportion = (float)cameraMain.pixelRect.size.x / (float)cameraMain.pixelRect.size.y;
		var screenSize = new Vector2(cameraMain.orthographicSize * screenProportion * 2, cameraMain.orthographicSize * 2);
		var ppu = component.sprite.pixelsPerUnit;
		var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
		component.transform.localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
	}

	/// <summary>
	/// Изменяем скейл компонента в размер ортографической камеры
	/// </summary>
	/// <param name="component">Сам компонент</param>
	public static void SpriteSliceSizeScreenVisible(this SpriteRenderer component) {

		if (component.sprite == null) {
			Debug.LogError("Отсутствует ссылка на картинку");
			return;
		}

		var cameraMain = Camera.main;

		if (!cameraMain.orthographic) {
			Debug.LogError("Камера должна быть ортографическая");
			return;
		}

		var screenProportion = (float)cameraMain.pixelRect.size.x / (float)cameraMain.pixelRect.size.y;
		var screenSize = new Vector2(cameraMain.orthographicSize * screenProportion * 2, cameraMain.orthographicSize * 2);
		var ppu = component.sprite.pixelsPerUnit;
		var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
		component.size = screenSize;
		//component.transform.localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
	}
	public static void SpritSliceSizeWithMinSazeScreenVisible(this SpriteRenderer component) {

		if (component.sprite == null) {
			Debug.LogError("Отсутствует ссылка на картинку");
			return;
		}

		var cameraMain = Camera.main;

		if (!cameraMain.orthographic) {
			Debug.LogError("Камера должна быть ортографическая");
			return;
		}

		var screenProportion = (float)cameraMain.pixelRect.size.x / (float)cameraMain.pixelRect.size.y;
		var screenSize = new Vector2(cameraMain.orthographicSize * screenProportion * 2, cameraMain.orthographicSize * 2);
		var ppu = component.sprite.pixelsPerUnit;
		var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
		Vector2 localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
		var minSize = Mathf.Min(localScale.x, localScale.y);
		component.transform.localScale = new Vector3(minSize, minSize, 1);
		var size = new Vector2(screenSize.x / minSize, screenSize.y / minSize);
		component.size = size;
	}

}

