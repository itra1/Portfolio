using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyWalletScreenSwitch : MonoBehaviour
{
    [SerializeField] private Button ClosestButton;
    [SerializeField] private GameObject ClosestButtonEnabledLabel;
    [SerializeField] private GameObject ClosestButtonDisabledLabel;

    [SerializeField] private Button TicketsButton;
    [SerializeField] private GameObject TicketsButtonEnabledLabel;
    [SerializeField] private GameObject TicketsButtonDisabledLabel;

    [SerializeField] private GameObject ClosestPanel;
    [SerializeField] private GameObject TicketsPanel;

    private void OnEnable()
    {
        SwitchToClosest();
    }

    public void SwitchToClosest()
    {
        ClosestButton.interactable = false;

        ClosestButtonEnabledLabel.SetActive(true);
        ClosestButtonDisabledLabel.SetActive(false);

        TicketsButton.interactable = true;

        TicketsButtonEnabledLabel.SetActive(false);
        TicketsButtonDisabledLabel.SetActive(true);

        ClosestPanel.SetActive(true);
        TicketsPanel.SetActive(false);
    }

    public void SwitchToTickets()
    {
        ClosestButton.interactable = true;

        ClosestButtonEnabledLabel.SetActive(false);
        ClosestButtonDisabledLabel.SetActive(true);

        TicketsButton.interactable = false;

        TicketsButtonEnabledLabel.SetActive(true);
        TicketsButtonDisabledLabel.SetActive(false);

        ClosestPanel.SetActive(false);
        TicketsPanel.SetActive(true);
    }
}
