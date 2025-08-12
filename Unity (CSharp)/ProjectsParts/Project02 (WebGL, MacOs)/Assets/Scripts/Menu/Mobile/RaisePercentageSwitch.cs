using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisePercentageSwitch : MonoBehaviour
{

	[SerializeField] private Garilla.Games.GameUIManager thisGameUIManager;

	[SerializeField] private GameObject PercentageRaise;
	[SerializeField] private GameObject KeyboardPanel;
	[SerializeField] private GameObject SliderPanel;

	[SerializeField] private GameObject[] EnabledSprites;
	[SerializeField] private GameObject[] DisabledSprites;

	[SerializeField] private GameObject[] GameWindowButtons;

	public void OnEnable()
	{
		DisableAllPanels();
		for (int a = 0; a < GameWindowButtons.Length; a++)
		{
			GameWindowButtons[a].SetActive(false);
		}
	}

	public void OnDisable()
	{
		for (int a = 0; a < GameWindowButtons.Length; a++)
		{
			GameWindowButtons[a].SetActive(true);
		}
	}


	public void SwitchPanel(int panelInt)
	{


		if (panelInt == 0)
		{
			if (KeyboardPanel.activeSelf)
			{
				DisableAllPanels();
			}
			else
			{
				if (SliderPanel.activeSelf)
				{
					DisableAllPanels();
				}
				KeyboardPanel.SetActive(true);
				SwitchSprites(panelInt);

			}

		}
		if (panelInt == 1)
		{

			if (SliderPanel.activeSelf)
			{
				DisableAllPanels();
			}
			else
			{
				if (KeyboardPanel.activeSelf)
				{
					DisableAllPanels();
				}
				SliderPanel.SetActive(true);
				SwitchSprites(panelInt);
			}
		}
		if (panelInt == 2)
		{
			PercentageRaise.SetActive(true);
		}


	}
	private void SwitchSprites(int panelInt)
	{
		if (EnabledSprites[panelInt] != null && panelInt < EnabledSprites.Length)
			EnabledSprites[panelInt].SetActive(!EnabledSprites[panelInt].activeSelf);
		if (DisabledSprites[panelInt] != null && panelInt < DisabledSprites.Length)
			DisabledSprites[panelInt].SetActive(!DisabledSprites[panelInt].activeSelf);
	}


	public void DisableAllPanels()
	{
		PercentageRaise.SetActive(false);
		KeyboardPanel.SetActive(false);
		SliderPanel.SetActive(false);
		for (int a = 0; a < EnabledSprites.Length; a++)
		{
			if (EnabledSprites[a] != null)
				EnabledSprites[a].SetActive(false);
			if (DisabledSprites[a] != null)
				DisabledSprites[a].SetActive(true);
		}
	}



	public void TouchRaiseButton()
	{
		if (SliderPanel.activeSelf || PercentageRaise.activeSelf)
		{
			thisGameUIManager.ActionsPanel.RaiseButton();
		}
		else
		{
			PercentageRaise.SetActive(!PercentageRaise.activeSelf);
		}
	}

}
