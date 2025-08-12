using System.Collections;
using Core.Engine.Components.Themes;
using NUnit.Framework;
using Scripts.Elements.Themes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

public class ThemeTests
{
	private DiContainer _container;
	[OneTimeSetUp]
	public void OneTapSetup()
	{
		SceneManager.LoadScene("Game",LoadSceneMode.Single);
		_container = new DiContainer(StaticContext.Container);
	}

	[UnityTest]
	public IEnumerator ActivateItemsList()
	{
		var themeProvider = _container.TryResolve<IThemeProvider>();
		var component = GameObject.FindAnyObjectByType<GameThemeComponents>();

		yield return new WaitForSeconds(2);

		var themeList = themeProvider.GetList;

		Debug.Log("Themes count " + themeList.Count);

		for (int i = 0; i < themeList.Count; i++)
		{
			var themeItem = themeList[i];
			Debug.Log("Iter " + themeItem.UUID);

			Assert.IsTrue(themeItem.Confirm(component));

			yield return new WaitForSeconds(1);
		}
	}
}
