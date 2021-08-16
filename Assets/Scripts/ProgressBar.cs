using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public Timer progress_timer;
    public List<SpriteRenderer> bars = new List<SpriteRenderer>();// first element represents closer to 0%. last element represents closer to 100%
    public Color bar_color = Color.white;

    [SerializeField]
    float percentage_per_bar = 0f;//also called PPB; calculated by doing 100% devided by the total number of bars 

    public float progress = 0f; //from 0 to 100%

    public void calculateAndSetPPB()
    {
        percentage_per_bar = 100f / (float)bars.Count;
    }

    public void resetProgress()
    {
        foreach (SpriteRenderer bar in bars)
        {
            bar.color = bar_color;
        }
        progress_timer.reset_timer();
    }

    public void setProgessMaxTime(float max_time)
    {
        progress_timer.max_time_in_seconds = max_time;
    }

}
