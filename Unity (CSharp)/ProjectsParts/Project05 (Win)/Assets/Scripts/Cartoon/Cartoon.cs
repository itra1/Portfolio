using UnityEngine;
using System.Collections.Generic;
using it.Game.Managers;

namespace it.Cartoons
{
  public class Cartoon : UUIDBase
  {
	 public static CartoonLibrary _library;
	 public static List<Cartoon> _cartoonsList = new List<Cartoon>();
	 public bool fideIn;
	 public bool fideOut;

	 public static CartoonLibrary Library
	 {
		get
		{
		  if (_library == null)
			 _library = Resources.Load<CartoonLibrary>(Game.ProjectSettings.CartoonLibrary);
		  return _library;
		}
	 }

	 public UnityEngine.Events.UnityAction onComplete;

	 public UnityEngine.Events.UnityAction onShowComplete;

	 public static Cartoon Get(string uuid)
	 {
		Cartoon cart = _cartoonsList.Find(x => x.Uuid.Equals(uuid));

		if (cart == null)
		{
		  GameObject pref = Resources.Load<GameObject>(Library.GetPath(uuid));

		  if(pref == null)
		  {
			 Debug.LogError("Нет видеоката с " + uuid);
			 return null;
		  }

		  cart = pref.GetComponent<Cartoon>();
		  _cartoonsList.Add(cart);
		}

		return cart;
	 }


	 public static void Play(string uuid,bool fideIn, bool fideOut, UnityEngine.Events.UnityAction onComplete)
	 {
		Cartoon cartPrefab = Get(uuid);

		if (cartPrefab == null)
		  return;

		cartPrefab.fideIn = fideIn;
		cartPrefab.fideOut = fideOut;

		var panel = UiManager.GetPanel<it.UI.Cartoon.CartoonDialog>();
		panel.gameObject.SetActive(true);
		cartPrefab.onComplete = () => {
		  panel.gameObject.SetActive(false);
		  onComplete?.Invoke();
		};
		panel.Play(cartPrefab);
	 }

	 public void Complete()
	 {
		onShowComplete?.Invoke();
	 }
  }
}