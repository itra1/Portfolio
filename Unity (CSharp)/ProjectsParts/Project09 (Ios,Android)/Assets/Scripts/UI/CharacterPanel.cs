using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Панель персонажаы
/// </summary>
public class CharacterPanel : MonoBehaviour {

  public Actione<Character> OnButton;

  public InputField nameInput;
  
  public GameObject createUiButton;
  public GameObject updateUiButton;

  Character character = new Character();

  void OnEnable() {
    Init();
  }

  public void Init() {
    if(User.instance.characters.Count > 0)
      character = User.instance.characters[0];

    if(character.id != null && character.id != "") {
      updateUiButton.SetActive(true);
      createUiButton.SetActive(false);
    } else {

      updateUiButton.SetActive(false);
      createUiButton.SetActive(true);
    }

    if(character.id != null && character.id != "") {
      nameInput.text = character.name;
    }
  }

  public void OnSendButton() {

    character.name = nameInput.text;
    if(OnButton != null) OnButton(character);
    CloseButton();
  }

  public void CloseButton() {
    Destroy(gameObject);
  }

}
