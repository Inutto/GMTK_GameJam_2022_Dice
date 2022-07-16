using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
    
public class TimerF
{
    float maxTime;
    float remainingTime;

    public bool isTicking { get; private set; }

    public bool isFinished { get { return remainingTime < 0; } }

    public TimerF(float time)
    {
        maxTime = time;
        remainingTime = time;
    }

    public void Begin()
    {
        isTicking = true;
    }

    public void Pause()
    {
        isTicking = false;
    }

    public void ResetAndPause()
    {
        remainingTime = maxTime;
        isTicking = false;
    }

    public void ResetAndBegin()
    {
        remainingTime = maxTime;
        isTicking = true;
    }

    public void ChangeMaxTime(float newmax)
    {
        maxTime = newmax;
        remainingTime = newmax;
        isTicking = false;
    }

    // Start is called before the first frame update
    public void FixedTick()
    {
        if (!isTicking) return;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isTicking = false;
        }

        remainingTime -= Time.fixedDeltaTime;
    }

    public void Tick()
    {
        if (!isTicking) return;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isTicking = false;
        }

        remainingTime -= Time.deltaTime;
    }


    public static IEnumerator DoThisAfterSeconds(float seconds, Action things)
    {
        yield return new WaitForSeconds(seconds);
        things?.Invoke();
    }
}



