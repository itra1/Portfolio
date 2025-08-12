using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class PushManager : Singleton<PushManager> {

	public List<SavePushParament> pushOrder = new List<SavePushParament>();
	
	public void SubscribePush() {
		Invoke("Subscribe",5);
	}
	
	private void Subscribe() {
		NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
	}
	
	public void Save() {
		PlayerPrefs.SetString("push", Newtonsoft.Json.JsonConvert.SerializeObject(pushOrder));
	}

	public void Load() {
		if(!PlayerPrefs.HasKey("push")) return;
		pushOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SavePushParament>>(PlayerPrefs.GetString("push"));
	}

	public string CreatePush(string body, DateTime date, bool isRepeat = false) {

		date = CheckDate(date);
		//Запрет отправлять позже 11 вечера и раньше 11 утра

		//date = CheckDate(date);
		

		var notifyInfo = new CrossPlatformNotification();

		notifyInfo.AlertBody = body;
		notifyInfo.FireDate = date;

		if(isRepeat)
			notifyInfo.RepeatInterval = eNotificationRepeatInterval.DAY;
#if UNITY_IOS
		notifyInfo.iOSProperties = new CrossPlatformNotification.iOSSpecificProperties();
		notifyInfo.iOSProperties.AlertAction = Application.productName;
#elif UNITY_ANDROID
		notifyInfo.AndroidProperties = new CrossPlatformNotification.AndroidSpecificProperties();
		notifyInfo.AndroidProperties.ContentTitle = Application.productName;
#endif
		
#if UNITY_EDITOR
		return "";
#endif
		
		string pushId = NPBinding.NotificationService.ScheduleLocalNotification(notifyInfo);

		var saveElem = new SavePushParament() {
			pushId = pushId,
			date = date
		};
		pushOrder.Add(saveElem);

		Save();

		return pushId;
	}

	public DateTime CheckDate(DateTime date) {

		bool change = true;

		while (change) {
			change = false;
			pushOrder = pushOrder.OrderBy(x => x.date).ToList();

			for (int i = 0; i < pushOrder.Count; i++) {

				if (pushOrder[i].date.AddHours(-3) < date && date < pushOrder[i].date.AddHours(3)) {
					change = true;
					date = pushOrder[i].date.AddHours(3);
				}

			}

			if (date.Hour >= 23) {
				change = true;
				date = date.AddHours(11 + (24 - date.Hour));
			} else if (date.Hour <= 10) {
				change = true;
				date = date.AddHours(11 - date.Hour);
			}

		}
		
		return date;
	}

	public void RemovePush(string pushId) {
		pushOrder.RemoveAll(x => x.pushId == pushId);
		
		NPBinding.NotificationService.CancelLocalNotification(pushId);
		Save();
	}
	public void RemoveAllPush() {
		pushOrder.Clear();
		NPBinding.NotificationService.ClearNotifications();
		Save();
	}

}

public class SavePushParament {
	public string pushId;
	public DateTime date;
}