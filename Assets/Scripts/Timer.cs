using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float max_time_in_seconds = 2f;
    public float time = 0f;
    public bool timer_stopped = false;

    public void reset_timer()
    { 
        time = max_time_in_seconds;
        timer_stopped = false;
    }

    public bool tick_n_check(float delta_time)
    {
        if (!timer_stopped)
        {
            time -= delta_time;

            if (time <= 0f)
            {
                timer_stopped = true;
                time = 0f;
                return true;
            }
            return false;
        }
        
        return true;
    }

    // percentage is in decimal format
    // ie 1.0f is 100%; 0.0f is 0%, 0.5f is 50%, etc. 
    public float getTimerCompletionPercentage()
    {
        if (isTimerDone())
            return 1.0f;//as 100%

        return (max_time_in_seconds - time) / max_time_in_seconds;
    }

    public bool isTimerDone()
    {
        return timer_stopped;
    }

    public void forceTimerStop()
    {
        timer_stopped = true;
        time = 0;
    }
}
