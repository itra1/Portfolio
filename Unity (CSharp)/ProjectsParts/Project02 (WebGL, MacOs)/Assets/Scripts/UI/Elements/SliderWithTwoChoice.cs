using UnityEngine;
using UnityEngine.UI;

public class SliderWithTwoChoice : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image back;
    [SerializeField] Sprite spiteOn;
    [SerializeField] Sprite spiteOf;


    public void ChangeChoice()
    {
        if (slider.value == 1)
        {
            back.sprite = spiteOn;
        }
        else
        {
            back.sprite = spiteOf;
        }

    }
}