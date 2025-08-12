using UnityEngine;

namespace Itra.Utils
{
	public static class MeshRendererUtils
	{

		/// <summary>
		/// Ресайз рендерера по камере
		/// </summary>
		/// <param name="component"></param>
		public static void FillScreenPerspectiveCamera(this MeshRenderer component)
		{
			var cameraMain = Camera.main;

			var screenSize = cameraMain.PerspectiveVisibleRect(Vector3.Distance(component.transform.position, cameraMain.transform.position));

			component.transform.localScale = new Vector3(screenSize.x, screenSize.y, 1);
		}
	}
}
