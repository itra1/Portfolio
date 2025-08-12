using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class FadeColor : MonoBehaviour
{
  private struct RenderersData
  {
    public TMPro.TextMeshProUGUI text;
    public Image image;
    public Color targetColor;
    public Color transparentColor;
  }
  private List<RenderersData> imagesMenuList = new List<RenderersData>();

  private RectTransform _source;

  public void Ready(RectTransform source)
  {
    if (_source == source)
      return;

    _source = source;
    FindImagesMenu();
  }

  private void FindImagesMenu()
  {
    imagesMenuList.Clear();

    Image[] menuImages = _source.GetComponentsInChildren<Image>(true);

    for (int i = 0; i < menuImages.Length; i++)
    {
      Color c = menuImages[i].color;
      c.a = 0;
      imagesMenuList.Add(new RenderersData()
      {
        image = menuImages[i],
        targetColor = menuImages[i].color,
        transparentColor = c
      });
    }


    TextMeshProUGUI[] menuTexts = _source.GetComponentsInChildren<TextMeshProUGUI>();

    for (int i = 0; i < menuTexts.Length; i++)
    {
      Color c = menuTexts[i].color;
      c.a = 0;
      imagesMenuList.Add(new RenderersData()
      {
        text = menuTexts[i],
        targetColor = menuTexts[i].color,
        transparentColor = c
      });
    }

  }

  private void OnDisable()
  {
    foreach (RenderersData mcd in imagesMenuList)
    {
      if (mcd.image != null)
      {
        mcd.image.color = mcd.targetColor;
      }
      if (mcd.text != null)
      {
        mcd.text.color = mcd.targetColor;
      }
    }
  }

  public void Show(float time = 2, System.Action onComplete = null)
  {
    _source.gameObject.SetActive(true);
    foreach (RenderersData mcd in imagesMenuList)
    {
      if (mcd.image != null && mcd.image.gameObject.activeInHierarchy)
      {
        mcd.image.color = mcd.transparentColor;
        mcd.image.DOColor(mcd.targetColor, time);
      }
      if (mcd.text != null && mcd.text.gameObject.activeInHierarchy)
      {
        mcd.text.color = mcd.transparentColor;
        mcd.text.DOColor(mcd.targetColor, time);
      }
    }

    DOVirtual.DelayedCall(time, () =>
    {
      onComplete?.Invoke();
    });

  }

  public void Hide(float time = 2, System.Action onComplete = null)
  {
    _source.gameObject.SetActive(true);
    foreach (RenderersData mcd in imagesMenuList)
    {
      if (mcd.image != null && mcd.image.gameObject.activeInHierarchy)
        mcd.image.DOColor(mcd.transparentColor, time);
      if (mcd.text != null && mcd.text.gameObject.activeInHierarchy)
        mcd.text.DOColor(mcd.transparentColor, time);
    }

    DOVirtual.DelayedCall(time, () =>
    {
      onComplete?.Invoke();
    });
  }

}
