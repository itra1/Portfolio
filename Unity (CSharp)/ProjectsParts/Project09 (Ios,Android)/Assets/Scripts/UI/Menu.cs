using UnityEngine;
using System;

/// <summary>
/// Навигация
/// </summary>
public class Menu : MonoBehaviour {

  public GameObject facebookBtn;

  public Action OnAuthorization;      // Авторизация
  public Action OnUser;               // Игрок
  public Action OnProfiles;           // Профиль
  public Action OnCharacter;          // Профиль
  public Action OnCards;              // Бой
  public Action OnBattleBot;             // Бой
  public Action OnBattlePlayer;             // Бой

	private void OnEnable() {
		OnAuth();
		SocialFb.OnAuthChange += OnAuth;

	}

	private void OnDisable() {
		SocialFb.OnAuthChange -= OnAuth;
	}

	void OnAuth() {
		if(!SocialFb.CheckFbLogin)
			FacebookBtnActive(true);
	}

	/// <summary>
	/// Авторизация
	/// </summary>
	public void AuthorizationButton() {
    if(OnAuthorization != null) OnAuthorization();
  }
  public void UserButton() {
    if(OnUser != null) OnUser();
  }
  public void ProfilesButton() {
    if(OnProfiles != null) OnProfiles();
  }
  public void CharacterButton() {
    if(OnCharacter != null) OnCharacter();
  }
  public void CardsButton() {
    if(OnCards != null) OnCards();
  }
  public void BattleButtonBot() {
    if(OnBattleBot != null) OnBattleBot();
  }
  public void BattleButtonPlayer() {
    if(OnBattlePlayer != null) OnBattlePlayer();
  }
  
  public void FacebookBtnActive(bool isActive) {
    facebookBtn.SetActive(isActive);
  }


}
