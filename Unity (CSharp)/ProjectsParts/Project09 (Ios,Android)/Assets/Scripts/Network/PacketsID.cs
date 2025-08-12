using UnityEngine;
public partial class Constants {
  public static Packet getPacket(short packetId) {
    //Debug.Log(string.Format("packetId = {0:X8}", packetId));
    switch(packetId) {
      case 0x0000: return new KeyPacket();                      // Авторизация пакета
      case 0x7101: return new in_Pong();                        // Пинг
      case 0x0301: return new ProfileInfo();                    // Информация о профиле
      case 0x0200: return new AuthLoginResult();                // Авторизация
      // Запуск боя
      case 0x0202: return new ConnectedToBattle();              // Подключение к бою к бою
      case 0x0702: return new PlayerJoinedToBattle();           // Подключение играков
      case 0x0402: return new SpawnObjects();                   // Генерация объектов
      case 0x0802: return new BattleJoinEnd();                  // Окончание подключения к бою
      case 0x0502: return new QueuedToBattle();                 // Постановка в очередь для боя
      case 0x0A02: return new BattleSettings();                 // Настройки боя
      case 0x0D02: return new EnergyUpdate();                   // Обновление энергии
      case 0x0C02: return new HealthUpdate();                   // Обновление жизней
      case 0x0902: return new MoveObjects();                    // Движение объекта
      case 0x0B02: return new ObjectDestroyed();                // Уничтожение объекта
      case 0x0302: return new DisconnectedFromBattle();         // Отклчение от боя
      case 0x0F02: return new RejoiningToBattle();              // Повторное подключение к бою
      case 0x1102: return new TimerInfo();                      // Информация о времени боя
      case 0x1202: return new ResultsBattle();                  // Результат боя
      case 0x1402: return new CardUsed();                       // Применение карты
      case 0x1502: return new CardsQueue();                     // Изменение списка карт
      case 0x1602: return new PlayEffects();                    // Эффекты играков
			case 0x1802: return new ClientBroadcast();								// Рассылка клиента

		}
    return null;
  }
  
  public static int GTIME {
    get {
      return System.Environment.TickCount;
    }
  }

}
