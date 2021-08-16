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
        percentage_per_bar = 1f / (float)bars.Count;
    }

    public void updateProgress(float delta_time)
    {
        bool isDone = progress_timer.tick_n_check(delta_time);

        //progress should be going down
        progress = progress_timer.getTimerCompletionPercentage();// ranges from 0.0 to 1.0 as 100%

        int bar_index = (int)Mathf.Ceil(progress / percentage_per_bar) - 1;

        // only multipling by 1f to show that you need to multiply by the max alpha range. 
        // For Color, alpha range is 0.0 to 1.0
        // For Color32, alpha range is 0 to 255;

        //localizes the progress of a given bar to adjust the alpha level of that bar
        float alpha_at_index = ((progress-(bar_index*percentage_per_bar))/ percentage_per_bar) * 1f;

        Color old_color = bars[bar_index].color;

        bars[bar_index].color = new Color( old_color.r, old_color.g, old_color.b, alpha_at_index);
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
