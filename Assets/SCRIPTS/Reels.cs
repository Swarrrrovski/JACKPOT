using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reels : MonoBehaviour
{[SerializeField]
    private Transform[] symbols;
 // The names of the five symbols.
    // The final BAR is a duplicate used for looping.
    [SerializeField]
    private string[] symbolNames =
    {
        "Bar",
        "Bell",
        "Cherry",
        "Seven",
        "Bar"
    };


    // Exact Y positions from your Unity scene.
    [SerializeField]
    private float[] startingYPositions =
    {
        3.25f,
        1.833842f,
        0.5110874f,
        -0.926158f,
        -2.366158f
    };


    // The center position of the visible result line.
    [SerializeField]
    private float resultLineY = 0.5110874f;


    // Bottom position of the reel.
    [SerializeField]
    private float bottomY = -2.366158f;


    // Distance between the first BAR and the duplicate BAR.
    private float reelCycleDistance;


    // True when this reel has completed its spin.
    public bool rowStopped { get; private set; } = true;


    // Symbol currently occupying the result line.
    public string stoppedSlot { get; private set; } = "";


    // Stores the original local Y positions.
    private float[] originalYPositions;
}
