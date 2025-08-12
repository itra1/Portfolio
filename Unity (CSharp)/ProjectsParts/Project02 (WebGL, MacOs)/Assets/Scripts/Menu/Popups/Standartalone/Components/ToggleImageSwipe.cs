using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Custom/ToggleImagesSwipe")]
[ExecuteAlways]
public class ToggleImageSwipe : MonoBehaviour, IPointerClickHandler
{
	public UnityEvent OnEnabled;
	public UnityEvent<ToggleImageSwipe> OnValueChanged;
	[SerializeField]
	private bool isOn;
	[SerializeField]
	private bool chageGameObject;
	[SerializeField]
	SwiperImageBase swiper;
	public bool IsOn
	{
		get
		{
			return isOn;
		}
		set
		{
			isOn = value;
			SetGraphic();
			swiper?.Swipe(IsOn);

		}
	}
	[SerializeField]
	private Sprite EnabledImg;
	[SerializeField]
	private Sprite DisabledImg;

	[SerializeField]
	private Image targetImage;

	[SerializeField]
	private bool useOpacityInsteadOfImages = false;
	[SerializeField]
	private float DisabledOpacity = 0.7f;
	public void OnPointerClick(PointerEventData eventData)
	{
		IsOn = !IsOn;
		OnValueChanged.Invoke(this);
		if (IsOn)
		{
			OnEnabled.Invoke();
		}

	}
	private void SetGraphic()
	{
		if (targetImage != null)
		{
			if (useOpacityInsteadOfImages)
			{
				var color = targetImage.color;
				color.a = isOn == true ? 1 : DisabledOpacity;
				targetImage.color = color;
			}
			else
			{
				targetImage.sprite = isOn == true ? EnabledImg : DisabledImg;
			}

		}
	}
	private void Awake()
	{
		SetGraphic();
	}
}



