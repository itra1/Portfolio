using System.Collections;
using System.Collections.Generic;
using com.ootii.Actors;
using UnityEngine;

namespace it.Game.Enemy.Boses.Skeleton {
    public abstract class SkeletonAnimationDriverBase : AnimatorDriver {

        protected SkeletonBehaviour m_Behaviour;

        public virtual void Initiate (SkeletonBehaviour behaviour) {
            m_Behaviour = behaviour;
        }

        public virtual void DeInitiate () {

        }

        protected override void Awake () {
            base.Awake();
        }

        protected override void Update () {
            base.Update();

        }

    }
}