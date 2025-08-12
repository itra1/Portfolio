using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.User
{
  public class UserMedicineChest : MonoBehaviour
  {

    private int _secontOne = 1800; // Секунд на 1 аптечку
    private int _maxCount = 4; // Максимальное количество аптечек

    public static event System.Action<int, TimeStruct> OnChange;
    
    /// <summary>
    /// Инифиализация медицинской аптечки
    /// </summary>
    public void Init()
    {

      var up = UserManager.Instance.UserProgress;

      int deltatime = Core.unixTime - up.medicineChestTime;

      if (Core.unixTime > up.medicineChestNextTime && up.medicineChestNextTime != 0) {
        deltatime -= up.medicineChestNextTime - up.medicineChestTime;
        Add(1);
      }

      int newChestCount = (int)Mathf.Floor(deltatime / _secontOne);

      Add(newChestCount);

      if (up.medicineChestCount >= _maxCount)
        up.medicineChestNextTime = 0;

      if (OnChange != null)
        OnChange(up.medicineChestCount,
          Core.GetTimeDecrement(up.medicineChestNextTime));
    }

    /// <summary>
    /// Обновление медицинской аптечки
    /// </summary>
    public void Update() {
      var up = UserManager.Instance.UserProgress;

      if (up.medicineChestCount == _maxCount)
        return;
      up.medicineChestTime = Core.unixTime;

      if (up.medicineChestNextTime == 0)
        up.medicineChestNextTime = Core.unixTime + _secontOne;

      up.medicineChestTime = Core.unixTime;

      if (up.medicineChestTime >= up.medicineChestNextTime) {
        Add(1);
      }

      if (OnChange != null)
        OnChange(up.medicineChestCount,
          Core.GetTimeDecrement(up.medicineChestNextTime));

    }

    /// <summary>
    /// Добавление аптечек
    /// </summary>
    /// <param name="addCount"></param>
    void Add(int addCount) {
      if (addCount == 0)
        return;

      var up = UserManager.Instance.UserProgress;

      up.medicineChestCount += addCount;
      if (up.medicineChestCount >= _maxCount) {
        up.medicineChestCount = _maxCount;
        up.medicineChestNextTime = 0;
      } else {
        up.medicineChestNextTime = Core.unixTime + _secontOne;
      }

      if (OnChange != null)
        OnChange(up.medicineChestCount,
          Core.GetTimeDecrement(up.medicineChestNextTime));
    }

    /// <summary>
    /// Применение аптечки
    /// </summary>
    public void Use() {
      var up = UserManager.Instance.UserProgress;

      if (up.medicineChestCount == 0)
        return;
      if (UserHealth.Instance.IsMax)
        return;
      if (up.medicineChestCount == _maxCount) {
        up.medicineChestNextTime = Core.unixTime + _secontOne;
      }

      up.medicineChestCount--;

      UserHealth.Instance.Value += 20;

      if (OnChange != null)
        OnChange(up.medicineChestCount,
          Core.GetTimeDecrement(up.medicineChestNextTime));
    }

  }

}
