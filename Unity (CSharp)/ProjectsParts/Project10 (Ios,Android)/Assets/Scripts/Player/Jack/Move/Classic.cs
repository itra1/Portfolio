using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {
	
	/// <summary>
	/// Стандартный бег
	/// </summary>
	public class Classic : PlayerMove {
		protected override float horizontalSpeed {
			get { return 5;	}
		}

		public override float jumpSpeed {
			get { return 17;}
		}

    public override void Init() {
			
		}

		protected override AudioClip boostClip {
			get { return null; }
		}

    protected override void RunChangePhase(RunnerPhase oldValue, RunnerPhase newValue) {
      base.RunChangePhase(oldValue, newValue);

      if (newValue != oldValue) {
        if (newValue == RunnerPhase.dead) {
          isDead = false;
          if (controller.isGround) {
            velocity.y = 12;
          }
        }
      }

    }

    protected override void Update() {

      if (pet.IsPet) return;

      // Поведение во время бустов, отдаем управление другому модулю
      if ((RunnerController.Instance.runnerPhase & (RunnerPhase.boost | RunnerPhase.preBoost | RunnerPhase.postBoost)) != 0)
        return;

      if (RunnerController.Instance.runnerPhase != RunnerPhase.start) {
        rigidbody.gravityScale = controller.isGround ? 0 : controller.gravityNorm;
      }

      switch (RunnerController.Instance.runnerPhase) {
        case RunnerPhase.run:
        case RunnerPhase.tutorial:
        case RunnerPhase.boss:
        case RunnerPhase.endRun:
        case RunnerPhase.lowEnergy:
          PlayMove();
          GameAnimation();    // Анимация  
          break;
        case RunnerPhase.dead:
          DeadAnimation();    // Анимация 
          DeadMovement();     // Движение 
          break;
        case RunnerPhase.preBoost:
          PreBoostMovement();    // Анимация  
          break;
      }


      //if ((RunnerController.Instance.runnerPhase & (RunnerPhase.run | RunnerPhase.tutorial | RunnerPhase.boss | RunnerPhase.endRun | RunnerPhase.lowEnergy)) != 0) {
      //  PlayMove();
      //  GameAnimation();    // Анимация  
      //}

      //// Поведение если убили
      //if (RunnerController.Instance.runnerPhase == RunnerPhase.dead) {
      //  DeadAnimation();    // Анимация  
      //}

      //if (RunnerController.Instance.runnerPhase == RunnerPhase.preBoost) {
      //  PreBoostMovement();     // Движение
      //}

      //if (RunnerController.Instance.runnerPhase == RunnerPhase.dead) {
      //  DeadMovement();     // Движение
      //}

      //if(playerState == RunnerPhase.preBoost) {
      //  PreBoostMovement();     // Движение
      //}

      if (jumpKey) {
        Questions.QuestionManager.ConfirmQuestion(Quest.jumped, 1, transform.position);
        jumpKey = false;
      }

      if (doubleJumpReady) {
        RunnerController.doubleJumpCount++;
        check.ChechDoubleJump();
        doubleJumpReady = false;
      }
      if (controller.isGround && isJumped && rigidbody.velocity.y <= 0) {

        OnGroundEmit();

        isJumped = false;
        check.endJump = true;
        GameObject jumpFinish = Pooler.GetPooledObject("PlayerJumpFinish");
        jumpFinish.transform.position = new Vector3(transform.position.x + 0.08f, transform.position.y - 0.4f, -0.1f);
        jumpFinish.SetActive(true);
      }
    }

    void PlayMove() {

      if (isStopped) return;
      
      velocity = rigidbody.velocity;
      velocity.x = RunSpeedToPlayer;
      //horizontalKey = 0;

      if (horizontalKeyValue < 0 && transform.position.x < CameraController.displayDiff.leftDif(0.8f))
        velocity.x = RunSpeedToPlayer;
      else if (horizontalKeyValue > 0 && transform.position.x > CameraController.displayDiff.rightDif(0.8f))
        velocity.x = RunSpeedToPlayer;
      else {
        //Debug.Log(horizontalSpeed * Mathf.Sign(vectorX) * (isDead ? 0.2f : 1));
        //velocity.x += horizontalSpeed * Mathf.Sign(vectorX) * (isDead ? 0.2f : 1);

        if(horizontalKeyValue != 0)
          velocity.x += horizontalKeyValue * horizontalSpeed * (isDead ? 0.2f : 1);
        else
          velocity.x *= (isDead ? 0.2f : 1);

      }

      if (isSticky)
        velocity.x = RunSpeedToPlayer;
      
      if (!isDoubleJumped && controller.isGround && rigidbody.velocity.y <= 0) isDoubleJumped = true;

      if (jumpKey) {
        jumpKey = false;
        if (controller.isGround) {
          animation.SetAnimation(animation.jumpIdleAnim, true);
          audio.PlayEffect(audio.jumpAudio, AudioMixerTypes.runnerEffect);
          isJumped = true;
        } else if ((isDoubleJumped && !doubleJumpReady) || GameManager.activeLevelData.gameFormat == GameMechanic.jetPack) {
          isDoubleJumped = false;
          doubleJumpReady = true;
          audio.PlayEffect(audio.jumpAudioAir, AudioMixerTypes.runnerEffect);
        } else
          return;

        isJumped = true;

        GameObject jumpStart = Pooler.GetPooledObject("PlayerJumpStart");
        jumpStart.transform.position = new Vector2(controller.graundPoint.position.x, controller.graundPoint.position.y);
        jumpStart.SetActive(true);

        rigidbody.velocity = new Vector2(velocity.x, 0);

        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
      } else {
        rigidbody.velocity = velocity;
      }
    }

    /// <summary>
    /// Анимация игрока во время игрового процесса
    /// </summary>
    void GameAnimation() {

      if (controller.isGround && rigidbody.velocity.y <= 0) {
        animation.timeScale = 1f;
        animation.SetAnimation(animation.runIdleAnim, true);
      } else if (isDoubleJumped || isJumped) {
        if (isDoubleJumped) {
          animation.timeScale = 2.5f;
          animation.SetAnimation(animation.jumpDoubleIdleAnim, false, false);
        } else
          animation.SetAnimation(animation.jumpIdleAnim, true);
      }

    }

    /// <summary>
    /// Анимация игрока во время смерти
    /// </summary>
    void DeadAnimation() {
      if (!isDead) {
        if (controller.isGround)
          animation.SetAnimation(animation.deadAnim, false);
        else
          animation.SetAnimation(animation.deadJumpAnim, false);
        isDead = true;
      }
    }

    void DeadMovement() {
      velocity.x = RunnerController.RunSpeed;
      rigidbody.velocity = new Vector2(velocity.x, rigidbody.velocity.y);
    }

    void PreBoostMovement() {
      animation.SetAnimation(animation.runIdleAnim, true);
      velocity.x = RunSpeedToPlayer;
      //controller.Move(velocity * Time.deltaTime);
    }

  }

}