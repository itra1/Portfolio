using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Enemy.Boses.Skeleton {
    public class SkeletonEffects : MonoBehaviourBase {
        [SerializeField]
        private TrailRenderer m_HandTrail;
        [SerializeField]
        private TrailRenderer m_KlikaTrail;

        void Awake () {
            DeactiveLineRenderer ();
        }

        public void DeactiveLineRenderer () {
            //m_HandTrail.emitting = false;
            //m_KlikaTrail.emitting = false;
        }

        public void AnimationEvent (string animationName) {

            //if (animationName == "HandAttackStart")
            //    m_HandTrail.emitting = true;
            //if (animationName == "HandAttackEnd")
            //    m_HandTrail.emitting = false;

            //if (animationName == "KlikaAttackStart")
            //    m_KlikaTrail.emitting = true;
            //if (animationName == "KlikaAttackEnd")
            //    m_KlikaTrail.emitting = false;
        }

    }
}