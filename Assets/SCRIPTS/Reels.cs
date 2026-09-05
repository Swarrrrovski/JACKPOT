using System.Collections;
using UnityEngine;

public class Reels : MonoBehaviour
{
    // =========================================================
    // SYMBOL REFERENCES
    // =========================================================

    // Five physical symbols belonging to this reel.
    //
    // Element 0 = TOP BAR
    // Element 1 = BELL
    // Element 2 = CHERRY
    // Element 3 = SEVEN
    // Element 4 = BOTTOM DUPLICATE BAR
    //
    [SerializeField]
    private Transform[] symbols;


    // =========================================================
    // SYMBOL NAMES
    // =========================================================

    // These names correspond to the five symbol objects.
    [SerializeField]
    private string[] symbolNames =
    {
        "BAR",
        "BELL",
        "CHERRY",
        "SEVEN",
        "BAR"
    };


    // =========================================================
    // SYMBOL POSITIONS
    // =========================================================

    // These are the actual local Y positions of the five
    // positions in the reel.
    //
    // TOP → BOTTOM
    //
    [SerializeField]
    private float[] startingYPositions =
    {
        3.25f,
        1.833842f,
        0.5110874f,
        -0.926158f,
        -2.366158f
    };


    // =========================================================
    // RESULT POSITION
    // =========================================================

    // The selected symbol must stop at this position.
    //
    // In your machine this is the TOP position.
    [SerializeField]
    private float resultLineY = 3.25f;


    // Lowest point of the reel.
    [SerializeField]
    private float bottomY = -2.366158f;


    // Distance from the top position to the bottom
    // duplicate position.
    private float reelCycleDistance;


    // =========================================================
    // REEL STATE
    // =========================================================

    // True when the reel is not spinning.
    public bool rowStopped { get; private set; } = true;


    // Symbol currently occupying the result position.
    public string stoppedSlot { get; private set; }


    // Randomly selected symbol index.
    private int selectedResultIndex;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // -----------------------------------------------------
        // VALIDATE SYMBOL ARRAY
        // -----------------------------------------------------

        if (symbols == null || symbols.Length != 5)
        {
            Debug.LogError(
                gameObject.name +
                " must have exactly 5 symbols assigned."
            );

            return;
        }


        // Make sure every element has a Transform.
        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] == null)
            {
                Debug.LogError(
                    gameObject.name +
                    " has a missing symbol at Element " +
                    i
                );

                return;
            }
        }


        // -----------------------------------------------------
        // CALCULATE REEL CYCLE
        // -----------------------------------------------------

        reelCycleDistance =
            startingYPositions[0]
            - startingYPositions[4];


        // -----------------------------------------------------
        // INITIAL RESULT
        // -----------------------------------------------------

        rowStopped = true;

        stoppedSlot = symbolNames[0];
    }


    // =========================================================
    // PUBLIC SPIN FUNCTION
    // =========================================================

    public IEnumerator Spin()
    {
        rowStopped = false;

        stoppedSlot = "";


        // -----------------------------------------------------
        // SELECT RANDOM RESULT
        // -----------------------------------------------------

        // 0 = BAR
        // 1 = BELL
        // 2 = CHERRY
        // 3 = SEVEN
        //
        // Element 4 is the duplicate BAR and is not randomly
        // selected because it exists only to make the reel loop.
        //
        selectedResultIndex =
            Random.Range(0, 4);


        Debug.Log(
            "[" + gameObject.name + "] " +
            "RNG selected: " +
            symbolNames[selectedResultIndex]
        );


        // -----------------------------------------------------
        // NUMBER OF FULL SPINS
        // -----------------------------------------------------

        int randomCycles =
            Random.Range(7, 11);


        // -----------------------------------------------------
        // CALCULATE DISTANCE
        // -----------------------------------------------------

        // Calculate the distance required for the selected
        // symbol to reach the TOP position.
        //
        // We use modulo so that the reel can make several
        // complete rotations first.
        //
        float distanceToResult =
            Mathf.Repeat(
                startingYPositions[selectedResultIndex]
                - resultLineY,
                reelCycleDistance
            );


        float totalDistance =
            randomCycles * reelCycleDistance
            + distanceToResult;


        Debug.Log(
            "[" + gameObject.name + "] " +
            "Spin distance: " +
            totalDistance
        );


        // -----------------------------------------------------
        // START ANIMATION
        // -----------------------------------------------------

        yield return StartCoroutine(
            SpinReel(totalDistance)
        );
    }


    // =========================================================
    // SPIN ANIMATION
    // =========================================================

    private IEnumerator SpinReel(float totalDistance)
    {
        float spinDuration = 2f;

        float elapsed = 0f;


        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;


            // Convert elapsed time into 0 → 1.
            float progress =
                Mathf.Clamp01(
                    elapsed / spinDuration
                );


            // Smooth acceleration and deceleration.
            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );


            // Calculate current distance.
            float distanceTravelled =
                Mathf.Lerp(
                    0f,
                    totalDistance,
                    smoothProgress
                );


            // Move the symbols.
            MoveSymbols(distanceTravelled);


            yield return null;
        }


        // -----------------------------------------------------
        // FINAL POSITION
        // -----------------------------------------------------

        // Don't rely on the animation's final floating-point
        // position.
        //
        // Explicitly arrange the symbols.
        SetFinalPositions();


        rowStopped = true;


        Debug.Log(
            "[" + gameObject.name + "] " +
            "STOPPED ON: " +
            stoppedSlot
        );
    }


    // =========================================================
    // MOVE SYMBOLS DURING SPIN
    // =========================================================

    private void MoveSymbols(float distanceTravelled)
    {
        float cycleOffset =
            Mathf.Repeat(
                distanceTravelled,
                reelCycleDistance
            );


        for (int i = 0; i < symbols.Length; i++)
        {
            // Move the symbol down.
            float newY =
                startingYPositions[i]
                - cycleOffset;


            // If the symbol goes below the bottom,
            // bring it back to the top.
            while (newY < bottomY)
            {
                newY += reelCycleDistance;
            }


            Vector3 position =
                symbols[i].localPosition;


            position.y = newY;


            symbols[i].localPosition =
                position;
        }
    }


    // =========================================================
    // FINAL STOPPING ARRANGEMENT
    // =========================================================

    private void SetFinalPositions()
    {
        // -----------------------------------------------------
        // POSITION EACH SYMBOL IN THE CORRECT SLOT
        // -----------------------------------------------------

        for (int i = 0; i < symbols.Length; i++)
        {
            // Calculate which symbol should occupy this
            // physical position.
            //
            // Example:
            //
            // selectedResultIndex = 2
            //
            // Then:
            //
            // position 0 → CHERRY
            // position 1 → SEVEN
            // position 2 → BAR
            // position 3 → BAR
            // position 4 → BELL
            //
            int symbolIndex =
                (selectedResultIndex + i)
                % symbols.Length;


            Vector3 position =
                symbols[symbolIndex].localPosition;


            // Put this symbol into the corresponding
            // physical reel position.
            position.y =
                startingYPositions[i];


            symbols[symbolIndex].localPosition =
                position;
        }


        // -----------------------------------------------------
        // SAVE THE RESULT
        // -----------------------------------------------------

        stoppedSlot =
            symbolNames[selectedResultIndex];


        // -----------------------------------------------------
        // DEBUG VERIFICATION
        // -----------------------------------------------------

        Debug.Log(
            "[" + gameObject.name + "] " +
            "FINAL SYMBOL = " +
            stoppedSlot +
            " at Y = " +
            symbols[selectedResultIndex].localPosition.y
        );
    }
}