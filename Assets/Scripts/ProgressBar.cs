using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public Timer progress_timer;
    public List<SpriteRenderer> bars = new List<SpriteRenderer>();// first element represents closer to 0%. last element represents closer to 100%
    public Color bar_color = Color.white;
    public List<Color> bar_colors = new List<Color>();
    int bars_per_color = 1; // also called BPC; used to determine

    [SerializeField]
    float percentage_per_bar = 0f;//also called PPB; calculated by doing 100% devided by the total number of bars 

    public float progress = 0f; //from 0 to 100%

    public void Start()
    {
        if (bar_colors.Count > 0 && bars.Count > 0)
        {
            calculateAndSetPBC();
            calculateAndSetPPB();

            for (int index = 0; index < bars.Count; index++)
            {
                bars[index].color = bar_colors[index / bars_per_color];
            }

        }
    }

    //DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && bar_colors.Count>0)
        {
            //want to show 100%; with mod 1f it would not work
            if (progress >= 1f)
                progress = 0f;

            progress += 0.05f;
            updateProgressByValue(progress);
        }
    }

    // bars_per_color = PBC
    public void calculateAndSetPBC()
    {
        bars_per_color = bars.Count / bar_colors.Count; // ex: 16 bars with 4 colors. 16 / 4 = 4 bars with one color
    }

    //percentage_per_bar = PPB
    public void calculateAndSetPPB()
    {
        percentage_per_bar = 1f / (float)bars.Count;
    }

    public void updateProgressByValue(float value)// progress goes left to right: 0% -> 100%
    {
        progress = value; // 0.0 to 1.0 as 100%

        // For right now, I don't want to show that the any overpercentage shows a different color
        if (value > 1f)
            value = 1f;

        // this returns at which the bar index should focus on the varying alpha to show current progress level
        // any index below this index, should be full alpha (1f or 255)
        // any index greater than this index, should be zero alpha (0f or 0)
        int alpha_changed_index = (int)(value / percentage_per_bar);

        for (int bar_index = 0; bar_index < bars.Count; bar_index++)
        {
            if (bar_index < alpha_changed_index)
            {
                //set the previous bar's alpha to zero, so doesn't appear. Again using range 0-1f. Not 0-255.
                Color current_bar_color = bars[bar_index].color;
                bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, 1f);
            }
            else if (bar_index == alpha_changed_index)
            {
                //localizes the progress of a given bar to adjust the alpha level of that bar
                float alpha_at_index = ((progress - (bar_index * percentage_per_bar)) / percentage_per_bar) * 1f;
                Color current_bar_color = bars[bar_index].color;
                bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, alpha_at_index);
            }
            else // bar index > alalpha_changed_index
            {
                //set the previous bar's alpha to zero, so doesn't appear. Again using range 0-1f. Not 0-255.
                Color current_bar_color = bars[bar_index].color;
                bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, 0f);
            }
        }
    }

    public void updateProgress(float delta_time)//progress goes right to left by fading: 0% <- 100%
    {
        bool isDone = progress_timer.tick_n_check(delta_time);

        //progress should be going down
        progress = 1f - progress_timer.getTimerCompletionPercentage();// ranges from 0.0 to 1.0 as 100%

        int bar_index = 0;

        if (!isDone)
        { 
            bar_index = (int)Mathf.Ceil(progress / percentage_per_bar) - 1;
        }

        // only multipling by 1f to show that you need to multiply by the max alpha range. 
        // For Color, alpha range is 0.0 to 1.0
        // For Color32, alpha range is 0 to 255;

        //localizes the progress of a given bar to adjust the alpha level of that bar
        float alpha_at_index = ((progress-(bar_index*percentage_per_bar))/ percentage_per_bar) * 1f;

        if (bar_index + 1 < bars.Count)
        {
            //set the previous bar's alpha to zero, so doesn't appear. Again using range 0-1f. Not 0-255.
            Color previous_bar_old_color = bars[bar_index+1].color;
            bars[bar_index+1].color = new Color(previous_bar_old_color.r, previous_bar_old_color.g, previous_bar_old_color.b, 0f);
        }

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
