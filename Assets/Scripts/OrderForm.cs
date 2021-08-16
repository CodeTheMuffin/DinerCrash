using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderForm
{
    public int cholocate_cookie_counter = 0;
    public int oatmeal_raisin_cookie_counter = 0;
    public int normal_milk_counter = 0;
    public int warm_milk_counter = 0;

    //since there are 4 counters; to be accessed through loops
    public int[] counters = new int[4];

    public int total_amount = 0;
    public bool discount_applied = false;
    public float prepare_time = 0f;

    // how much time it takes to prepare one cholocate cookie in seconds (ie 0.5f is a half a second)
    public const float prepare_time_for_cholocate_cookie = 0.35f;
    public const float prepare_time_for_oatmeal_raisin_cookie = 0.50f;
    public const float prepare_time_for_normal_milk = 0.25f;
    public const float prepare_time_for_warm_milk = 0.65f;

    // Start is called before the first frame update
    public OrderForm(int cholocate_counter, int oatmeal_counter, int milk_counter, int w_milk_counter, int total, bool discount)
    {
        cholocate_cookie_counter = cholocate_counter;
        oatmeal_raisin_cookie_counter = oatmeal_counter;
        normal_milk_counter = milk_counter;
        warm_milk_counter = w_milk_counter;

        counters[(int)OrderOptions.Options.cholocate_cookie] = cholocate_cookie_counter;
        counters[(int)OrderOptions.Options.oatmeal_raisin_cookie] = oatmeal_raisin_cookie_counter;
        counters[(int)OrderOptions.Options.normal_milk] = normal_milk_counter;
        counters[(int)OrderOptions.Options.warm_milk] = warm_milk_counter;


        total_amount = total;
        discount_applied = discount;
        prepare_time = get_estimated_prepare_time( cholocate_counter,  oatmeal_counter,  milk_counter,  w_milk_counter);
    }

    public int getTotalQuantity()
    {
        return cholocate_cookie_counter + oatmeal_raisin_cookie_counter + normal_milk_counter + warm_milk_counter;
    }


    // if a counter > 0, then count it as 1. 
    public int getTotalOptionsSelected()
    {
        int totalSelected = 0;
        totalSelected += (cholocate_cookie_counter > 0) ? 1 : 0;
        totalSelected += (oatmeal_raisin_cookie_counter > 0) ? 1 : 0;
        totalSelected += (normal_milk_counter > 0) ? 1 : 0;
        totalSelected += (warm_milk_counter > 0) ? 1 : 0;

        return totalSelected;
    }


    public static float get_estimated_prepare_time(int cholocate_counter, int oatmeal_counter, int milk_counter, int w_milk_counter)
    {
        float process_time = (cholocate_counter * prepare_time_for_cholocate_cookie) + (oatmeal_counter * prepare_time_for_oatmeal_raisin_cookie) + 
            (milk_counter * prepare_time_for_normal_milk) + (w_milk_counter * prepare_time_for_warm_milk);
        return process_time;
    }

}
