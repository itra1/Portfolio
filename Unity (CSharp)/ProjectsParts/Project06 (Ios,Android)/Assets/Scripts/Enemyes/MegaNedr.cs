using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

/// <summary>
/// Гик презыватель
/// </summary>
public class MegaNedr : Enemy {

  float targetPositionX;
  float leftBorder;               // Левая граница перемещения
  float rightBorder;              // Правая граница перемещения

	private bool getDamage;
	bool isTeleport;
  bool isFirst;

  float nextTimeGenerate;
  float moveVectorChangeTime;

  int generateCount = 2;

  protected override void OnEnable() {
		isTeleport = false;
		getDamage = false;
    base.OnEnable();
		
    isFirst = true;
    GetComponent<PolygonCollider2D>().enabled = false;

    leftBorder = CameraController.leftPointX.x + CameraController.distanseX / 2;
    rightBorder = CameraController.leftPointX.x + CameraController.distanseX/6*5;
    SetDirectionVelocity(-1);
    targetPositionX = GetNextPosition();
    //StartCreateCheburator();
  }

  public override void Update() {
    
    if(isTeleport) {
      //if(isHide)
      //  HideEnemy();
      //else
      //  Show();
			return;
    }
		
		base.Update();
		if (phase == Phase.run) {
      if(nextTimeGenerate <= Time.time && directionVelocity > 0 && transform.position.x > targetPositionX) {
        CheckPhase();
      }
      if(nextTimeGenerate <= Time.time && directionVelocity < 0 && transform.position.x < targetPositionX) {
        CheckPhase();
      }

      if(!isFirst) ChengeMoveVector();
    }

  }

	IEnumerator TeleportProcess(System.Action OnComplete) {
		while (graphic.transform.localScale.x > 0) {
			graphic.transform.localScale -= Vector3.one * Time.deltaTime;
			skeletonAnimation.skeleton.a -= 1 * Time.deltaTime;
			yield return null;
		}
		graphic.transform.localScale -= Vector3.zero;
		skeletonAnimation.skeleton.a -= 0;

		transform.position = new Vector3(GetNextPosition(), transform.position.y, transform.position.z);

		while (graphic.transform.localScale.x < 1) {
			graphic.transform.localScale += Vector3.one * Time.deltaTime;
			skeletonAnimation.skeleton.a += 1 * Time.deltaTime;
			yield return null;
		}
		skeletonAnimation.skeleton.a = 1;
		graphic.transform.localScale = Vector3.one;

		OnComplete();
	}
	/*
	IEnumerator ShowProcess(Actione OnComplete) {
		while (graphic.transform.localScale.x <1){
			graphic.transform.localScale += Vector3.one * Time.deltaTime;
			skeletonAnimation.skeleton.a += 1 * Time.deltaTime;
			yield return null;
		}
		skeletonAnimation.skeleton.a += 1;
		graphic.transform.localScale = Vector3.one;
		OnComplete();
	}
	*/

  void StartLoop() {
    isFirst = false;
    GetComponent<PolygonCollider2D>().enabled = true;
    SetDirectionVelocity(-1);
    generateCount = 2;
  }
	/*
  void HideEnemy() {
    graphic.transform.localScale -= Vector3.one *Time.deltaTime;
		skeletonAnimation.skeleton.a -= 1 * Time.deltaTime;
		PlayShowAudio();
		if(graphic.transform.localScale.x <= 0) {
      graphic.transform.localScale = Vector3.zero;
      //isHide = false;
      transform.position = new Vector3(GetNextPosition(), transform.position.y, transform.position.z);
    }
  }

  void Show() {
    graphic.transform.localScale += Vector3.one * Time.deltaTime;
    skeletonAnimation.skeleton.a += 1 * Time.deltaTime;
		if(graphic.transform.localScale.x >= 1 && skeletonAnimation.skeleton.a >= 1) {
			skeletonAnimation.skeleton.a += 1;
			graphic.transform.localScale = Vector3.one;
      isTeleport = false;
      skeletonAnimation.timeScale = 1;
      GetComponent<PolygonCollider2D>().enabled = true;
		
			CheckPhase();
    }
  }
	*/
	
  void StartTeleport() {
    SetPhase(Phase.wait);
    GetComponent<PolygonCollider2D>().enabled = false;
    skeletonAnimation.timeScale = 0;
    isTeleport = true;

	  StartCoroutine(TeleportProcess(() => {
			skeletonAnimation.timeScale = 1;
			GetComponent<PolygonCollider2D>().enabled = true;
			CheckPhase();
			isTeleport = false;

		}));

	  //CheckPhase();
  }

	protected override void SetDamagePhase() {
		
	}
	protected override void SetRunPhase() {
		if (getDamage) {
			getDamage = false;
			StartTeleport();
			return;
		}
		if (!isTeleport && !getDamage) base.SetRunPhase();
	}
	protected override void CheckPhase() {
		if (!isTeleport)     base.CheckPhase();

    if(phase == Phase.run && !isTeleport) {
      if(nextTimeGenerate <= Time.time && directionVelocity > 0 && transform.position.x > targetPositionX) {
        StartLoop();
        SetPhase(Phase.wait);
        StartCreateCheburator();
      }
      if(nextTimeGenerate <= Time.time && directionVelocity < 0 && transform.position.x < targetPositionX) {
        StartLoop();
        SetPhase(Phase.wait);
        StartCreateCheburator();
      }
    }

    if(phase == Phase.wait && !isTeleport) {
      if(generateCount > 0 && !getDamage)
        StartCreateCheburator();

      if(!getDamage && generateCount == 0 && nextTimeGenerate > 0) {
        SetRunPhase();
        GetComponent<PolygonCollider2D>().enabled = false;
      }

      if(getDamage) {
        getDamage = false;
        StartTeleport();
      }

    }

  }

  void ChengeMoveVector() {
    if(phase != Phase.run) return;

    if(directionVelocity > 0 && transform.position.x > rightBorder) {
      SetDirectionVelocity(-1);
      moveVectorChangeTime = Time.time;
    } else if(directionVelocity > 0 && transform.position.x < rightBorder) {
      SetDirectionVelocity(+1);
      moveVectorChangeTime = Time.time;
    } else if(moveVectorChangeTime + 0.5f <= Time.time) {
      SetDirectionVelocity(Random.value <= 0.5f ? -1 : 1);
      moveVectorChangeTime = Time.time;
    }

  }

  float GetNextPosition() {

		float nextPositionX = transform.position.x;

		do {
			nextPositionX = Random.Range(leftBorder, rightBorder);
		} while(Mathf.Abs(nextPositionX - transform.position.x) < 2);

    return nextPositionX;
  }

  void StartCreateCheburator() {
    SetAnimation(appearance, false, false);
  }

  void CreateCheburator() {
    GameObject cheburatorInst = EnemysSpawn.Instance.GetEnemy("Cheburator");
    cheburatorInst.transform.position = new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z);
    cheburatorInst.SetActive(true);
    generateCount--;
		PlayCreateAudio();
	}

	public override void SetRunAnimation() {
		if (runAnim != null) SetAnimation(runAnim, true, false);
	}
	public override void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
    base.AnimEvent(trackEntry, e);
    if(e.Data.Name == "hit") CreateCheburator();
  }

  protected override void GetDamage(float newSpeedDelay = 0, float newTimeSpeedDelay = 0) {
    //ChangeHealthPanel();
		if (!isTeleport) getDamage = true;
  }

  public override void AnimComplited(TrackEntry trackEntry) {
    base.AnimEnd(trackEntry);
    if(trackEntry.ToString() == appearance) {

      if(generateCount == 0) {
        nextTimeGenerate = Time.time + 10;
        GetComponent<PolygonCollider2D>().enabled = false;
      }

      CheckPhase();
    }
		if (trackEntry.ToString() == deadAnim) {
			if (healthPanel != null)
				Destroy(healthPanel);
			DeactiveEnemy();

		}
	}

  [SpineAnimation(dataField: "skeletonAnimation")]
  public string appearance = "";     // создания


	#region Звуки

	public List<AudioClipData> createAudio;
	public AudioBlock spawnAudioBlock;

	protected virtual void PlayCreateAudio() {
		spawnAudioBlock.PlayRandom(this);
	}


	public List<AudioClipData> showAudio;
	public AudioBlock showAudioBlock;

	protected virtual void PlayShowAudio() {
		showAudioBlock.PlayRandom(this);
	}


	#endregion

}
