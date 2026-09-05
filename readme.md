# 🎰 Unity Slot Machine

A playable 3-reel slot machine game developed in Unity as part of the Unity Slot Game Assignment.

The project focuses on implementing a functional slot machine with randomized reel outcomes, smooth reel animations, sequential reel stopping, betting mechanics, win/loss detection, payouts, audio feedback, and a WebGL build.

---

## 🎮 Game Overview

The game features a classic 3-reel slot machine.

The player begins with a fixed number of coins and must select a bet before each spin.

Available bets:

- 10G
- 50G
- 100G

After selecting a bet, the player presses the EXIT button to close the betting popup and then pulls the slot-machine handle.

The machine then:

1. Deducts the selected bet from the player's balance.
2. Plays the lever-pull animation.
3. Starts the reel spinning animation.
4. Stops Reel 1.
5. Stops Reel 2.
6. Stops Reel 3.
7. Checks the resulting combination.
8. Calculates the payout if the player wins.
9. Displays the result.
10. Opens the betting popup again for the next round.

---

## 🕹️ Controls

### Mouse

- Click a betting button to select a wager.
- Click EXIT to confirm the selected bet and close the betting popup.
- Click the slot-machine handle to start the spin.

### Touch

The handle and UI are designed to support touch interaction where supported by the WebGL/mobile environment.

---

## 💰 Betting System

The player can choose between three betting amounts:

| Bet | Cost |
|-----|------|
| 10G | 10 coins |
| 50G | 50 coins |
| 100G | 100 coins |

A bet cannot be placed if the player does not have enough coins.

The player must select a bet before pulling the handle.

After a spin is completed, the betting popup automatically becomes available again for the next round.

---

## 🎰 Winning Logic

The player wins when all three reels display the same symbol.

For example:

```text
BAR    | BAR    | BAR
BELL   | BELL   | BELL
CHERRY | CHERRY | CHERRY
SEVEN  | SEVEN  | SEVEN