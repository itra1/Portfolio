using System.Collections;
using System.Collections.Generic;
using com.ootii.Actors;
using com.ootii.Geometry;
using com.ootii.Timing;
using UnityEngine;

namespace it.Game.Enemy.Boses.Skeleton {

    public class BattleDriver : SkeletonAnimationDriverBase {

        #region Property

        /// <summary>
        /// Отрисовка дебага
        /// </summary>
        [SerializeField]
        private bool m_IsDebug = true;

        /// <summary>
        /// Вектор движения
        /// </summary>
        private Vector3 m_Movement;

        /// <summary>
        /// Целевая позиция при перемещении
        /// </summary>
        protected Vector3 m_TargetPosition = Vector3.zero;
        private bool m_Attack = false;
        private float m_LastAttack = 0;
        private float m_AttackWait = 5f;
        /// <summary>
        /// Determines how far from the destination we'll consider
        /// us to have arrived
        /// </summary>
        public float m_StopDistance = 1f;
        public float StopDistance {
            get { return m_StopDistance; }
            set { m_StopDistance = value; }
        }

        /// <summary>
        /// Distance we'll use to start slowing down so we can arrive nicely.
        /// </summary>
        public float _SlowDistance = 4.0f;
        public float SlowDistance {
            get { return _SlowDistance; }
            set { _SlowDistance = value; }
        }
        /// <summary>
        /// Смещение от центра до точки атаки
        /// </summary>
        [SerializeField]
        private float m_offsetToAttackHand = 1.5f;
        [SerializeField]
        private float m_offsetToAttackKluka = 1.5f;

        private float AttackOffset {
            get {
                switch (m_Ability) {
                    case 1:
                        return m_offsetToAttackKluka;
                    default:
                        return m_offsetToAttackHand;
                }
            }
        }
        private float m_attackHitTimeHand = 1.25f;
        private float m_attackHitTimeKluka = 0.2f;

        private float AttackHitTime {
            get {
                switch (m_Ability) {
                    case 1:
                        return m_attackHitTimeKluka;
                    default:
                        return m_attackHitTimeHand;
                }
            }
        }

        private float TimeAnimation {
            get {
                switch (m_Ability) {
                    case 1:
                        // Клюка
                        return 2.1f;
                    default:
                        // Рукой
                        return 2.1f;
                }
            }
        }
        private float m_SideOffset = 0;
        private float m_Ability;
        public float _PathHeight = 0.05f;
        public float PathHeight {
            get { return _PathHeight; }
            set { _PathHeight = value; }
        }

        /// <summary>
        /// Distance between the current position and actual target
        /// </summary>
        protected float mTargetDistance = 0f;

        /// <summary>
        /// Set when we're within the slow distance
        /// </summary>
        protected bool mIsInSlowDistance = false;
        public bool IsInSlowDistance {
            get { return IsInSlowDistance; }
        }
        /// <summary>
        /// Speed we'll ultimately reduce to before stopping
        /// </summary>
        public float _SlowFactor = 0.25f;
        public float SlowFactor {
            get { return _SlowFactor; }
            set { _SlowFactor = value; }
        }

        private Quaternion m_Rotation = Quaternion.identity;

        private AnimationSate m_AnimationState = AnimationSate.move;
        private enum AnimationSate {
            catScene = 0,
            move = 1,
            attack = 2
        }

        /// <summary>
        /// Текущее состояние
        /// </summary>
        private State m_State;
        private int m_SubState;

        private int m_StateIndex;

        /// <summary>
        /// Время до новой проверки остояния
        /// </summary>
        private float m_StateChangeTime = 0;
        private enum State {
            idle,
            moveForward,
            moveBack,
            speedOffset,
            attack,
            attackMove
        }
        private int m_LastDriffPhase = 0;
        private int m_LastAttackPhase = 0;

        private Vector3 m_VectorDrift;

        /// <summary>
        /// Время начала атаки
        /// </summary>
        private float m_StartAttack;

        #endregion

        /// <summary>
        /// Проверка состояния
        /// </summary>
        private void CheckState () {

            // Атака в движении
            if (ForwardMoveAttackCheck ()) {
                SetState (State.attackMove, m_AttackWait);
                return;
            }
            // Атакуем игрока
            if (AttackContactCheck () && (Random.value < 0.7f)) {
                SetState (State.attack, m_AttackWait);
                return;
            }
            // Смещение
            if (SpeedOffsetCheck() && (Random.value < 0.3f))
            {
                SetState(State.speedOffset, Random.Range(0.7f, 1f));
                return;
            }
            // Ждем
            if (IdleCheck () ) {
                SetState (State.idle, Random.Range (0.5f, 1.5f));
                return;
            }

            // //Атакуем игрока
            // if (CheckAttackMove () && (Random.value < 0.7f && (m_StateIndex - m_LastAttackPhase >= 5 || m_LastAttackPhase == 0))) {
            //     SetState (State.attackMove, m_AttackWait);
            //     return;
            // }

            // //Двигаемся к игроку
            // if (!CkeckDistance () && m_State != State.moveBack) {
            //     SetState (State.moveForward, 0);
            //     return;
            // }

            // // Атакуем игрока
            // if (CheckAttack () && (Random.value < 0.7f || m_StateIndex - m_LastAttackPhase >= 2)) {
            //     SetState (State.attack, m_AttackWait);
            //     return;
            // }

            // //Выполняем выполняем случайное действие из движение назад, или в сторону или подождем
            // float randomAction = Random.value;
            // if (randomAction < 0.2f && (m_State != State.moveBack && m_State != State.driffSide)) {
            //     SetState (State.moveBack, Random.Range (0.4f, 0.7f));
            // } else if (randomAction < 0.4f || m_State == State.moveBack) {
            //     SetState (State.idle, Random.Range (0.5f, 1.5f));
            // } else if (m_State != State.driffSide && m_StateIndex - m_LastDriffPhase >= 2) {
            //     SetState (State.driffSide, Random.Range (0.7f, 1f));
            // }

        }
        /// <summary>
        /// Инифиализация
        /// </summary>
        /// <param name="behaviour">Основной контроллер</param>
        public override void Initiate (SkeletonBehaviour behaviour) {
            base.Initiate (behaviour);
            m_Behaviour.EffectsComponent.DeactiveLineRenderer ();
      InputSourceOwner = null;
        }
    private void Start()
    {

      InputSourceOwner = null;
    }

    protected override void Awake () {
            base.Awake ();
            m_StateIndex = 0;
            m_LastDriffPhase = 0;
            m_LastAttack = -m_AttackWait;
      InputSourceOwner = null;
    }
    
        /// <summary>
        /// Установка состояния
        /// </summary>
        /// <param name="state">Устанвока состояния</param>
        /// <param name="timeWait">Время удержания</param>
        private void SetState (State state, float timeWait) {
            if (m_StateChangeTime > Time.time) {
                return;
            }

            if ((m_State == State.moveForward || m_State == State.speedOffset) && state == State.moveBack)
                return;

            if (m_State == state) {
                return;
            }

            m_StateIndex++;
            Debug.Log (state);
            m_State = state;
            m_StateChangeTime = Time.time + timeWait;
            if (m_State == State.speedOffset)
                SpeedOffsetInit ();
            if (m_State == State.attack || m_State == State.attackMove) {
                m_LastAttack = Time.time;
                m_LastAttackPhase = m_StateIndex;

                if (m_State == State.attackMove)
                    ForwardMoveAttackInit ();
            }
        }

        #region Отрисовка

        private void OnDrawGizmosSelected () {
            if (!m_IsDebug)
                return;

            Gizmos.DrawLine (transform.position, transform.position + transform.forward * m_offsetToAttackHand + Vector3.up * 2);
            Gizmos.color = Color.red;
            Gizmos.DrawLine (transform.position, transform.position + transform.forward * m_offsetToAttackKluka + Vector3.up * 2);
            Gizmos.DrawWireSphere (m_TargetPosition, 0.1f);

        }

        #endregion

        protected override void Update () {

            if (!_IsEnabled || it.Game.Player.PlayerBehaviour.Instance == null) { return; }
            //if (IsWaitAttack ()) { return; }

            CheckState ();

            if (m_State == State.moveForward) {
                RotationToPlayer ();
                MoveToPlayer ();
                m_AnimationState = AnimationSate.move;

            }
            if (m_State == State.idle) {
                IdleUpdate ();
                //AttackPhase ();
                m_AnimationState = AnimationSate.move;
            }
            if (m_State == State.attack) {
                AttackContactUpdate ();
                //AttackPhase ();
                m_AnimationState = AnimationSate.attack;
            }
            if (m_State == State.moveBack) {
                RotationToPlayer ();
                MoveToPlayer (true);
                m_AnimationState = AnimationSate.move;

            }
            if (m_State == State.speedOffset) {
                SpeedOffsetUpdate ();
                m_AnimationState = AnimationSate.move;

            }
            if (m_State == State.attackMove) {
                ForwardMoveAttackUpdate ();
                //AttackPhase ();
                //MoveToPlayerPhase ();

            }

            //m_Rigidbody.velocity = transform.forward * velocity * m_Speed * Time.deltaTime;
            SetAnimatorProperties (Vector3.zero, m_Rotation);

            m_Attack = false;

        }

        #region Move

        /// <summary>
        /// поворот тела в направление игрока
        /// </summary>
        private void RotationToPlayer () {

            m_Rotation = Quaternion.identity;
            CalculateRotation (ref m_Rotation, it.Game.Player.PlayerBehaviour.Instance.transform.position);
            mActorController.Rotate (m_Rotation);
        }
        /// <summary>
        /// Поворот тела в направлении цели
        /// </summary>
        private void RotationToTarget () {

            m_Rotation = Quaternion.identity;
            CalculateRotation (ref m_Rotation, m_TargetPosition);
            mActorController.Rotate (m_Rotation);
        }
        /// <summary>
        /// Рассчет вращения
        /// </summary>
        /// <param name="rRotate">ref Quaternion вращения</param>
        /// <param name="lockTarget">Объект, в направление которого нужно вращать</param>
        protected virtual void CalculateRotation (ref Quaternion rRotate, Vector3 lockTarget) {
            float lDeltaTime = TimeManager.SmoothedDeltaTime;

            // Direction we need to travel in
            Vector3 lDirection = lockTarget - transform.position;
            lDirection.y = lDirection.y - _PathHeight;
            lDirection.Normalize ();

            // Determine our rotation
            Vector3 lVerticalDirection = Vector3.Project (lDirection, transform.up);
            Vector3 lLateralDirection = lDirection - lVerticalDirection;

            float lYawAngle = Vector3Ext.SignedAngle (transform.forward, lLateralDirection);

            if (_RotationSpeed == 0f) {
                rRotate = Quaternion.AngleAxis (lYawAngle, transform.up);
            } else {
                rRotate = Quaternion.AngleAxis (Mathf.Sign (lYawAngle) * Mathf.Min (Mathf.Abs (lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
            }

        }
        /// <summary>
        /// Фаза движения к игроку
        /// </summary>
        private void MoveToPlayer (bool inverce = false, bool driftSide = false) {

            m_Movement = Vector3.zero;
            Vector3 l_TargetPosition = it.Game.Player.PlayerBehaviour.Instance.transform.position - transform.position;
            mTargetDistance = l_TargetPosition.magnitude;

            CalculateMove (ref m_Movement, m_Rotation, inverce, driftSide);

            // if (m_State == State.attackMove) {

            //     if (m_SubState == 0) {
            //         m_Movement = m_Movement.normalized * m_moveAttackTargetSpeed * Time.deltaTime;
            //     }
            // }

            mActorController.Move (m_Movement);
        }
        /// <summary>
        /// Перемещение к цели
        /// </summary>
        /// <param name="inverce"></param>
        /// <param name="driftSide"></param>
        private void MoveToTarget (bool inverce = false, bool driftSide = false) {

            m_Movement = Vector3.zero;
            Vector3 l_TargetPosition = m_TargetPosition - transform.position;
            mTargetDistance = l_TargetPosition.magnitude;

            CalculateMove (ref m_Movement, m_Rotation, inverce, driftSide);

            mActorController.Move (m_Movement);
        }

        protected virtual void CalculateMove (ref Vector3 rMove, Quaternion rRotate, bool inverce = false, bool driftSide = false) {
            float lDeltaTime = TimeManager.SmoothedDeltaTime;

            // Grab the base movement speed
            float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
            if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }
            lMoveSpeed = _MovementSpeed;

            // Calculate our own slowing
            float lRelativeMoveSpeed = 1f;
            if (mIsInSlowDistance && _SlowFactor > 0f) {
                float lSlowPercent = (mTargetDistance - m_StopDistance) / (_SlowDistance - m_StopDistance);
                lRelativeMoveSpeed = ((1f - _SlowFactor) * lSlowPercent) + _SlowFactor;
            }

            // TRT 4/5/2016: Force the slow distance as an absolute value
            if (mIsInSlowDistance && _SlowFactor > 0f) {
                lMoveSpeed = _SlowFactor;
                lRelativeMoveSpeed = 1f;
            }

            // Set the final velocity based on the future rotation
            Quaternion lFutureRotation = transform.rotation * rRotate;

            if (driftSide) {
                rMove = Quaternion.AngleAxis (90 * m_VectorDrift.x, Vector3.up) * lFutureRotation.Forward () * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime) * (inverce ? -1 : 1);
            } else {

                if (m_State == State.attackMove) {
                    rMove = lFutureRotation.Forward ().normalized * (m_moveAttackTargetSpeed * lRelativeMoveSpeed * Time.deltaTime) * (inverce ? -1 : 1);
                } else
                    rMove = lFutureRotation.Forward () * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime) * (inverce ? -1 : 1);
            }

        }

        #endregion

        #region Фазы

        private bool CkeckDistance () {
            if (it.Game.Player.PlayerBehaviour.Instance == null)
                return false;

            Vector3 l_TargetVector = it.Game.Player.PlayerBehaviour.Instance.transform.position - transform.position;
            float l_Distance = l_TargetVector.magnitude;
            return m_StopDistance > l_Distance;

        }

        private bool IsWaitAttack () {
            return m_LastAttack + 2 > Time.time;
        }

        private float m_moveAttackTargetSpeed;

        #region Бездействие

        private bool IdleCheck () {
            return true;
        }

        private void IdleUpdate () {

            RotationToPlayer ();
        }

        #endregion

        #region Прямая атака с движением

        private void ForwardMoveAttackInit () {
            m_Ability = Random.value <= 0.5f ? 0 : 1;
            Vector3 vectortarget = it.Game.Player.PlayerBehaviour.Instance.transform.position - transform.position;
            m_TargetPosition = transform.position + vectortarget.normalized * (vectortarget.magnitude - AttackOffset);
            m_SubState = 0;
            m_StateChangeTime = Time.time + TimeAnimation;

            m_moveAttackTargetSpeed = (m_TargetPosition - transform.position).magnitude / AttackHitTime;
            m_AnimationState = AnimationSate.move;
        }

        private bool ForwardMoveAttackCheck () {
            Vector3 l_TargetVector = it.Game.Player.PlayerBehaviour.Instance.transform.position - transform.position;
            float l_Distance = l_TargetVector.magnitude;
            return 2 < l_Distance; // (&& m_LastAttack + m_AttackWait < Time.time);
        }

        private void ForwardMoveAttackUpdate () {

            if (m_SubState == 2)
            {
                if (m_StateChangeTime < Time.time)
                {
                    m_SubState = 3;
                    ForwardMoveAttackInit();
                }
            }

            // Ускорение с атакой
            if (m_SubState == 1) {
                MoveToTarget ();
                if (m_StartAttack + AttackHitTime < Time.time)
                    m_SubState = 2;
            }

            // Поворачиваемся к игроку
            if (m_SubState == 0) {
                RotationToTarget ();

                if (Mathf.Abs (m_Rotation.eulerAngles.y) < 1f) {
                    m_StartAttack = Time.time;
                    m_AnimationState = AnimationSate.attack;
                    m_SubState = 1;
                }
            }

        }

        #endregion

        #region Атака при контакте

        /// <summary>
        /// Проверка возможности атаковать
        /// </summary>
        /// <returns></returns>
        private bool AttackContactCheck () {
            return m_LastAttack + m_AttackWait < Time.time;
        }

        private void AttackContactInit () {

        }

        private void AttackContactUpdate () {
            RotationToPlayer ();
        }

        #endregion

        #region Смещение в сторону

        private void SpeedOffsetInit () {
            m_LastDriffPhase = m_StateIndex;;
            m_VectorDrift = Random.value <= 0.5f ? Vector3.left : Vector3.right;
        }

        private bool SpeedOffsetCheck () {
            return true;
        }

        private void SpeedOffsetUpdate () {
            //otationToPlayer ();
            MoveToPlayer (false, true);
        }

        #endregion

        #endregion
        /// <summary>
        /// Установка анимации
        /// </summary>
        /// <param name="rInput"></param>
        /// <param name="rRotation"></param>
        protected void SetAnimatorProperties (Vector3 rInput, Quaternion rRotation) {
            float lDeltaTime = TimeManager.SmoothedDeltaTime;
            if (m_State == State.moveForward || m_State == State.attackMove) {
                mAnimator.SetFloat ("Forward", 1);
            } else if (m_State == State.moveBack) {
                mAnimator.SetFloat ("Forward", -1);
            } else
                mAnimator.SetFloat ("Forward", 0);
            mAnimator.SetInteger ("State", (int) m_AnimationState);
            mAnimator.SetFloat ("SideOffset", m_VectorDrift.x);
            mAnimator.SetFloat ("Ability", m_Ability);
        }

    }
}