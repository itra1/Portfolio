using UnityEngine;
using System.Collections;

public class UpStoneController : SpawnAbstract {

  public HingeJoint2D mace;

  bool maceConnect;

  void OnEnable() {
    maceConnect = true;
  }

  void Update() {
    // Удалять при 0 позиции
    if(CameraController.displayDiff.transform.position.x <= CameraController.displayDiff.left * 3) Destroy(gameObject);

    if(maceConnect && mace.connectedBody != null) {
      maceConnect = false;
      Questions.QuestionManager.ConfirmQuestion(Quest.knockHeadStone, 1, transform.position);
    }
  }
}
