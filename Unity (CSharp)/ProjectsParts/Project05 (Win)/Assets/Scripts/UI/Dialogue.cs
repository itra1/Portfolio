using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using I2.Loc;
using DG.Tweening;

namespace it.UI.Dialogue
{
  /// <summary>
  /// Диалоги играков
  /// </summary>
  public class Dialogue : UIDialog
  {
	 [SerializeField]
	 private BlockData _leftBlock;
	 [SerializeField]
	 private BlockData _rightBlock;
	 [SerializeField]
	 private float printSpeed = 20;
	 private bool _disableGameControll;

	 private it.Dialogue.Dialogue _dialog;
	 private int indexFrame;

	 private string _textPrint;

	 private BlockData _actualBlock;

	 private bool _showComplete = false;

	 private bool _isPrinting;

	 public void Show(it.Dialogue.Dialogue dialog)
	 {
		base.Show();

		_dialog = dialog;
		_dialog.onStart?.Invoke();
		_showComplete = true;

		Cursor.visible = true;
		indexFrame = -1;
		Next();
	 }

	 public void Next()
	 {
		if (_isPrinting)
		{
		  StopAllCoroutines();
		  _actualBlock.text.text = _textPrint;
		  _showComplete = true;
		  _isPrinting = false;
		  return;
		}

		if (!_showComplete)
		  return;

		indexFrame++;

		if(indexFrame >= _dialog._items.Length)
		{
		  _dialog.onComplete?.Invoke();

		  Cursor.visible = false;
		  Hide();
		  return;
		}

		_dialog.onNextFrame?.Invoke(indexFrame);
		ShowFrame();
	 }

	 private void ShowFrame()
	 {
		if (!_showComplete)
		  return;
		_showComplete = false;
		HideBlocks();

		it.Dialogue.DialogueItem item = _dialog._items[indexFrame];

		_actualBlock = GetBlock(item);

		_actualBlock.block.SetActive(true);
		if (item.Icone != null)
		{
		  _actualBlock.icon.sprite = item.Icone;
		  _actualBlock.icon.color = item.IconColor;
		  _actualBlock.icon.GetComponent<AspectRatioFitter>().aspectRatio = item.Icone.rect.width / item.Icone.rect.height;
		  _actualBlock.iconeParent.anchoredPosition = item.Offset;
		}
		else
		{
		  _actualBlock.icon.color = new Color(1,1,1,0);
		}
		_textPrint = LocalizationManager.GetTranslation(item.TextLangId);
		_actualBlock.text.text = "";
		StartCoroutine(PrintCoroutine());
	 }

	 IEnumerator PrintCoroutine()
	 {
		_isPrinting = true;
		for (int i = 0; i < _textPrint.Length; i++)
		{
		  _actualBlock.text.text = _textPrint.Substring(0,i);
		  yield return new WaitForSeconds(1f/ printSpeed);
		}
		_isPrinting = false;
		_showComplete = true;
	 }

	 private BlockData GetBlock(it.Dialogue.DialogueItem item)
	 {
		return item.SideBlock == it.Dialogue.DialogueItem.SideBlockType.left ? _leftBlock : _rightBlock;
	 }

	 private void HideBlocks()
	 {
		_leftBlock.block.SetActive(false);
		_rightBlock.block.SetActive(false);
	 }


	 [System.Serializable]
	 public struct BlockData
	 {
		public GameObject block;
		public Text text;
		public Image icon;
		public RectTransform iconeParent;
	 }
  }
}