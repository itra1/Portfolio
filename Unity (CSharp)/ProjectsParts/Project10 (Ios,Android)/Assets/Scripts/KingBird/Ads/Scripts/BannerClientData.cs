using System;
using UnityEngine;

namespace KingBird.Ads {
    [Serializable]
    public class BannerClientData {
        public int id; // Уникальный id баннера
        public int advertAppId; // id приложения
        public string title; // Переведенный в локаль заголовок (или ключ, если что-то сильно сломалось)
        public string description; // -II-
        public BannerPlacement placement; // Enum в виде Int
        public int showTime; // Время показа баннера (для некоторых плейсментов)
        public string url; // URL ресурса (картинка баннера или ссылка на видео)
        public ApplicationStoreData storeData; // Информация о магазине
        
        // [JsonIgnore]
        private Texture2D _texture2d = null;
        private bool _shown = false;
        private bool _clicked = false;
        private bool _isFullReady = false;

        public Texture2D GetTexture2D() {
            return _texture2d;
        }

        public void SetTexture2D(Texture2D texture2D) {
            _texture2d = texture2D;
            _isFullReady = true;
        }

        public void Show() {
            _shown = true;
        }

        public void Click() {
			Debug.Log("Click");
            _clicked = true;
        }

        public void NoClick() {
            _clicked = false;
        }

        public bool IsShown() {
            return _shown;
        }

        public bool IsClicked() {
            return _clicked;
        }

        public bool IsFullReady() {
            return _isFullReady;
        }
        
        public Sprite GetSprite() {
            return Sprite.Create(_texture2d,
                new Rect(0.0f, 0.0f, _texture2d.width, _texture2d.height),
                new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
}