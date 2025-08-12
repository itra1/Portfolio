using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

public class CardAnimController : MonoBehaviour
{
  [SerializeField] GameObject cardFrontPref;
  [SerializeField] GameObject cardBackPref;
  [SerializeField] GameObject fishkaPref;
  [SerializeField] float AnimTime = 0.5F;
  [SerializeField] Ease animType = Ease.InOutQuad;
  [SerializeField] Ease animFishkaType = Ease.InExpo;
  [SerializeField] float step = 20F;
  [SerializeField] float delayTime = 0.5F;
  [SerializeField] float delayFishkas = 0.2F;
  [SerializeField] bool repeat = false;
	[SerializeField] GameObject[] cardsBackObject = new GameObject[5];
	[SerializeField] GameObject[] cardsFrontObject = new GameObject[5];

	public bool animate = false;

	private GameObject[] fishkaObj = new GameObject[5];
  private float width = 100F;
	private List<GameObject> objects;
	public void StartAnimation(){
      repeat = true;
      width = GetComponent<RectTransform>().rect.width / 2;
      objects = new List<GameObject>();
      if (animate==false){
      StartCoroutine(CreateAnimInstance(AnimateCard, 0, 4));
      animate = true;
    }
  }
  delegate void AnimDelegat(int index);
  private delegate void MyFunc();
  private IEnumerator CreateAnimInstance(AnimDelegat delegat,int startValue,int maxValue)
  {
    delegat.Invoke(startValue);
    yield return new WaitForSeconds(delayTime);
    if (startValue < maxValue)
    {
      StartCoroutine(CreateAnimInstance(delegat,startValue+1,maxValue));
    }

  }
  private IEnumerator DelayCoroutine(IEnumerator routine, float delay){
    yield return new WaitForSeconds(delay);
    StartCoroutine(routine);
  }
  
  private IEnumerator DelayCoroutine(MyFunc func, float delay)
  {
    yield return new WaitForSeconds(delay);
    func();
  }
  private void AnimateCard(int index)
  {
    if (gameObject.activeInHierarchy)
    {
    
      var cardB = GetObjectFromArrayOrCreate(cardBackPref,cardsBackObject,index);
      cardB.SetActive(true);
      if (!objects.Exists((x)=> { return cardB == x; }))
      objects.Add(cardB);
      var rectB = cardB.GetComponent<RectTransform>();
      rectB.anchoredPosition = new Vector2(0, 0);
      rectB.localScale = Vector2.one;
      rectB.DOAnchorPos(new Vector2(width - step * (index), 0), AnimTime).SetEase(animType);
      
       rectB.DOScale(new Vector3(0.1F, 1), AnimTime).SetEase(animType).OnComplete(() =>
      {
        StartFrontCardAnim(rectB.anchoredPosition, index);
        cardB.SetActive(false);
      }); ;
    }
  }
  private GameObject GetObjectFromArrayOrCreate(GameObject prefab,GameObject[] array,int index){
  if (array[index]==null)
		{
			array[index] = Instantiate(prefab,this.transform);
  }
    return array[index];
  }
  private void StartFishkaAnimation(int index){
    if (gameObject.activeInHierarchy)
    {
      var fishka = GetObjectFromArrayOrCreate(fishkaPref, fishkaObj, index);
      fishka.SetActive(true);
      if (objects.Exists((x)=> { return x == fishka; }))
      objects.Add(fishka);
      var rect = fishka.GetComponent<RectTransform>();
      rect.anchoredPosition = new Vector2(0, 0);
      rect.localScale = Vector2.one;
      rect.DOScale(new Vector2(1, 1), AnimTime / 4).OnComplete(() =>
      {
        rect.DOScale(new Vector2(0.1F, 1), AnimTime *3/ 4).OnComplete(() =>
        {
          rect.DOScale(new Vector2(1F, 1F), AnimTime / 2);
        });
      });

      rect.DOAnchorPos(new Vector2(-width+index*step/4, 0), AnimTime).SetEase(animType).OnComplete(() =>
      {
        rect.DOAnchorPos(new Vector2(-width * 2, 0), AnimTime).SetEase(animFishkaType).OnComplete(() =>
        {
          //Destroy(fishka);
          fishka.SetActive(false);
          if (index == 4)
          {
            if (repeat)
            {
              
              if (gameObject.activeInHierarchy){
                StartCoroutine(DelayCoroutine(StartAnimation, delayFishkas));
                //StartAnimation();
                animate = false;
              }
                
            }

          }
        });
      });
    }
  }
  private void StartFrontCardAnim(Vector2 position,int index){
    if (gameObject.activeInHierarchy)
    {
      var card = GetObjectFromArrayOrCreate(cardFrontPref,cardsFrontObject,index);
      card.SetActive(true);
      var rect = card.GetComponent<RectTransform>();
      if (objects.Exists((x)=>{ return x == card; }))
      objects.Add(card);
      rect.anchoredPosition = position;
      rect.localScale = new Vector2(0.1F, 1);
      rect.DOScale(new Vector2(1, 1), AnimTime / 3).SetEase(animType).OnComplete(() =>
      {
        rect.DOAnchorPos(new Vector2(width * 2, 0), AnimTime).SetEase(animType).OnComplete(() =>
        {
          card.SetActive(false);
          if (index == 4)
          {
            if (gameObject.activeInHierarchy)
            {
              animate = false;
              StartCoroutine(DelayCoroutine(CreateAnimInstance(StartFishkaAnimation, 0, 4), delayFishkas));
            }

          }
        });
      });
    }
    
  }
  public void OnDisable()
  {
    StopAllCoroutines();
  }
  public void StopAnimation(){
    repeat = false;  
    animate = false;
    StopAllCoroutines();
    foreach(var obj in objects){
    if (obj!=null){
        obj.transform.DOKill();
        //Destroy(obj);
    }
    }
   }
}
