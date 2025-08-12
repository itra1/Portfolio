using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace it.UI.Elements
{
	public class InteractableActivationObject : MonoBehaviour
	{
		[SerializeField] private GameObject _object;
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private bool _inverce = true;
		[SerializeField] private HoverAnimation _hiverAnimation;
		private Selectable _selectable;

		public bool Interactable
		{
			get
			{
				if (_selectable == null)
					_selectable = GetComponent<Selectable>();
				return _selectable.interactable;
			}
			set
			{
				if (_selectable == null)
					_selectable = GetComponent<Selectable>();
				_selectable.interactable = value;

				if (_hiverAnimation != null)
					_hiverAnimation.enabled = value;

				if (_object != null)
					_object.SetActive(_inverce ? !_selectable.interactable : _selectable.interactable);

				if (_label != null)
					_label.color = _selectable.interactable ? (_inverce ? Color.gray : Color.white) : (_inverce ? Color.white : Color.gray);
			}
		}

	}
}