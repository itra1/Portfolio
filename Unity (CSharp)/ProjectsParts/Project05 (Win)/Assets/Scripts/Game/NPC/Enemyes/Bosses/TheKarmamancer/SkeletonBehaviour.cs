using System.Collections;
using System.Collections.Generic;
using com.ootii.Actors;
using UnityEngine;

namespace it.Game.Enemy.Boses.Skeleton {
    public class SkeletonBehaviour : MonoBehaviourBase {

        /// <summary>
        /// Текущее состояние
        /// </summary>
        private StateType m_State = StateType.none;
        /// <summary>
        /// Состояние
        /// </summary>
        /// <value></value>
        public StateType State {
            get {
                return m_State;
            }
            set {
                if (m_State == value)
                    return;
                m_State = value;
                ConfirmState ();
            }
        }
        /// <summary>
        /// Типы состояния
        /// </summary>
        public enum StateType {
            none,
            catScene,
            battle
        }

        public SkeletonEffects EffectsComponent { get; private set; }

        /// <summary>
        /// Текущий брайвер
        /// </summary>
        private SkeletonAnimationDriverBase m_Driver;
        /// <summary>
        /// применение фазы
        /// </summary>
        private void ConfirmState () {

            if (m_Driver != null) {
                m_Driver.DeInitiate ();
                Destroy (m_Driver);
            }

            switch (m_State) {
                case StateType.battle:
                    m_Driver = gameObject.AddComponent<BattleDriver> ();
                    break;
                case StateType.catScene:
                    m_Driver = gameObject.AddComponent<CatSceneDriver> ();
                    break;
            }
            m_Driver.Initiate (this);
        }

        private void Awake () {
            //GetComponents ();
           // State = StateType.battle;
        }

        private void GetComponents () {
            EffectsComponent = GetComponent<SkeletonEffects> ();
        }
    }
}