using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// панель создания профиля
/// </summary>
public class GameProfileCreatePanel : MonoBehaviour {

  public Actione<GameProfile> OnButton;
  
  public InputField nameInput;
  public InputField ratingInput;
  public InputField goldInput;
  public InputField crystalsInput;
  public InputField experienceInput;
  public InputField claneIdInput;

  GameProfile gameProfile;

  public GameObject createUiButton;
  public GameObject updateUiButton;

  void OnEnable() {
    Init();
  }

  public void Init() {
    gameProfile = User.instance.gameProfile;

    if(gameProfile.id != null && gameProfile.id != "") {
      updateUiButton.SetActive(true);
      createUiButton.SetActive(false);
    } else {

      updateUiButton.SetActive(false);
      createUiButton.SetActive(true);
    }

    if(gameProfile.id != null && gameProfile.id != "") {
      nameInput.text = gameProfile.name;
      ratingInput.text = gameProfile.rating.ToString();
      goldInput.text = gameProfile.gold.ToString();
      crystalsInput.text = gameProfile.crystals.ToString();
      experienceInput.text = gameProfile.experience.ToString();
      claneIdInput.text = gameProfile.clan_id;
    }
  }

  public void OnSendButton() {

    gameProfile.name = nameInput.text;
    gameProfile.rating = int.Parse(ratingInput.text);
    gameProfile.gold = int.Parse(goldInput.text);
    gameProfile.crystals = int.Parse(crystalsInput.text);
    gameProfile.experience = int.Parse(experienceInput.text);
    gameProfile.clan_id = claneIdInput.text;
    if(OnButton != null) OnButton(gameProfile);
    CloseButton();
  }
  
  public void CloseButton() {
    Destroy(gameObject);
  }

}
