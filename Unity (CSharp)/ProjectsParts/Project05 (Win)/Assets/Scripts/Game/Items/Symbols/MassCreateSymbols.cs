using UnityEngine;
using System.Collections;
namespace it.Game.Items.Symbols
{
  public class MassCreateSymbols : MonoBehaviourBase
  {

	 [SerializeField]
	 private Sprite[] _sprites;
	 [SerializeField]
	 private UnityEngine.VFX.VisualEffectAsset[] _vfx;


	 [ContextMenu("Create Symbols")]
	 public void SreateSymbols()
	 {
		foreach (var elem in _sprites)
		  CreateOneSymbol(elem, GetVFX(elem.name));
	 }


	 private void CreateOneSymbol(Sprite sprite, UnityEngine.VFX.VisualEffectAsset vfxAsset)
	 {
		if(vfxAsset == null)
		{
		  Debug.Log("No vfx to " + sprite.name);
		}

		var go = CreateGameObject(transform);
		go.name = sprite.name;

		var symb = go.AddComponent<Symbol>();
		symb.SetIcon(sprite);
		symb.SetTitle(sprite.name);

		var goVFX = CreateGameObject(go.transform);
		goVFX.name = "VFX";
		var vfx = goVFX.AddComponent<UnityEngine.VFX.VisualEffect>();
		vfx.visualEffectAsset = vfxAsset;

		go.AddComponent<BoxCollider>();

	 }

	 private UnityEngine.VFX.VisualEffectAsset GetVFX(string name)
	 {

		foreach (var elem in _vfx)
		  if (name == elem.name)
			 return elem;
		return null;
	 }

  }
}