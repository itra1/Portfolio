using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class ActionController : AutoCreateInstance<ActionController>
{

	public void Emit(string actionKey, List<string> options = null)
	{
		it.Logger.Log("Action Controller emit " + actionKey);
		if (actionKey == "promotions")
		{
			PromotionController.Instance.OpenPronotionsPanel();
			return;
		}

		if (actionKey == "badBeat")
		{
#if UNITY_STANDALONE
			if (AppConfig.TableExe != null)
			{
				PlayerPrefs.SetString(StringConstants.BUTTON_BADBEAT, "");
				StandaloneController.Instance.FocusMain();
			}
#endif
			it.Main.SinglePageController.Instance.Show(SinglePagesType.BadBeat);
			return;
		}

		// Соытие обновления таймбанка
		if (actionKey == "timeBankUpdate")
		{
#if UNITY_STANDALONE
			if (AppConfig.TableExe != null)
			{
				PlayerPrefs.SetString(StringConstants.TIMEBANK_UPDATE, "");
				return;
			}
#endif
			UserController.Instance.UpdateTaimbank();
			return;
		}

	}

}