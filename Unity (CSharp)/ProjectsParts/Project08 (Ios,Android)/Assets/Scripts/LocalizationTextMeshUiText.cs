using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationTextMeshUiText : ExEvent.EventBehaviour
{

    public string code;
    private TextMeshProUGUI _textUi;
    private Vector2 rectTrans;
    public bool toUppcase;

    public bool isShadow = true;
    public bool scailing = true;

    private float _fontSize;

    protected override void Awake()
    {
        base.Awake();
        _textUi = GetComponent<TextMeshProUGUI>();
        _textUi.font = LanguageManager.Instance.activeLanuage.graphicFontShadow;
        rectTrans = _textUi.GetComponent<RectTransform>().sizeDelta;
        _fontSize = _textUi.fontSize;
    }

    private void OnEnable()
    {
        ChangeLanuage();
    }

    public void SetCode(string code)
    {
        this.code = code;
        ChangeLanuage();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.LanuageEvents.OnChangeLanguage))]
    public void OnChangeLanuage(ExEvent.LanuageEvents.OnChangeLanguage language)
    {
        ChangeLanuage();
    }

    private void ChangeLanuage()
    {

        string titleText = LanguageManager.GetTranslate(code);

        if (String.IsNullOrEmpty(titleText)) return;

        if (toUppcase)
            titleText = titleText.ToUpper();

        _textUi.font = isShadow ? LanguageManager.Instance.activeLanuage.graphicFontShadow : LanguageManager.Instance.activeLanuage.graphicFont;

        _textUi.text = titleText;

        _textUi.fontSize = _fontSize;

        if (scailing)
        {
            while (rectTrans.x < _textUi.preferredWidth)
                _textUi.fontSize--;
        }

    }
}
