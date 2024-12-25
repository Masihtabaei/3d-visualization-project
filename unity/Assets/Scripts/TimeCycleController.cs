using System;
using TMPro;
using UnityEngine;

public class TimeCycle : MonoBehaviour
{
    private const int DAY_LENGTH_IN_SECONDS = 60 * 60 * 24;
    private const int YEAR_LENGTH_IN_DAYS = 365;

    private float deltaTimeAccumulator = 0;

    [Header("Time")]

    [SerializeField]
    private int simulatorPlaybackSpeedRatio = 1;
    [SerializeField]
    private int timeCounter = 0;
    [SerializeField]
    private int dayCounter = 0;
    [SerializeField]
    private int yearCounter = 0;


    public void Update()
    {
        deltaTimeAccumulator += Time.deltaTime;
        if (deltaTimeAccumulator - 1 < 0)
            return;

        deltaTimeAccumulator = 0;
        timeCounter += simulatorPlaybackSpeedRatio;
        int mostSignificantBit = ((timeCounter - 1 - DAY_LENGTH_IN_SECONDS) >> 31) & 1;
        timeCounter *= mostSignificantBit;
        dayCounter += (1 - mostSignificantBit);
        mostSignificantBit = ((dayCounter - 1 - YEAR_LENGTH_IN_DAYS) >> 31) & 1;
        dayCounter *= mostSignificantBit;
        yearCounter += (1 - mostSignificantBit);
    }
}
