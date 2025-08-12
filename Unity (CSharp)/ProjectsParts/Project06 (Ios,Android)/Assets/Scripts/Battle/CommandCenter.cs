using UnityEngine;
using System.Collections;

public class CommandCenter : MonoBehaviour {

  float timeSendMessage;

  [System.Serializable]
  public struct TypeLetter
  {
    public string text;
    public float timeSend;
    public bool isRepetition;
    public float timeRepeat;
    public bool isSent;
    [HideInInspector]
    public float lastShowTime;
  }

  public TypeLetter[] letterMas;
  // Use this for initialization
  void Start () {
    SetTimeSendNextMessage();
  }
	
	// Update is called once per frame
	void Update () {
    if (timeSendMessage < Time.time)    {
      if (!CopterController.instance.gameObject.activeInHierarchy) SendMessage();
      else timeSendMessage += 0.2f;
    }
	}

  int countLetterMas;

  void SetTimeSendNextMessage() {
    timeSendMessage = 88888888f;

    for (int i = 0; i < letterMas.Length; i++)    {
      if (timeSendMessage > letterMas[i].timeSend)
      {
        timeSendMessage = letterMas[i].timeSend;
        countLetterMas = i;
      }
    }
    if (letterMas[countLetterMas].isRepetition) letterMas[countLetterMas].timeSend += letterMas[countLetterMas].timeRepeat;
    else letterMas[countLetterMas].timeSend = 88888888f;
    }

    void SendMessage() {
    GameObject delivery = CopterController.instance.SetTypeDeliverly(DeliveryType.letter);
    if (delivery != null && delivery.GetComponent<MessageController>()) delivery.GetComponent<MessageController>().WriteMessage(letterMas[countLetterMas].text);
    CopterController.instance.gameObject.SetActive(true);
    SetTimeSendNextMessage();
  }

  void GenerateMessage(Vector3 positionGenerate)  {
  }
}
