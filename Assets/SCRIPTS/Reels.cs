using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reels : MonoBehaviour
{[SerializeField]
private int selectedResultIndex;
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
 private void Start()
{
    // The distance from the top BAR to the duplicate bottom BAR
    // defines one complete reel cycle.
    reelCycleDistance =
        startingYPositions[0] - startingYPositions[4];
   // Store the original positions so that the reel can calculate its position during every spin.
    originalYPositions = new float[startingYPositions.Length];
  for (int i = 0; i < startingYPositions.Length; i++)
    {
        originalYPositions[i] = startingYPositions[i];
    }
// Make sure the reel starts in a stopped state.
    rowStopped = true;
// The initial center symbol is CHERRY.
    stoppedSlot = "Cherry";
}
public IEnumerator Spin()
{
    rowStopped = false;
    stoppedSlot = "";


    // There are four real outcomes:
    //
    // 0 = Bar
    // 1 = Bell
    // 2 = Cherry
    // 3 = Seven
    //
    // The fifth BAR is only a duplicate used for looping.
    selectedResultIndex = Random.Range(0, 4);


    // Choose how many complete rotations happen
    // before the reel begins stopping.
    int randomCycles = Random.Range(7, 11);


    // Calculate the distance required to place the
    // selected symbol exactly on the center result line.
  float distanceToResult =
    Mathf.Repeat(
        startingYPositions[selectedResultIndex] - resultLineY,
        reelCycleDistance
    );


    float totalDistance =
        distanceToResult +
        (randomCycles * reelCycleDistance);


    yield return StartCoroutine(
        SpinReel(totalDistance)
    );
}
private IEnumerator SpinReel(float totalDistance)
{
    float distanceTravelled = 0f;

    // How long the entire reel spin should take.
    float spinDuration = 2.0f;

    float elapsedTime = 0f;


    while (elapsedTime < spinDuration)
    {
        elapsedTime += Time.deltaTime;


        // Convert elapsed time into a 0-1 value.
        float progress =
            Mathf.Clamp01(elapsedTime / spinDuration);


        // SmoothStep gives us acceleration at the beginning
        // and deceleration near the end.
        float smoothProgress =
            Mathf.SmoothStep(0f, 1f, progress);


        distanceTravelled =
            Mathf.Lerp(
                0f,
                totalDistance,
                smoothProgress
            );


        MoveSymbols(distanceTravelled);


        yield return null;
    }


    // Make absolutely sure we finish at the exact
    // calculated distance rather than a frame-dependent value.
    MoveSymbols(totalDistance);


    SetFinalPositions(totalDistance);


    rowStopped = true;
}
private void MoveSymbols(float distanceTravelled)
{
    float cycleOffset =
        distanceTravelled % reelCycleDistance;


    for (int i = 0; i < symbols.Length; i++)
    {
        float newY =
            originalYPositions[i] - cycleOffset;


        // If the symbol has moved below the reel,
        // wrap it back to the top.
        while (newY < bottomY)
        {
            newY += reelCycleDistance;
        }


        Vector3 position =
            symbols[i].localPosition;

        position.y = newY;

        symbols[i].localPosition = position;
    }
}
private void SetFinalPositions(float totalDistance)
{
    // Determine which of the four real symbols was selected.
    int resultIndex = GetResultIndex(totalDistance);


    // Calculate the exact distance needed for that
    // symbol to occupy the result line.
    float resultDistance =
        Mathf.Repeat(
            startingYPositions[resultIndex] - resultLineY,
            reelCycleDistance
        );


    for (int i = 0; i < symbols.Length; i++)
    {
        float newY =
            startingYPositions[i] - resultDistance;


        while (newY < bottomY)
        {
            newY += reelCycleDistance;
        }


        Vector3 position =
            symbols[i].localPosition;

        position.y = newY;

        symbols[i].localPosition = position;
    }


    stoppedSlot =
        symbolNames[resultIndex];
}
private int GetResultIndex(float totalDistance)
{
    return selectedResultIndex;
}
}
