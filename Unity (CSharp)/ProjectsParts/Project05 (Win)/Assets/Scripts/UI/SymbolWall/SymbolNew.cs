using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;

#if UNITY_EDITOR

using UnityEditor;
#endif


namespace it.UI.Symbols
{
  public class SymbolNew : MonoBehaviour
  {
	 [SerializeField]
	 private RectTransform _parent;
	 // 0.1751881


	 [SerializeField]
	 private Transform _itemParent;
	 [SerializeField]
	 private SymbolAnimItem _symbol;

	 [SerializeField]
	 [ColorUsage(true,true)]
	 private Color _pictureLightColor;
	 [ColorUsage(true, true)]
	 private Color _pictureLightColorTr;
	 private Color _pictureColor;

	 private Color _symbolColor;
	 private Color _symbolColorTr;

	 [ContextMenu("Play")]
	 public void Play(it.Game.Items.Symbols.Symbol symbol, RectTransform parent, System.Action onComplete)
	 {
		_parent = parent;
		 GameObject inst = Instantiate(symbol.UiIten.gameObject, _itemParent);
		_symbol = inst.GetComponent<SymbolAnimItem>();
		_symbol.gameObject.SetActive(true);
		Animator animator = inst.GetComponent<Animator>();
		animator.SetTrigger("Play");

		Color wullColor = _symbol.Full.color;
		wullColor.a = 0;
		Color wullColorTr = wullColor;
		wullColorTr.a = 0;
		_symbol.Full.color = wullColorTr;

		_symbolColor = _symbol.Symbol.color;
		_symbolColorTr = _symbolColor;
		_symbolColor.a = 1;
		_symbolColorTr.a = 0;
		_symbol.Symbol.color = _symbolColorTr;

		_pictureColor = _symbol.Picture.material.GetColor("_ColorMain");
		_pictureLightColorTr = _pictureLightColor;
		_pictureLightColorTr.a = 0;
		_symbol.Picture.material.SetColor("_ColorMain", _pictureLightColorTr);

		DOVirtual.DelayedCall(1.3f, ()=> {
		  _symbol.Symbol.DOColor(_symbolColor, 1);
		});
		DOVirtual.DelayedCall(2.3f, () => {
		  _symbol.Picture.material.DOColor(_pictureLightColor, "_ColorMain", 0.8f).OnComplete(() => {
			 _symbol.Picture.material.DOColor(Color.white, "_ColorMain", 0.8f);
		  });
		});

		DOVirtual.DelayedCall(4,()=> {
		  RectTransform rts = _symbol.GetComponent<RectTransform>();

		  rts.SetParent(_parent);
		  rts.DOLocalMove(Vector3.zero, 0.5f);
		  rts.DOLocalRotate(Vector3.zero, 0.5f);
		  rts.DOScale(Vector3.one * 0.1751881f, 0.5f).OnComplete(() => {
			 onComplete?.Invoke();
		  });

		  _symbol.Full.DOColor(Color.white, 0.5f);

		});

	 }

#if UNITY_EDITOR

	 [ContextMenu("Create items")]
	 public void CreateItems()
	 {
		string path = Application.dataPath + "/Content/Textures/Cards";

		string[] files = Directory.GetFiles(path);

		for(int i = 0; i < files.Length; i++)
		{
		  if (files[i].Substring(files[i].Length - 4) == "meta")
			 continue;

		  files[i] = files[i].Remove(0, path.Length+1);
		  files[i] = files[i].Remove(files[i].Length - 4);

		  if (files[i].Substring(0, 4) != "Card")
			 continue;

		  files[i] = files[i].Remove(0, 5);

		  Debug.Log(files[i]);
		  CreateItem(files[i]);
		}
	 }

	 private void CreateItem(string name)
	 {
		string path = "Assets/Content/Textures/Cards/";

		string pathFullSprite = string.Format("{0}Card_{1}.png", path, name);
		Sprite fullSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(pathFullSprite);
		string pathImgSprite = string.Format("{0}Img_{1}.png", path, name);
		Sprite imgSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(pathImgSprite);
		string pathSymbolSprite = string.Format("{0}Simbol_{1}.png", path, name);
		Sprite symbolSprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(pathSymbolSprite);

		GameObject inst = Instantiate(_symbol.gameObject, _symbol.transform.parent);

		SymbolAnimItem simb = inst.GetComponent<SymbolAnimItem>();
		simb.Symbol.sprite = symbolSprite;
		simb.Picture.sprite = imgSprite;
		simb.Full.sprite = fullSprite;
		inst.name = name + "Symbol";
	 }

#endif

  }
}