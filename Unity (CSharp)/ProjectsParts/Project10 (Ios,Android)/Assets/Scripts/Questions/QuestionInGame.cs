/*
  Контроллер плавающего в воздухе окна
*/

using UnityEngine;
using System.Collections;

public class QuestionInGame : MonoBehaviour {


	Questions.QuestionManager quest;

  public GameObject[] questionOneInGame;


  float waitTime;
  float speed;

  void Start() {
    quest = Questions.QuestionManager.Instance;
		Questions.LevelList listQuestion = quest.actionList;


    for(int i = 0; i < questionOneInGame.Length; i++) {

      GameObject iconNeed = questionOneInGame[i].transform.Find("IconNeed").gameObject;
      GameObject iconConf = questionOneInGame[i].transform.Find("IconConfirm").gameObject;

      string titleText = listQuestion.items[i].descr;
      questionOneInGame[i].transform.Find("Title").gameObject.GetComponent<TextMesh>().text = titleText;
      questionOneInGame[i].transform.Find("TextCount").GetComponent<TextMesh>().text = Mathf.Round(listQuestion.items[i].value) + "/" + listQuestion.items[i].needvalue;

      if(listQuestion.items[i].needvalue <= listQuestion.items[i].value) {
        iconConf.SetActive(true);
        iconNeed.SetActive(false);

        float koefX = titleText.Length * 1.5f;
        float lengthX = titleText.Length * 0.15f;

        questionOneInGame[i].transform.Find("line").transform.localScale = new Vector3(koefX, 0.3f, 1);
        questionOneInGame[i].transform.Find("line").transform.localPosition = new Vector3(-3.15f + (lengthX / 2), questionOneInGame[i].transform.Find("line").transform.localPosition.y, questionOneInGame[i].transform.Find("line").transform.localPosition.z);

      } else {
        iconConf.SetActive(false);
        iconNeed.SetActive(true);
      }

    }

  }

  void Update() {
    /*
    if (transform.position.x < CameraController.displayDiff.transform.position.x && waitTime == 0)
        waitTime = Time.time + 5f;

    if (waitTime > Time.time) {
        speed = RunnerController.RunSpeed;
    } else {
        speed -= RunnerController.RunSpeed * Time.deltaTime;
    }

    if (speed < 0) speed = 0;
    */

    speed = RunnerController.RunSpeed * 0.8f;
    transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;

    if(transform.position.x < CameraController.displayDiff.leftDif(2))
      Destroy(gameObject);

  }

}
