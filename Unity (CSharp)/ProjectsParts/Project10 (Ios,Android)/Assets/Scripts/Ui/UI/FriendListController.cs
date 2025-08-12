/* *************************
 * Контроллер по списком друзей
 * *************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public struct FriendItemList {
    public string id;
    public string name;
    public string bestDistantion;
    public string picture;
}

public class FriendListController : MonoBehaviour {

    public GameObject listField;                                // Область со списком
    public GameObject listItem;                                 // Предмет списка
    GameObject[] createItems;                                   // Массив созданных элементов
    List<FriendItemList> FriendsInGameBest;
    public GameObject invateList;
    

    void OnEnable() {
        
        StartCoroutine(Apikbs.Instance.GetLeaderboardFb(DownloadFriendList));
    }

    void OnDisable() {
        for (int iii = 0; iii < listField.transform.childCount; iii++)
            Destroy(listField.transform.GetChild(iii).gameObject);
    }

    void DownloadFriendList(List<LeaderboardItem> resFriends) {

        if (resFriends.Count == 0) return;

        RectTransform listScale = listField.GetComponent<RectTransform>();
        listScale.sizeDelta = new Vector2(listScale.sizeDelta.x, 83 * resFriends.Count + 150);
				int newMaxDistance = (int)UserManager.Instance.survivleMaxRunDistance;

        int num = 0;
        foreach (LeaderboardItem friend in resFriends) {
            GameObject clone = Instantiate(listItem, Vector3.zero, Quaternion.identity) as GameObject;
            clone.GetComponent<ItemListFriendController>().SetName(friend.name);
            clone.GetComponent<ItemListFriendController>().SetNumber((num + 1).ToString());

            if(friend.fb == FBController.GetUserId && newMaxDistance > int.Parse(friend.bestDistantion)) {
                clone.GetComponent<ItemListFriendController>().SetStatCount(newMaxDistance.ToString());

#if UNITY_IOS || UNITY_ANDROID
                Apikbs.Instance.SaveLeaderboardFbValue(newMaxDistance);
#endif
            } else
                clone.GetComponent<ItemListFriendController>().SetStatCount(friend.bestDistantion);
            clone.GetComponent<ItemListFriendController>().SetPicture(friend.picture);

            if (friend.fb == FBController.GetUserId)
                clone.GetComponent<ItemListFriendController>().IsMy();

            //clone.transform.parent = listField.transform;
            clone.GetComponent<RectTransform>().SetParent(listField.transform);
            clone.transform.localScale = Vector3.one;
            clone.transform.localPosition = Vector3.zero;

            clone.transform.localPosition = new Vector2(0, -(83 * num));
            //if (num % 2 > 0) clone.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            num++;
            clone.SetActive(true);
        }

        invateList.transform.localPosition = new Vector2(0, -(83 * num)-70);

    }

}
