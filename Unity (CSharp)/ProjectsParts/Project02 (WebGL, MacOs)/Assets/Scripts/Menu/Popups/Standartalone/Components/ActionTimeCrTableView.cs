using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTimeCrTableView : MonoBehaviour, IPokerTableSettings
{
  [SerializeField]
  private ActionTimeItemView itemPref;
  [SerializeField]
  private List<ActionTimeItemView> items=new List<ActionTimeItemView>();
  [SerializeField]
  RectTransform content;
  [SerializeField]
  ImageSwiperGroup swiperGroup;

  public int currentValue;
  public void SetSettings(PokerTableSettings settings)
  {
    var lists = settings.timesSteps;
    items.ForEach(x => GameObject.Destroy(x.gameObject));
    items.Clear();
    var listSwipes = new List<ToggleImageSwipe>();
    foreach (var element in lists)
		{
			var item = Instantiate(itemPref, content);

			item.Value = element;
      items.Add(item);
      var imgSwipe = item.GetComponent<ToggleImageSwipe>();
      listSwipes.Add(imgSwipe);
      imgSwipe.OnValueChanged.AddListener(OnToggleClick);
			item.gameObject.SetActive(true);
		}

		currentValue = items[0].Value;
    swiperGroup.SetGroup(listSwipes);
  }
  public void SetSettings(int[] timeSteps)
  {
    var lists = timeSteps;
    items.ForEach(x => GameObject.Destroy(x.gameObject));
    items.Clear();
    var listSwipes = new List<ToggleImageSwipe>();
    foreach (var element in lists)
		{
			var item = Instantiate(itemPref, content);
      item.Value = element;
      items.Add(item);
      var imgSwipe = item.GetComponent<ToggleImageSwipe>();
      listSwipes.Add(imgSwipe);
      imgSwipe.OnValueChanged.AddListener(OnToggleClick);
			item.gameObject.SetActive(true);
		}

    currentValue = items[0].Value;
    swiperGroup.SetGroup(listSwipes);
  }
  private void OnToggleClick(ToggleImageSwipe swiper)
  {
     if (swiper.IsOn){
      currentValue=swiper.GetComponent<ActionTimeItemView>().Value;
     }
  }
}
