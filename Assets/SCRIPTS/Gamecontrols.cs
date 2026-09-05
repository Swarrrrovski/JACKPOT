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
    [Header("Audio")]

[SerializeField]
private AudioSource reelSpinAudio;

[SerializeField]
private AudioSource resultAudio;
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

    // No bet has been selected yet.
    selectedBet = 0;

    // Show the betting popup immediately when the game starts.
    if (betPopup != null)
    {
        betPopup.SetActive(true);
    }

    if (resultText != null)
    {
        resultText.text = "";
    }

    if (selectedBetText != null)
    {
        selectedBetText.text = "BET: 0G";
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
 private IEnumerator PlaySlotMachine()
{
    // Prevent another spin from being started while
    // the current game round is running.
    isSpinning = true;


    // ---------------------------------------------------------
    // TAKE THE BET
    // ---------------------------------------------------------

    coinBalance -= selectedBet;

    UpdateCoinUI();


    // Clear the previous result.
    if (resultText != null)
    {
        resultText.text = "";
    }


    // ---------------------------------------------------------
    // PULL THE HANDLE
    // ---------------------------------------------------------

    if (handleAnimator != null)
    {
        handleAnimator.SetTrigger(pullTriggerName);
    }


    // Give the handle animation time to play.
    yield return new WaitForSeconds(0.35f);


    // ---------------------------------------------------------
    // START REEL AUDIO
    // ---------------------------------------------------------

    // The spinning sound starts immediately before
    // the first reel begins moving.
    if (reelSpinAudio != null)
    {
        reelSpinAudio.Play();
    }


    // ---------------------------------------------------------
    // REEL 1
    // ---------------------------------------------------------

    yield return StartCoroutine(
        rows[0].Spin()
    );


    // Small delay between reel 1 and reel 2.
    yield return new WaitForSeconds(0.25f);


    // ---------------------------------------------------------
    // REEL 2
    // ---------------------------------------------------------

    yield return StartCoroutine(
        rows[1].Spin()
    );


    // Small delay between reel 2 and reel 3.
    yield return new WaitForSeconds(0.25f);


    // ---------------------------------------------------------
    // REEL 3
    // ---------------------------------------------------------

    yield return StartCoroutine(
        rows[2].Spin()
    );


    // ---------------------------------------------------------
    // STOP REEL AUDIO
    // ---------------------------------------------------------

    // All three reels have now stopped.
    if (reelSpinAudio != null)
    {
        reelSpinAudio.Stop();
    }


    // ---------------------------------------------------------
    // CHECK RESULT
    // ---------------------------------------------------------

 bool playerWon = CheckResults();


// Only play the result sound when the player wins.
if (playerWon && resultAudio != null)
{
    resultAudio.Play();
}


    // ---------------------------------------------------------
    // ROUND COMPLETE
    // ---------------------------------------------------------

    isSpinning = false;


    // The old bet is finished.
    // The player must select a new bet.
    selectedBet = 0;


    // ---------------------------------------------------------
    // OPEN BET POPUP FOR NEXT ROUND
    // ---------------------------------------------------------

    if (betPopup != null)
    {
        betPopup.SetActive(true);
    }
}
private bool CheckResults()
{
    // Get the final symbol from each reel.
    string row1Result = rows[0].stoppedSlot;
    string row2Result = rows[1].stoppedSlot;
    string row3Result = rows[2].stoppedSlot;


    // Display the three final symbols in the Console.
    Debug.Log(
        "RESULT: " +
        row1Result + " | " +
        row2Result + " | " +
        row3Result
    );


    // ---------------------------------------------------------
    // WIN CONDITION
    // ---------------------------------------------------------
    // The player wins only when all three reels show
    // exactly the same symbol.
    //
    // Example:
    //
    // BAR | BAR | BAR       → WIN
    // BELL | BELL | BELL    → WIN
    // CHERRY | CHERRY | CHERRY → WIN
    //
    // BAR | BELL | BAR      → LOSS
    // ---------------------------------------------------------

    if (row1Result == row2Result &&
        row2Result == row3Result)
    {
        HandleWin(row1Result);

        // Tell PlaySlotMachine() that the player won.
        return true;
    }


    // ---------------------------------------------------------
    // LOSS
    // ---------------------------------------------------------

    HandleLoss();

    // Tell PlaySlotMachine() that the player lost.
    return false;
}
private void HandleWin(string symbol)
{
    int multiplier = GetMultiplier(symbol);

    int winnings =
        selectedBet * multiplier;


    coinBalance += winnings;


    if (resultText != null)
    {
        resultText.text =
            "WIN! " +
            symbol +
            " +" +
            winnings +
            "G";
    }


    UpdateCoinUI();
}
private int GetMultiplier(string symbol)
{
    switch (symbol)
    {
        case "Bar":
            return 2;

        case "Bell":
            return 5;

        case "Cherry":
            return 10;

        case "Seven":
            return 20;

        default:
            return 0;
    }
}
private void HandleLoss()
{
    if (resultText != null)
    {
        resultText.text = "NO WIN";
    }

    UpdateCoinUI();
}
private void UpdateCoinUI()
{
    if (coinText != null)
    {
        coinText.text =
            "COINS (G): " +
            coinBalance;
    }
}
}