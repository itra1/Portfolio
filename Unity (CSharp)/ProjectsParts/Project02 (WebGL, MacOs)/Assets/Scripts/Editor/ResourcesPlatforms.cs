using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Garilla.Editor
{
	[ExecuteInEditMode]
	public class ResourcesPlatforms : MonoBehaviour
	{
		private static string PCResources => Application.dataPath + "/PlatformsResources/ResPC";
		private static string AndroidResources => Application.dataPath + "/PlatformsResources/ResAndroid";
		private static string Resources => Application.dataPath + "/PlatformsResources/Resources";

		[InitializeOnEnterPlayMode]
		public static async Task ReadyResources()
		{
			RenameResourceGamePlayMode();
			await WaitExitPlayMode();
		}

		public async static Task WaitExitPlayMode()
		{
			while (!EditorApplication.isPlaying)
			{
				await Task.Delay(100);
			}
			while (EditorApplication.isPlaying)
			{
				await Task.Delay(1000);
			}
			RenameResourceEditorPlayMode();
		}

		public static void RenameResourceGamePlayMode()
		{
			it.Logger.Log("RenameResourceGamePlayMode");
			Directory.Move(PCResources, Resources);
		}

		public static void RenameResourceEditorPlayMode()
		{
			it.Logger.Log("RenameResourceEditorPlayMode");
			Directory.Move(Resources, PCResources);
		}


	}
}