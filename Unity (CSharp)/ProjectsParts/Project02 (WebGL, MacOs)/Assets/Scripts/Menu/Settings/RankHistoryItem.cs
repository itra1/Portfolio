using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankHistoryItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI RankTxt;
    [SerializeField] private TextMeshProUGUI StartRateTxt;
    [SerializeField] private TextMeshProUGUI EarnedTxt;
    [SerializeField] private TextMeshProUGUI RequiedTxt;
    [SerializeField] private TextMeshProUGUI CompletedTxt;
    [SerializeField] private TextMeshProUGUI DataTxt;
    [SerializeField] private TextMeshProUGUI AmountTxt;
    [SerializeField] private TextMeshProUGUI SPsTxt;

    [Space]
    [SerializeField] private Slider slider;

    [Space]
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] names;

    public void Show (RankRecord rankRecord)
    {
        if(RankTxt) RankTxt.text = rankRecord.rank.name;
        if (StartRateTxt) StartRateTxt.text = rankRecord.created_at;
        if (EarnedTxt) EarnedTxt.text = rankRecord.rank.ClimbSp.ToString(); //TO DO
        if (RequiedTxt) RequiedTxt.text = rankRecord.rank.MaintainSp.ToString(); //TO DO
        if (CompletedTxt) CompletedTxt.text = rankRecord.finished_at == "" ? "None" : "Completed";
        if (DataTxt) DataTxt.text = rankRecord.finished_at == "" ? "None" : rankRecord.finished_at;
        if (AmountTxt) AmountTxt.text = rankRecord.rank.timebank_cost_usd.ToString(); //TO DO
        if (SPsTxt) SPsTxt.text = rankRecord.rank.MaintainSp + "/" + rankRecord.rank.ClimbSp; //TO DO
        if (slider) slider.value = (rankRecord.rank.MaintainSp / (float)rankRecord.rank.ClimbSp);

        if (image)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == rankRecord.rank.name)
                {
                    image.sprite = sprites[i];
                    break;
                }
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
