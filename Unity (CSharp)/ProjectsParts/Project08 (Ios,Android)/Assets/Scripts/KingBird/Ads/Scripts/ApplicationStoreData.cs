using System;

namespace KingBird.Ads {
    [Serializable]
    public class ApplicationStoreData {
        public StoreType store; // Enum в виде Int
        public string storeId; // Уникальный ID приложения в магазине
        public string storeUrl; // Ссылка на приложение в магазине в формате браузера
    }
}