using Game.Base;
using TMPro;
using UnityEngine;

namespace Game.Providers.Ui.Elements
{
	public class GameSelectButton : MonoBehaviour, IInjection
	{
		[SerializeField] private GameObject _selectImage;
		[SerializeField] private TMP_Text _label;
		[SerializeField] private Material _baseMaterial;
		[SerializeField] private Material _selectMaterial;
		[SerializeField] private Color _baseColor;
		[SerializeField] private Color _selectColor;

		public void SetSelect(bool isSelect)
		{
			_selectImage.gameObject.SetActive(isSelect);
			_label.color = isSelect ? _selectColor : _baseColor;
			_label.fontMaterial = isSelect ? _selectMaterial : _baseMaterial;
		}
	}
}
