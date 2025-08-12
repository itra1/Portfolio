using UnityEngine;

namespace KingBird.Ads {

    public class WaitTimerYieldInstruction : CustomYieldInstruction {
        private float _timeLeft;
        private float _lastTime;

        public override bool keepWaiting {
            get {
                _timeLeft -= Time.unscaledDeltaTime;
                return _timeLeft > 0;
            }
        }

        public WaitTimerYieldInstruction(float time) {
            Reset(time);
        }

        public void Reset(float time = 0) {
            if (time == 0) {
                _timeLeft = _lastTime;
            } else {
                _lastTime = _timeLeft = time;
            }
        }
    }
}