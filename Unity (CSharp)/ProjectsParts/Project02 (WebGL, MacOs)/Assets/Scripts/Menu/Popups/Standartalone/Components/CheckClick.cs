using it.UI.Elements;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckClick: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private InputDropDown _regionDropDown;
    [SerializeField] GameObject info;

    public event Action Hide;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(_regionDropDown != null)
       _regionDropDown.HideDropDown();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (info != null)
        {
            info.gameObject.SetActive(false);
            Hide?.Invoke();
        }
           
    }
}





