using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public Timer progress_timer;
    public List<SpriteRenderer> bars = new List<SpriteRenderer>();// first element represents closer to 0%. last element represents closer to 100%
    public Color bar_color = Color.white;
    public bool left_to_right_progression = false; // false LTR; true RTL
    public List<Color> bar_colors = new List<Color>();
    int bars_per_color = 1; // also called BPC; used to determine

    [SerializeField]
    float percentage_per_bar = 0f;//also called PPB; calculated by doing 100% devided by the total number of bars 

    [Range(0f,1f)]
    public float progress = 0f; //from 0 to 100%  == 0.0f to 1.0f

    //This NEEDS to be Awake(), because the Start() in Stats runs before ProgressBar's Starts in WebGL build 
    // and crashes at start.
    public void Awake()
    {
        if (bar_colors.Count > 0 && bars.Count > 0)
        {
            //Debug.Log($"Inside Progress Bar's Awake(): {this.name}");
            calculateAndSetPBC();
            calculateAndSetPPB();

            for (int index = 0; index < bars.Count; index++)
            {
                bars[index].color = bar_colors[index / bars_per_color];
            }
            updateProgressByValue(progress);

            /*
            //DEBUG
            if (progress_timer)
            { progress_timer.reset_timer(); }*/
            //Debug.Log($"Exitting Progress Bar's Awake(): {this.name}");
        }
    }

    //DEBUG
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && bar_colors.Count>0)
        {
            //want to show 100%; with mod 1f it would not work
            if (progress >= 1f)
                progress = 0f;

            progress += 0.05f;
            updateProgressByValue(progress);

            if (progress_timer)
            { progress_timer.reset_timer(); }
        }

        if (left_to_right_progression)
        {
            updateProgressByValueAndDeltaTime(Time.deltaTime);
        }
    }*/

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

    //uses the progress value itself
    public void updateProgress()
    {
        //Debug.Log($"Inside updateprogress(): {this.name}");
        updateProgressByValue(progress);
    }

    public void updateProgressByValueAndDeltaTime(float delta_time)//progress goes right to left by fading: 0% <- 100%
    {
        if (progress_timer)
        {
            bool isDone = progress_timer.tick_n_check(delta_time);

            /*//Debugging
            if (isDone)
                progress_timer.reset_timer();*/

            //progress should be going down
            progress = 1f - progress_timer.getTimerCompletionPercentage();// ranges from 0.0 to 1.0 as 100%

            updateProgressByValue(progress);
        }
    }

    //handles both LTR and RTL progression based on value
    public void updateProgressByValue(float value)// progress goes left to right: 0% -> 100%
    {
        //Debug.Log($"Inside updateProgressByValue(): {this.name} value: {value} value type: {value.GetType()}");
        progress = value; // 0.0 to 1.0 as 100%

        // For right now, I don't want to show that the any overpercentage shows a different color
        if (value > 1f)
            value = 1f;
        else if (value < 0f)
            value = 0f;

        // this returns at which the bar index should focus on the varying alpha to show current progress level
        // any index below this index, should be full alpha (1f or 255)
        // any index greater than this index, should be zero alpha (0f or 0)
        int alpha_changed_index = (int)(value / percentage_per_bar);

        if (!left_to_right_progression)
        { alpha_changed_index = (bars.Count -1) - alpha_changed_index; }

        //print($"alpha index: {alpha_changed_index}");

        // FOR LEFT TO RIGHT PROGRESSION (0% is on left side, 100% on right side)
        int starting_index = 0;
        int stopping_index = bars.Count;
        int index_increament_value = 1;//positive

        if (!left_to_right_progression)// RTL progression
        {
            starting_index = bars.Count-1;
            stopping_index = 0;
            index_increament_value = -1;//make negative
        }

        for (int bar_index = starting_index; ; bar_index+= index_increament_value)
        {
            if (left_to_right_progression)
            {
                if (bar_index >= stopping_index)
                    break;
            }
            else {// RTL progression
                if (bar_index < 0)
                    break;
            }

            if (bar_index < alpha_changed_index)
            {
                //bars[bar_index].color = setBarAlpha(bars[bar_index].color, 1f); //full alpha

                if (left_to_right_progression)
                {
                    //set the previous bar's alpha to zero, so doesn't appear. Again using range 0-1f. Not 0-255.
                    //Color current_bar_color = bars[bar_index].color;
                    //bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, 1f);
                    bars[bar_index].color = setBarAlpha(bars[bar_index].color, 1f); //full alpha
                }
                else // RTL 
                {
                    bars[bar_index].color = setBarAlpha(bars[bar_index].color, 0f);//no alpha
                }
            }
            else if (bar_index == alpha_changed_index)
            {
                //localizes the progress of a given bar to adjust the alpha level of that bar
                /*
                 Let's say progress is 10% or 0.10
                 Percentage Per Bar/index (PPB) is 1/16 = 0.0625

                 %       0           0.0625     0.125
                 Aindex: 0           1          2
                 progress:                  ^ 0.10
                 graph:  ####################   
                ========================================
                local %:             0          1
                local graph:         0#######   1
                local % normally:    0          0.0625

                Note: Aindex is actually (starting_index - current_index); this resolves both LTR and RTL local percentage

                0.10 is somewhere between index 1 (0.0625) and 2 (0.0625*2 = 0.125)
                So we set index 2 as 100% and index 1 as 0% locally/ respective to the progress value.
                
                Now to get percetange its a simple equation: X / Y
                X is the progress (0.10)
                Y is starting value X came from locally aka the highest index X reaches. 
                    Since values are increasing from left to right, this would using index 1 ( 0.0625)
                    Kind of sounds backwards, and you would think Y should be index 2, but nope! Trust me.

                
                We need to further localize and make local index 0 have a value of 0, and local index 1 have a value of 1.0 .
                To do this you just subtract the value of local index 0 by itself.  ( 0.0625    - 0.0625 = 0)
                Subtract the progress of the value of local index 0 too!            ( 0.10      - 0.0625 = 0.0375)

                0.0375 is X! Y is simply the local % (100%) but what it would be if it was at index 1 (0.0625%)
                X = 0.0375              Y = 0.0625
                X/Y = 0.60 or 60% locally! 

                If we were using 255 alpha values, the conversion would simply be the max alpha value 255 * the local percentage (0.60)
                255 * 0.60 = 153 (rounded to closes integer; luckily this works perfect)
                But since our alpha values range from 0.0 to 1.0, we can leave 0.60 sicne 1 (max alpha range) * 0.60 is 0.60!

                In short,

                I found that to get the local alpha value of an index based on a progress value is by following this formula:
                                            
                LTR: starting_index is 0
                RTL: starting_index is max index (15 in a list of 16 bars)

                                           [    (progress - ( Abs(starting_index -  bar_index) * percentage_per_bar)    ]
                float alpha_at_index =     [    -----------------------------------------------------------------       ]   *   1f  ;
                                           [                           percentage_per_bar                               ]


                Another way to see this formulat is:
                                           [    {      progress       }                                         ]
                float alpha_at_index =     [    {---------------------}   -  Abs(starting_index -  bar_index)   ]    *   1f  ;
                                           [    {  percentage_per_bar }                                         ]
                 
                 */

                // LTR
                float alpha_at_index = ((progress - (Mathf.Abs(starting_index -  bar_index) * percentage_per_bar)) / percentage_per_bar) * 1f;

                /*if (!left_to_right_progression)
                { alpha_at_index = (( (bar_index * percentage_per_bar) - progress) / percentage_per_bar) * 1f; }*/
                               
                // print($"alpha_at_index: {alpha_at_index}");
                
                /*Color current_bar_color = bars[bar_index].color;
                bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, alpha_at_index);*/
                bars[bar_index].color = setBarAlpha(bars[bar_index].color, alpha_at_index);
            }
            else // bar index > alalpha_changed_index
            {
                //set the previous bar's alpha to zero, so doesn't appear. Again using range 0-1f. Not 0-255.
                /*Color current_bar_color = bars[bar_index].color;
                bars[bar_index].color = new Color(current_bar_color.r, current_bar_color.g, current_bar_color.b, 0f);*/

                //bars[bar_index].color = setBarAlpha(bars[bar_index].color, 0f); // no alpha

                if (left_to_right_progression)
                {
                    bars[bar_index].color = setBarAlpha(bars[bar_index].color, 0f); // no alpha
                }
                else // RTL
                {
                    bars[bar_index].color = setBarAlpha(bars[bar_index].color, 1f); //full alpha
                }
            }
        }

        //Debug.Log($"End of updateProgressByValue(): {this.name} value: {value}");
    }

    Color setBarAlpha(Color a_bar_color, float alpha_value_at_index) // alpha_value_at_index is 0.0f to 1.0f
    {
        return new Color(a_bar_color.r, a_bar_color.g, a_bar_color.b, alpha_value_at_index);
    }

    //keep for Sprite's progress bar
    //meant to go from full to empty and one direction. Not jump between values
    public void updateOneDirectionalProgress(float delta_time)//progress goes right to left by fading: 0% <- 100%
    {
        bool isDone = progress_timer.tick_n_check(delta_time);

        //progress should be going down
        progress = 1f - progress_timer.getTimerCompletionPercentage();// ranges from 0.0 to 1.0 as 100%

        int bar_index = 0;

        if (!isDone)
        { 
            bar_index = (int)(Mathf.Ceil(progress / percentage_per_bar)) - 1;
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
