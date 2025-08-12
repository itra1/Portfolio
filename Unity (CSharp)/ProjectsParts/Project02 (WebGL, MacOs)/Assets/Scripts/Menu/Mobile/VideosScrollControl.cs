using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideosScrollControl : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    

    private RectTransform[] content;

    int contentToShow = 0;

    void Start ()
    {
        content = new RectTransform[contentPanel.childCount];

        for (int a = 0; a < content.Length; a++)
        {
            content[a] = (RectTransform) contentPanel.GetChild(a);
        }

        SnapTo(contentToShow);
    }

    public void SnapTo(int cts)
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.horizontalNormalizedPosition = (float)cts / ((float)contentPanel.childCount-1) ;
        it.Logger.Log(scrollRect.horizontalNormalizedPosition);
    }

    public void NextPosition()
    {
        contentToShow++;
        contentToShow = Mathf.Clamp(contentToShow, 0, content.Length-1);
        SnapTo(contentToShow);
    }

    public void PrevPosition()
    {
        contentToShow--;
        contentToShow = Mathf.Clamp(contentToShow, 0, content.Length-1);
        SnapTo(contentToShow);
    }
}
