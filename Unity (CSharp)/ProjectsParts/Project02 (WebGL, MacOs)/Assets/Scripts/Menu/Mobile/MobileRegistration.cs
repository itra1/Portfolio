using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class MobileRegistration : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown CountryDropdown;

    [SerializeField] private TMP_InputField EmailInput;
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private TMP_Text LoginErrorOutput;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private TMP_InputField PromoInput;

    [SerializeField] private Toggle RulesCheckbox;

    [SerializeField] private GameObject SuccessPopup;

    public bool RulesCheckboxValue = false;
    private GpCountryResponce AvaliableCountries;

    private int SelectedCountryName = 0;

    private void Awake()
    {
        ClearInputs();
    }
    public void ClearInputs()
    {
        CountryDropdown.value = 0;
        EmailInput.text = "";
        if (NicknameInput) NicknameInput.text = "";
        PasswordInput.text = "";
        LoginErrorOutput.text = "";
        if (PromoInput) PromoInput.text = "";
    }
    
    public void SetrulesCheckboxValue(bool isChecked)
    {
        RulesCheckboxValue = isChecked;
    }

    void ShowWrongEmail()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Wrong Email";
    }

    void CheckBoxError()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Apply rules to register";
    }


    void ShowWronNickNameWhitespace()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Nickname contains whitespaces, this is not allowed";
    }

    void ShowPasswordWhitespacesError()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Password cant contain whitespaces";
    }

    void ShowPasswordShort()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Password too short";
    }

    void ShowPasswordTooLong()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Password too long";
    }

    void ShowNicknameTooShort()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Nickname is too short";
    }

    void ShowNicknameContainsWhitespaces()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Nickname contains whitespaces";
    }

    void ShowRegisterError()
    {
        SuccessPopup.SetActive(false);
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Something went wrong. May be such email or nickname already registered.";
    }


    public void SelectCountry(int id)
    {
        SelectedCountryName = AvaliableCountries.data[id].id;
    }

    public void ShowChooseCountry()
    {
        LoginErrorOutput.gameObject.SetActive(true);
        LoginErrorOutput.text = "Select Country";
    }

    
    public void Register()
    {
        string nick = NicknameInput == null ? "test_nickname" : NicknameInput.text;

        if (RulesCheckboxValue == false)
        {
            CheckBoxError();

            return;
        }

        if (Validator.Email(EmailInput.text) != -1)
        {
            ShowWrongEmail();
            return;
        }

        if (nick.Any(Char.IsWhiteSpace))
        {
            ShowWronNickNameWhitespace();
            return;
        }

        if (PasswordInput.text.Any(Char.IsWhiteSpace))
        {
            ShowPasswordWhitespacesError();
            return;
        }

        if (PasswordInput.text.Length < 8)
        {
            ShowPasswordShort();
            return;
        }

        if (PasswordInput.text.Length > 20)
        {
            ShowPasswordTooLong();
            return;
        }

        if (nick.Length < 4)
        {
            ShowNicknameTooShort();
            return;
        }
        if (nick.Any(Char.IsWhiteSpace))
        {
            ShowNicknameContainsWhitespaces();
            return;
        }

        if (SelectedCountryName == 0)
        {
            ShowChooseCountry();
            return;
        }
        if (PromoInput && PromoInput.text == null)
        {
            PromoInput.text = "";
        }




        RegisterRequest regInfo = new RegisterRequest()
        {
            email = EmailInput.text,
            password = PasswordInput.text,
            nickname = nick,
            country_id = SelectedCountryName,
            promo = PromoInput != null ? PromoInput.text : "test"
        };


        //var result = RegisterAction(regInfo);




    }

    //private RequestException registerError;
    //public async Task<RegisterRequest> RegisterAction(RegisterRequest info)
    //{
    //    var tcs = new TaskCompletionSource<RegisterRequest>();

    //    //RestClient.Post<RegisterRequest>(Config.BasePath + "auth/reg_user", info.ToString())
    //    //    .Then(res => tcs.SetResult(res))
    //    //    .Catch(err =>
    //    //    {
    //    //        var requestException = (RequestException)err;
    //    //        registerError = requestException;
    //    //        it.Logger.Log((int)requestException.StatusCode);

    //    //        tcs.SetResult(null);
    //    //        ShowRegisterError();
    //    //    });

    //    //if (registerError != null)
    //    //{

    //    //    registerError = null;
    //    //}
    //    //else
    //    //{
    //    //    SuccessPopup.SetActive(true);
    //    //}


    //    return await tcs.Task;
    //}

    private void OnEnable()
    {
        ClearInputs();
        GetCountries();
    }

    void GetCountries()
    {
        StartCoroutine(MakeCountriesReques("ru"));

    }

    public void PasswordSee(bool bl)
    {
        PasswordInput.contentType = bl ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        string txt = PasswordInput.text;
        PasswordInput.text = "";
        PasswordInput.text = txt;
    }


    IEnumerator MakeCountriesReques(string LangName)
    {
        UnityWebRequest request = UnityWebRequest.Get(Config.BasePath + "reference/gp/countries?lang=" + LangName);

        yield return request.SendWebRequest();

        if (((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError)))
        {
            it.Logger.Log(request.error);
        }
        else
        {
            it.Logger.Log(request.downloadHandler.text);
            GpCountryResponce countryResponce = new GpCountryResponce();
            countryResponce = Jsonable.ToObject<GpCountryResponce>(request.downloadHandler.text);
            
            AvaliableCountries = countryResponce;
            

            List<String> bumpOptionsList = new List<string>();
                CountryDropdown.ClearOptions();

            bumpOptionsList.Add("Choose Country");

                for (int a = 0; a < countryResponce.data.Count; a++)
                {
                    bumpOptionsList.Add(AvaliableCountries.data[a].title);
                }

                CountryDropdown.AddOptions(bumpOptionsList);
            CountryDropdown.value =0;

        }
    }

    
    [Serializable]
    public class RegisterRequest : Jsonable
    {
        public string email;
        public string password;
        public string nickname;
        public int country_id;
        public string promo;
    }

    [Serializable]
    public class Datum : Jsonable
    {
        public int id;
        public string title;
        public object short_title;
    }
    
    [Serializable]
    public class GpCountryResponce : Jsonable
    {
        public List<Datum> data;
    }
}

