using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyTXMValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI copyFrom;
    [SerializeField] private TextMeshProUGUI copyTo;

    public void CopyValue ()
    {
        copyTo.text = copyFrom.text;
    }

}
