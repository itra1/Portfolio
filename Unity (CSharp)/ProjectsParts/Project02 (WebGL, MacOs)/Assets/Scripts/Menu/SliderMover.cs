using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderMover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bidCount;
    [SerializeField] public Slider slider;

    public Action UpdateSlider = null;
    private float currentValue;
    private float desiredValue = 1f;
    private float speedModifier;
    private float stepSlider = 1f;
    private bool isWhole = false;

    public void Init(float step, float min, float max, bool isWhole)
    {
        this.isWhole = isWhole;
        stepSlider = step;
        slider.minValue = min;
        slider.maxValue = max;

        SetEnabledSlider(min != max);
        SetValue(min);
    }

    public void AddDesiredValue()
    {
        ChangeDesiredValue(stepSlider);
    }
    
    public void InsertDesiredValue()
    {
        ChangeDesiredValue(-stepSlider);
    }

    private void ChangeDesiredValue(float value)
    {
        desiredValue = currentValue;
        desiredValue += value;
        if (desiredValue > slider.maxValue)
        {
            desiredValue = slider.maxValue;
        }
        else if (desiredValue < slider.minValue)
        {
            desiredValue = slider.minValue;
        }
        
        SetValue(desiredValue);
    }
    
    public void SetValue(float value)
    {
        currentValue =  isWhole  && slider.value != slider.maxValue ? (int)value : value;
        slider.value = currentValue;
    }

    public void OnValueChangedBid()
    {
        var count = (isWhole && slider.value != slider.maxValue ? (int) slider.value : slider.value);
        if (count < 0) count = 0;
        bidCount.text = "<color=#E9B069>$</color>" + count.ToString("#.##");
        currentValue = slider.value;
        UpdateSlider?.Invoke();
    }

    public float GetValue()
    {
        if (isWhole && slider.value != slider.maxValue) return (int)slider.value;
        return slider.value;
    }
    
    public void SetEnabledSlider(bool isEnable)
    {
        slider.interactable = isEnable;
    }
}
