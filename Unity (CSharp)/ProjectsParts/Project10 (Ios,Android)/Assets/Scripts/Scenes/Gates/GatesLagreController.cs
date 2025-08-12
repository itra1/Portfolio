using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Общий контроллер сцены с воротами
/// </summary>
public class GatesLagreController : MonoBehaviour {

    public static GatesLagreController instance;
    [SerializeField] TextMesh textTitle;
    [SerializeField] TextMesh countKeys;

    public GameObject[] gates;
    public int[] needsKeys;
    int openGate;
    int useKeys;
    [SerializeField] Text buttonTextBack;

    [SerializeField] GameObject keysPanel;

    bool isClosing;

    void Start() {
        instance = this;
        isSpeed = false;
        countKeys.GetComponent<MeshRenderer>().sortingOrder = 3;
        isClosing = false;
        //InitParametrs();
    }

    public void InitParametrs(bool showAnim = true, int? gateNum = null) {
        openGate = PlayerPrefs.GetInt("openGate",0);
        useKeys = UserManager.Instance.keys;

#if UNITY_EDITOR
        //openGate = 2;
#endif
        
        if (gateNum != null) {

            textTitle.text = LanguageManager.GetTranslate("gate_Gate") + " " + (gateNum+1).ToString();
            if(openGate == gateNum)
                countKeys.text = string.Format("{0}/{1}", useKeys.ToString(), needsKeys[(int)gateNum].ToString());
            else
                countKeys.text = string.Format("{0}/{1}", 0, needsKeys[(int)gateNum].ToString());

        } else {

            CheckGate(ref useKeys, ref openGate);

            textTitle.text = LanguageManager.GetTranslate("gate_Gate") + " " + ( openGate + 1 ).ToString();
            countKeys.text = string.Format("{0}/{1}", useKeys.ToString(), needsKeys[openGate].ToString());
        }

        /*
        if (gateNum == null) {
            for (int i = 0 ; i < gates.Length ; i++) gates[i].SetActive(( i == openGate ));
        } else {
            for (int i = 0 ; i < gates.Length ; i++) gates[i].SetActive(( i+1 == gateNum ));
        }
        */
        
        for (int i = 0 ; i < gates.Length ; i++) {
            if( i == (gateNum != null ? gateNum : openGate)) {
                
                gates[i].SetActive(true);
                gates[i].GetComponent<GateLagre>().ShowAnim(showAnim);
            } else
                gates[i].SetActive(false);
        }
        
    }
    
    void CheckGate(ref int useKeys, ref int openGate) {
        if (useKeys > needsKeys[(int)openGate] && openGate != needsKeys.Length) {
            useKeys -= needsKeys[(int)openGate];
            openGate++;
            CheckGate(ref useKeys, ref openGate);
        }
    }

    /// <summary>
    /// Реакция на кнопку закрытия
    /// </summary>
    public void ButtonClose() {
        
        if (isClosing) return;
        isClosing = true;

        GameManager.HideGateScene();
    }

    [SerializeField] AudioClip chainDownClip;
    [SerializeField] AudioClip openPadlockClip;

    public void PlayChainDown() {
        AudioManager.PlayEffect(chainDownClip, AudioMixerTypes.gateEffect);
    }
    public static void OpenPadlock() {
        AudioManager.PlayEffect(instance.openPadlockClip, AudioMixerTypes.gateEffect);
    }

    bool isSpeed;

    public delegate void TapSpeedy();
    public static event TapSpeedy OnTapSpeedy;

    /// <summary>
    /// Ускорение по тапу
    /// </summary>
    /// <returns></returns>
    public void SpeedyTap() {
        if (isSpeed) return;
        isSpeed = true;
        if (OnTapSpeedy != null)
            OnTapSpeedy();
    }

}
