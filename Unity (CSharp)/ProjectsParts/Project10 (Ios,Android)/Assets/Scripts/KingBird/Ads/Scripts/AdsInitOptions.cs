using System;
using UnityEngine;

namespace KingBird.Ads {
    public class AdsInitOptions {

        public string ApplicationId;
        public string Locale = "en-US";
        public int FullBannerMinLaunch = 2;
        public float FullBannerMinGameTime = 10f * 60f;
        
        public AdsInitOptions SetApplicationId(string applicationId) {
            ApplicationId = applicationId;
            return this;
        }

        public AdsInitOptions SetLocale(string locale) {
            Locale = locale;
            return this;
        }
        
        public AdsInitOptions SetFullBannerMinLaunch(int fullBannerMinLaunch) {
            FullBannerMinLaunch = fullBannerMinLaunch;
            return this;
        }
        
        public AdsInitOptions SetFullBannerMinGameTime(float fullBannerMinGameTime) {
            FullBannerMinGameTime = fullBannerMinGameTime;
            return this;
        }
    }
}