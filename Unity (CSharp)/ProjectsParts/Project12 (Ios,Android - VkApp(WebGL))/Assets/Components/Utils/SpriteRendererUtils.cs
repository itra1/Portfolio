using UnityEngine;

namespace Itra.Utils {
	/// <summary>
	/// SpriteREnderer дополнительный инструментарий
	/// </summary>
	public static class SpriteRendererUtils {

		/// <summary>
		/// Изменяем скейл компонента в размер ортографической камеры
		/// </summary>
		/// <param name="component">Сам компонент</param>
		public static void ScaleToOrtoScreenVisible(this SpriteRenderer component, Camera camera) {

			if (component.sprite == null) {
				Debug.LogError("Отсутствует ссылка на картинку");
				return;
			}

			var screenSize = camera.OrthographicVisibleRect();
			var ppu = component.sprite.pixelsPerUnit;
			var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
			component.transform.localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
		}

		/// <summary>
		/// Изменяем скейл компонента в размер ортографической камеры
		/// </summary>
		/// <param name="component">Сам компонент</param>
		public static void SliceSizeOrtoScreenVisible(this SpriteRenderer component, Camera camera) {

			if (component.sprite == null) {
				Debug.LogError("Отсутствует ссылка на картинку");
				return;
			}

			var screenSize = camera.OrthographicVisibleRect();
			var ppu = component.sprite.pixelsPerUnit;

			_ = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
			component.size = screenSize;
			//component.transform.localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
		}
		public static void SpritSliceSizeWithMinSazeScreenVisible(this SpriteRenderer component) {

			if (component.sprite == null) {
				Debug.LogError("Отсутствует ссылка на картинку");
				return;
			}

			var camera = Camera.main;

			var screenSize = camera.OrthographicVisibleRect();
			var ppu = component.sprite.pixelsPerUnit;
			var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
			Vector2 localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
			var minSize = Mathf.Min(localScale.x, localScale.y);
			component.transform.localScale = new Vector3(minSize, minSize, 1);
			var size = new Vector2(screenSize.x / minSize, screenSize.y / minSize);
			component.size = size;
		}

		/// <summary>
		/// Ресайз рендерера по камере
		/// </summary>
		/// <param name="component"></param>
		public static void FillScreenPerspectiveCamera(this SpriteRenderer component) {
			var cameraMain = Camera.main;

			var screenSize = cameraMain.PerspectiveVisibleRect(Vector3.Distance(component.transform.position, cameraMain.transform.position));

			var ppu = component.sprite.pixelsPerUnit;
			var sizeSprite = new Vector2(component.sprite.rect.width / ppu, component.sprite.rect.height / ppu);
			component.transform.localScale = new Vector3(screenSize.x / sizeSprite.x, screenSize.y / sizeSprite.y, 1);
		}
	}
}