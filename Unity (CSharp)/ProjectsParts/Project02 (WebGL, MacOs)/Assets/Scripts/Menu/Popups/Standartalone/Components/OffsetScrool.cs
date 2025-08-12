using System.Collections;
using UnityEngine;

public class OffsetScrool : MonoBehaviour
{
    [SerializeField] RectTransform rect;

    [SerializeField] CheckClick check;

    private void OnEnable()
    {
        if(check !=null)
        check.Hide += SetDefault;
    }

    public void MakeOffset(float hight)
    {
        StartCoroutine(Move(rect, new Vector2(0, hight)));
    }

    public void SetDefault()
    {
        StartCoroutine(Move(rect, new Vector2(0, 0)));
    }

    IEnumerator Move(RectTransform rt, Vector2 targetPos)
    {
        float step = 0;
        while (step < 1)
        {
            rt.offsetMax = Vector2.Lerp(rt.offsetMax, targetPos, step += Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}







