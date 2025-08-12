using System;
using KingBird.Ads;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenBannerWindow : MonoBehaviour {
    private static Color TRANSPARENT = new Color(1, 1, 1, 0);
    private const float EXPAND_TIME = 0.3f;

    [SerializeField] private Image _bannerImage;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Transform _bannerContainer;
    [SerializeField] private Camera _cameraBanner;
    [SerializeField] private Canvas _canvasScaller;

    [SerializeField] private GameObject _timerContainer;
    [SerializeField] private Image _timerButton;
    [SerializeField] private Text _timerValue;

    private AdsShowOptions _adsShowOptions;
    private BannerClientData _bannerClientData;
    private float _closeTimer;
    private float _expandTimer;
    private int _lastTimeValue;
    private Phase _currentPhase = Phase.None;
    private Action<bool> _closeCallback;

    public void Show(AdsShowOptions adsShowOptions, BannerClientData bannerClientData,
        Action<bool> closeCallback = null) {
        _adsShowOptions = adsShowOptions;
        _bannerClientData = bannerClientData;
        _closeCallback = closeCallback;

        _cameraBanner.depth = _adsShowOptions.CameraDepth;
        _cameraBanner.transform.position = new Vector3(0,0,_adsShowOptions.CameraZPos);
        SetBannerImage(_bannerClientData.GetSprite());
        _closeButton.SetActive(false);
        _timerContainer.SetActive(true);
        _timerButton.fillAmount = 0;
        _bannerContainer.localScale = Vector3.zero;
        _closeTimer = bannerClientData.showTime;
        _expandTimer = EXPAND_TIME;
        _currentPhase = Phase.Expand;
    }
    
    private void Update() {
        _closeTimer -= Time.unscaledDeltaTime;
        
        switch (_currentPhase) {
            case Phase.None:
                // Nothing to do, waiting for close button press
                return;
            case Phase.Expand:
                ProcessScale();
                return;
            case Phase.Wait:
                ProcessTimer();
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessScale() {
        _expandTimer -= Time.unscaledDeltaTime;
        var t = (EXPAND_TIME - _expandTimer) / EXPAND_TIME;
        _bannerContainer.localScale = Mathf.Lerp(0, 1, t) * Vector3.one;
        _bannerImage.color = Color.Lerp(TRANSPARENT, Color.white, t);
                
        if (_expandTimer > 0) return;
                
        _currentPhase = Phase.Wait;
    }

    private void ProcessTimer() {
        _timerButton.fillAmount = 1 - _closeTimer / _bannerClientData.showTime;
        var currentValue = Mathf.CeilToInt(_closeTimer);
        if (currentValue != _lastTimeValue) {
            _lastTimeValue = currentValue;
            _timerValue.text = _lastTimeValue.ToString();
        }

        if (!(_closeTimer <= 0)) return;
        
        _closeButton.SetActive(true);
        _timerContainer.SetActive(false);
        _currentPhase = Phase.None;
    }

    private void SetBannerImage(Sprite sprite) {
        _bannerImage.sprite = sprite;
        var scale = Mathf.Max(_canvasScaller.gameObject.GetComponent<RectTransform>().sizeDelta.x / sprite.rect.width,
            _canvasScaller.gameObject.GetComponent<RectTransform>().sizeDelta.y / sprite.rect.height);
        _bannerImage.GetComponent<RectTransform>().sizeDelta =
            scale * new Vector2(sprite.rect.width, sprite.rect.height);
    }

    public void CloseButtonClick() {
        if (_closeCallback != null) _closeCallback(false);
        Hide();
    }

    public void PlayButtonClick() {
        if (_closeCallback != null) _closeCallback(true);
        Application.OpenURL(_bannerClientData.storeData.storeUrl);
        Hide();
    }

    public void Hide() {
        Destroy(this.gameObject);
    }

    private enum Phase {
        None = 0,
        Expand = 1,
        Wait = 2
    }
}