using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ImageSwiperGroup : MonoBehaviour
{
	public List<ToggleImageSwipe> elements = new List<ToggleImageSwipe>();
	public void SetGroup(List<ToggleImageSwipe> list)
	{
		elements.ForEach(x =>
		{
			x.OnValueChanged.RemoveAllListeners();
		});
		elements = list;
		elements.ForEach(x =>
		{
			x.IsOn = false;
			x.OnValueChanged.AddListener(OnToggleValueChanged);
		});
		elements[0].IsOn = true;
	}
	public UnityEvent OnToggleChanged;
	private void Start()
	{
		if (elements != null & elements.Count > 0)
			elements.ForEach(x =>
			{
				x.OnValueChanged.AddListener(OnToggleValueChanged);
			});
		elements[0].IsOn = true;
	}
	private void OnToggleValueChanged(ToggleImageSwipe toggle)
	{
		elements.ForEach(x =>
		{
			if (x != toggle)
			{
				x.IsOn = false;
			}
			else
			{
				x.IsOn = true;
			}
		});

	}
}

