using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Spine;

/// <summary>
/// Контроллер врага со средней скоростью
/// </summary>
public class Diversant : Enemy {

    /// <summary>
    /// Вероятность, что противник отпрыгнет после атаки
    /// </summary>
    private const float JUMP_AFTER_ATTACK_CHANCE = 0.5f;
    /// <summary>
    /// сила прыжка
    /// </summary>
    private const float JUMP_STRENGTH = 7.5f;
    /// <summary>
    /// дельта изменения скейла тени при прыжке
    /// </summary>
    private const float JUMP_SHADOW_CHANGE_STEP = 0.01f;

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string forwardFlipAnim; // Анимация переднего сальто

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string backwardFlipAnim; // Анимация заднего сальто

    private bool _isJump = false; //находится ли противник в воздухе
    private bool _moveUp = false; //является ли фаза прыжка - подъём
    private bool _willDie = false; //нужно ли установить фазу смерти после того как противник приземлится
    private float _shadowPosY; //позиция тени по оси Y. Запоминается, чтобы во время прыжка её устанавливать
    private Vector3 _shadowPos = new Vector3(); //утилитный вектор для изменения и установки позиции тени
    private Vector3 _shadowScale = new Vector3(); //утилитный вектор дял изменения и установки скейла тени

    protected override void OnEnable() {
        base.OnEnable();
        _willDie = false;
        _moveUp = false;
        _isJump = false;

    }
    /// <summary>
    /// Выполнение прыжка. Устанавливается фаза, анимация и обратное направление движения
    /// </summary>
    private void Jump() {
        _shadowPosY = shadow.transform.position.y;
        _shadowPos = shadow.transform.position;
        _shadowScale = shadow.transform.localScale;
        _moveUp = true;
        _isJump = true;
        velocity.y = 0;
        SetDirectionVelocity(1, false);
        SetPhase(Phase.run, true);
        SetAnimation(backwardFlipAnim, true);
    }
    /// <summary>
    /// Шаг перемещения.
    /// </summary>
    public override void Move() {

        if (phase == Phase.dead) return;
        if (_isJump && _moveUp) {
            MoveUp();
        } else if (_isJump && !_moveUp){
            MoveDown();
        } else {
            base.Move();
        }
    }
    /// <summary>
    /// Шаг прыжка в состоянии подъема. Увеличивается расстояние до земли, меняются параметры тени
    /// </summary>
    private void MoveUp() {
        velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);
        velocity.y = JUMP_STRENGTH;
        transform.position += velocity * Time.deltaTime;
        ShadowAdjustments(-1);
    }
    /// <summary>
    /// Шаг прыжка в состоянии приземления. Уменьшается расстояние до земли, меняются параметры тени
    /// </summary>
    private void MoveDown() {
        velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);
        velocity.y = -JUMP_STRENGTH;
        transform.position += velocity * Time.deltaTime;
        ShadowAdjustments(1);
    }
    /// <summary>
    /// Изменение параметров тени: установка позиции по Y в запомненное значение на момент старта прыжка.
    /// Изменение скейла на дельту в зависимости от состояния прыжка
    /// </summary>
    /// <param name="shadowScaleMod"></param>
    private void ShadowAdjustments(float shadowScaleMod) {
        _shadowPos.y = _shadowPosY;
        shadow.transform.position = _shadowPos;
        _shadowScale.x += JUMP_SHADOW_CHANGE_STEP * shadowScaleMod;
        _shadowScale.y += JUMP_SHADOW_CHANGE_STEP * shadowScaleMod;
        shadow.transform.localScale = _shadowScale;
    }
    /// <summary>
    /// Обработка событий анимации при ударе и прыжке
    /// </summary>
    /// <param name="trackEntry"></param>
    /// <param name="e"></param>
    public override void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
        base.AnimEvent(trackEntry, e);
        //Если анимация - атака и событие с именем "hit", то производится прыжок противника
        //если позволяет вероятность
        if (trackEntry.ToString() == attackAnim[0]) {
            if (e.Data.Name == "hit") {
                float jumpRand = Random.Range(0f, 1f);
                if (jumpRand <= JUMP_AFTER_ATTACK_CHANCE) {
                    Jump();
                }
            }
        } else if (trackEntry.ToString() == backwardFlipAnim) {
            //Если анимация заднее сальто и событие "move_down", то устанавливаем, что текущее
            //состояние прыжка - не подъем
            if (e.Data.Name == "move_down") {
                _moveUp = false;
            //Если анимация заднее сальто и событие "move_stop", то убираем состояние прыжка
            } else if (e.Data.Name == "move_stop") {
                _isJump = false;
                //Если во время прыжка противник был убит, то устанавливаем фазу dead
                if (_willDie) {
                    SetPhase(Phase.dead);
                    velocity = Vector3.zero;
                } else {
                    //Устанавливаем по окончании прыжка состояние в бег, меняем направление движения
                    //и анимацию
                    SetPhase(Phase.run);
                    SetAnimation(runAnim, true);
                    SetDirectionVelocity(-1, false);
                    velocity.y = 0;
                }
            }
        }
    }

    public override void Update() {
        if (phase == Phase.dead /*|| battlePhase == BattlePhase.end*/) return;

        if (phase == Phase.run) {
            Move();
        }
        if (_isJump) {
            return;
        }
        if (phase == Phase.attack) Attack();
            CheckState();
    }

    protected override void CheckPhase() {
        if (_isJump) {
            return;
        }
        base.CheckPhase();
    }

    public override void SetPhase(Phase phase, bool force = false) {
        if (phase == Phase.dead && _isJump) {
            _willDie = true;
            return;
        }
        base.SetPhase(phase, force);
    }

    protected override void GetDamage(float stunDelay = 0, float stunTime = 0) {
        if (_isJump) {
            return;
        }
        base.GetDamage(stunDelay, stunTime);
    }

    public override void SetStun(float stunDelay = 0, float stunTime = 0) {
        if (_isJump) {
            return;
        }
        base.SetStun(stunDelay, stunTime);
    }

    protected override void SetStunPhase() {
        if (_isJump) {
            return;
        }
        base.SetStunPhase();
    }

    protected override void SetStunAnim() {
        if (_isJump) {
            return;
        }
        base.SetStunAnim();
    }

    protected override void SetDamageAnim() {
        if (_isJump) {
            return;
        }
        base.SetDamageAnim();
    }

    protected override void SetDeadPhase() {
        if (_isJump) {
            _willDie = true;
        } else {
            base.SetDeadPhase();
        }
    }

    protected override void EnemyContact(Enemy enemy) {
        if (_isJump) {
            return;
        }
        base.EnemyContact(enemy);
    }

    

}
