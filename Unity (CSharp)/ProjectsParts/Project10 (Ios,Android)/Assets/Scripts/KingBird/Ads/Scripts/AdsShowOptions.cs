using System;
using UnityEngine;

namespace KingBird.Ads {
    public class AdsShowOptions {
        public Action<AdsShowResult> ResultCallback = null;
        public AdsShowOptionsType ShowOptionsType = AdsShowOptionsType.None;
        public GameObject BannerPrefab;
        public float CameraDepth = 10f;
        public float CameraZPos = -1000f;

        public AdsShowOptions OnComplete(Action<AdsShowResult> onComplete) {
            ResultCallback = onComplete;
            return this;
        }

        public AdsShowOptions SetShowOptionsType(AdsShowOptionsType showOptionsType) {
            ShowOptionsType = showOptionsType;
            return this;
        }

        public AdsShowOptions SetCameraDepth(float cameraDepth) {
            CameraDepth = cameraDepth;
            return this;
        }

        public AdsShowOptions SetCameraZPos(float cameraZPos) {
            CameraZPos = cameraZPos;
            return this;
        }

        public AdsShowOptions SetBannerPrefab(GameObject bannerPrefab) {
            BannerPrefab = bannerPrefab;
            return this;
        }
    }
}