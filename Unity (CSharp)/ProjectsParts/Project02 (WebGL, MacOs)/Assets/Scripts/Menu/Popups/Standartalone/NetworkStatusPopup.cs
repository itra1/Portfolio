using it.UI;
using System.Collections;
using UnityEngine;
using Sett = it.Settings;
using TMPro;
using it.Main.SinglePages;

namespace it.Popups
{
	public class NetworkStatusPopup : PopupBase
	{
    [SerializeField] private it.UI.Avatar _avatar;
    [SerializeField] private bool checkingPing = true;
    [SerializeField] private int pingCycle = 1;

    [SerializeField] private TextMeshProUGUI _myIpAdressLabel;

    [SerializeField] private PingViewBase amazonStatusText;
    [SerializeField] private PingViewBase baiduStatusText;
    [SerializeField] private PingViewBase microsoftStatusText;
    [SerializeField] private PingViewBase serverStatusText;
    
    [SerializeField] private SignalButton signalController;
    [SerializeField] private CardAnimController cardAnim;
		protected override void Awake()
		{
			base.Awake();
			_myIpAdressLabel.text = "-";

			amazonStatusText.Host = "https://Amazon.co.uk";
			baiduStatusText.Host = "https://baidu.com";
			microsoftStatusText.Host = "https://microsoft.com";
			serverStatusText.Host = ServerManager.Server;

			//SendPingToPingView("Amazon.co.uk", amazonStatusText);
			//  SendPingToPingView("baidu.com", baiduStatusText);
			//  SendPingToPingView("microsoft.com", microsoftStatusText);
			//  SendPingToPingView(Sett.Settings.Servers.Server, serverStatusText);

			//if (signalController == null)
			//{
			//  signalController = FindObjectOfType<SignalButton>()?.GetComponent<SignalButton>();
			//  if (signalController!=null)
			//  signalController.OnPingTick += ChekPings;
			//  ChekPings();
			//}
		}

		// private void Start()
		// {
		// _myIpAdressLabel.text = "-";
		//if (signalController != null)
		//{
		//  signalController.OnPingTick += ChekPings;
		//  ChekPings();
		//}

		// }
		//private void OnDestroy()
		//{
		//  if (signalController != null)
		//    signalController.OnPingTick -= ChekPings;
		//}

		public override void Show(bool force = false)
    {
      _avatar.SetDefaultAvatar();
      if(UserController.IsLogin)
        _avatar.SetAvatar(UserController.User.AvatarUrl);
			base.Show(force);


			_myIpAdressLabel.gameObject.SetActive(true);
      _myIpAdressLabel.text = "-";


#if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
			_myIpAdressLabel.text = UserController.User.nickname;
      amazonStatusText.SetPing(99);
			baiduStatusText.SetPing(99);
			microsoftStatusText.SetPing(99);
			serverStatusText.SetPing(99);
			return;
#endif

			GetMyIp();
      //ChekPings();
      checkingPing = true;
      if(cardAnim != null)
      cardAnim.StartAnimation();
    }

    private void GetMyIp(){
      it.Api.UserApi.GetMyApi((result) =>
      {
        it.Logger.Log(result);
        _myIpAdressLabel.gameObject.SetActive(true);
        _myIpAdressLabel.text = result;
      },
      (error) =>
      {
        _myIpAdressLabel.gameObject.SetActive(true);
        _myIpAdressLabel.text = "-";
      });
		}

    public override void Hide()
    {      
      base.Hide();
      checkingPing = false;
			if (cardAnim != null)
				cardAnim.StopAnimation();
    }
    //private void ChekPings()
    //{
    //  SendPingToPingView("Amazon.co.uk", amazonStatusText);
    //  SendPingToPingView("baidu.com", baiduStatusText);
    //  SendPingToPingView("microsoft.com", microsoftStatusText);
    //  SendPingToPingView(Sett.Settings.Servers.Server, serverStatusText);
    //}
    //public void RunToolButton()
    //{
    //  ChekPings();
    //}
    //private async void SendPingToPingView(string url, PingViewBase view){
    //  if (signalController == null) return;
    //  view.Await(); 
    //  #if !UNITY_WEBGL
    //  view.SetPing(await signalController.GetPing(url));
    //  #endif
    //}
    protected override void EnableInit()
    {
      base.EnableInit();
    }
    
  }
}