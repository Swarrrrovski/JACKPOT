using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Main controller for the slot machine.
///
/// GameControl manages:
/// - Betting
/// - The bet popup
/// - The slot-machine handle
/// - Sequential reel spinning
/// - Winning combination detection
/// - Payout calculation
public class GameControl : MonoBehaviour
{
    [Header("Reels")]

    // Three reels in left-to-right order.
    [SerializeField]
    private Reels[] rows;


    [Header("Handle")]

    // Animator attached to the slot handle.
    [SerializeField]
    private Animator handleAnimator;


    // Name of the Animator Trigger used to pull the lever.
    [SerializeField]
    private string pullTriggerName = "Pull";


    [Header("Bet Popup")]

    // Your Bet 10G / 50G / 100G popup.
    [SerializeField]
    private GameObject betPopup;


    [Header("UI")]

    [SerializeField]
    private TMP_Text resultText;


    [SerializeField]
    private TMP_Text coinText;


    [SerializeField]
    private TMP_Text selectedBetText;


    [Header("Player Coins")]

    [SerializeField]
    private int startingCoins = 1000;


    private int coinBalance;

    private int selectedBet = 0;

    private bool isSpinning = false;
    private void Start()
{
    coinBalance = startingCoins;

    UpdateCoinUI();

    // The player must select a bet before pulling
    // the slot-machine handle.
    selectedBet = 0;

    if (resultText != null)
    {
        resultText.text = "";
    }
}
public void SelectBet10()
{
    SelectBet(10);
}


public void SelectBet50()
{
    SelectBet(50);
}


public void SelectBet100()
{
    SelectBet(100);
}
public void SelectBet(int amount)
{
    if (isSpinning)
    {
        return;
    }


    if (amount > coinBalance)
    {
        resultText.text = "Not enough coins!";

        return;
    }


    selectedBet = amount;


    if (selectedBetText != null)
    {
        selectedBetText.text =
            "BET: " + selectedBet + "G";
    }
}
/// <summary>
/// Closes the bet popup.
///
/// The player must press EXIT after selecting
/// their desired bet before the handle can be pulled.
/// </summary>
public void CloseBetPopup()
{
    if (selectedBet <= 0)
    {
        return;
    }


    if (betPopup != null)
    {
        betPopup.SetActive(false);
    }
}
private void OnMouseDown()
{
    TryPullHandle();
}
/// <summary>
/// Checks whether the player is allowed to pull the handle.
/// </summary>
private void TryPullHandle()
{
    // Do nothing while another spin is running.
    if (isSpinning)
    {
        return;
    }


    // Player must select a bet first.
    if (selectedBet <= 0)
    {
        if (resultText != null)
        {
            resultText.text =
                "Select a bet first!";
        }

        return;
    }


    // The popup must be closed.
    if (betPopup != null &&
        betPopup.activeSelf)
    {
        return;
    }


    // Make sure the player can afford the bet.
    if (selectedBet > coinBalance)
    {
        if (resultText != null)
        {
            resultText.text =
                "Not enough coins!";
        }

        return;
    }


    // Start the actual slot-machine sequence.
    StartCoroutine(PlaySlotMachine());
}
}