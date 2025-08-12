using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnScreenNumberKeyboard : MonoBehaviour
{
    public SliderMover sliderMover;

    private string input;

    [SerializeField] private Button ButtonOne;
    [SerializeField] private Button ButtonTwo;
    [SerializeField] private Button ButtonThree;
    [SerializeField] private Button ButtonFour;
    [SerializeField] private Button ButtonFive;
    [SerializeField] private Button ButtonSix;
    [SerializeField] private Button ButtonSeven;
    [SerializeField] private Button ButtonEight;
    [SerializeField] private Button ButtonNine;
    [SerializeField] private Button ButtonZero;
    [SerializeField] private Button clearButton;

    void Start()
    {     
    }

    public void PressOne() 
    {
        input = input + "1";
        SetInput();
    }

    public void PressTwo() 
    {
        input = input + "2";
        SetInput();
    }

    public void PressThree() 
    {
        input = input + "3";
        SetInput();
    }

    public void PressFour() 
    {
        input = input + "4";
        SetInput();
    }

    public void PressFive() 
    {
        input = input + "5";
        SetInput();
    }
    public void PressSix() 
    {
        input = input + "6";
        SetInput();
    }
    public void PressSeven()  
    {
        input = input + "7";
        SetInput();
    }
    public void PressEight() 
    {
        input = input + "8";
        SetInput();
    }
    public void PressNine() 
    {
        input = input + "9";
        SetInput();
    }
    public void PressZero()
    {
        input = input + "0";
        SetInput();
    }
    public void PressComma()
    {
        input = input + ",";
    }
    public void PressClear() 
    {
        input = "";
        SetInput();
    }
    
    public void SetInput ()
    {
        if (input.Equals(""))
            sliderMover.SetValue(0);

        else
            sliderMover.SetValue(float.Parse(input));
    }

    public void ClearInput ()
    {
        input = "";
    }

}

