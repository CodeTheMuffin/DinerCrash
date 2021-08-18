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
    public const float prepare_time_for_cholocate_cookie = 0.35f; // max time (ie. 9* 0.35f) = 3.15 seconds
    public const float prepare_time_for_oatmeal_raisin_cookie = 0.50f; // max time 4.5 seconds
    public const float prepare_time_for_normal_milk = 0.25f;    // max time 2.25 seconds
    public const float prepare_time_for_warm_milk = 0.65f;  // max time 5.85 seconds
    // absolute max: 15.75 seconds

    public OrderForm()
    { }

    // Start is called before the first frame update
    public OrderForm(int cholocate_counter, int oatmeal_counter, int milk_counter, int w_milk_counter, int total, bool discount)
    {
        cholocate_cookie_counter = cholocate_counter;
        oatmeal_raisin_cookie_counter = oatmeal_counter;
        normal_milk_counter = milk_counter;
        warm_milk_counter = w_milk_counter;

        UpdateCounters();

        total_amount = total;
        if (getTotalQuantity() > 0)
        {
            discount_applied = discount;
        }
        prepare_time = get_estimated_prepare_time();
    }

    public int getTotalQuantity()
    {
        return cholocate_cookie_counter + oatmeal_raisin_cookie_counter + normal_milk_counter + warm_milk_counter;
    }

    public void UpdateCounters()
    {
        counters[(int)OrderOptions.Options.cholocate_cookie] = cholocate_cookie_counter;
        counters[(int)OrderOptions.Options.oatmeal_raisin_cookie] = oatmeal_raisin_cookie_counter;
        counters[(int)OrderOptions.Options.normal_milk] = normal_milk_counter;
        counters[(int)OrderOptions.Options.warm_milk] = warm_milk_counter;
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

    public float get_estimated_prepare_time()
    {
        return get_estimated_prepare_time(cholocate_cookie_counter, oatmeal_raisin_cookie_counter, normal_milk_counter, warm_milk_counter);
    }

    public static float get_estimated_prepare_time(int cholocate_counter, int oatmeal_counter, int milk_counter, int w_milk_counter)
    {
        float process_time = (cholocate_counter * prepare_time_for_cholocate_cookie) + (oatmeal_counter * prepare_time_for_oatmeal_raisin_cookie) + 
            (milk_counter * prepare_time_for_normal_milk) + (w_milk_counter * prepare_time_for_warm_milk);
        return process_time;
    }


    //TODO: should take received Time into account!!!
    public static Dictionary<string, object> rateOrderReceived(OrderForm expected, OrderForm received)
    {
        int rating = 0;// want rating of 0 or higher!
        int[] missing = {0, 0, 0, 0};// expected to me the same length as counters[]

        //using the prepare time as the weights for receiving vs missing and rating!
        float[] weight = { prepare_time_for_cholocate_cookie, prepare_time_for_oatmeal_raisin_cookie, prepare_time_for_normal_milk, prepare_time_for_warm_milk };
        int missing_total = 0;

        float expected_weighted_rating = 0f;
        float received_weighted_rating = 0f;
        float missing_weighted_rating  = 0f;

        /*
            Basic formuala rating
            
            Expected or Received or Missing rating formula ==> SUM(Quantity of Option * Option's weight)
            
            // if discount is applied, then the Missing values are halved, otherwise, they have full negative weight.
            Rating % = [Received - ( Missing * 0.5 (if discount applied))] / Expected

            Example:
            Lets say there are options A, B, C and D.
            with respective prep times/weights of 0.35, 0.50, 0.25, and 0.65.

            Lets say the expected order is 4A, 3B, 2C, and 1D.
            This would mean, the Expected rating would be (4*0.35) + (3*0.50) + (2*0.25) + (1*0.65) = 4.05

            Lets say we actually received 6A, 4B, 1C, and 0D.
            This would mean the Received rating would be (6*0.35) + (4*0.50) + (1*0.25) + (0*0.65) = 4.35

            We count the missing items. AKA for each option, subtract expected - received. if negative, that option was missing!
            So this would mean 0A, 0B, 1C and 1D.
            This would mean the Missing rating would be (0*0.35) + (0*0.50) + (1*0.25) + (1*0.65) = 0.90

            Lets see how a discount could change the rating!
            >>Discount NOT APPLIED
            Rating %    = [Received - ( Missing * 1)] / Expected
                        = [4.35 - 0.90]/ 4.05
                        = [3.45]/ 4.05 =    ~85.185% Not too bad!
            
            >>Discount APPLIED
            Rating %    = [Received - ( Missing * 0.5)] / Expected
                        = [4.35 - (0.90 * 0.5)]/ 4.05 =  ~85.185% Not too bad!
                        = [3.9] / 4.05 =    ~96.29 % really nice!!

            If nothing was received: it should give a (-)100%
         */


        for (int index = 0; index < expected.counters.Length; index++)
        {
            int expected_val = expected.counters[index];
            int received_val = received.counters[index];
            int diff = expected_val - received_val;

            if (diff > 0) // missing expecting {diff} more 
            {
                missing_total += diff;
                missing[index] = diff;
                rating-= diff;
            }
            else if (diff < 0) // received more than expected
            {
                rating += (diff * -1);
            }

            float current_weight = weight[index];

            expected_weighted_rating += (expected_val   * current_weight);
            received_weighted_rating += (received_val   * current_weight);
            missing_weighted_rating  += (missing[index] * current_weight);
        }

        if (received.discount_applied)
        {
            missing_weighted_rating *= 0.5f;
        }

        float weighted_rating = (received_weighted_rating - missing_weighted_rating)/ expected_weighted_rating;
        weighted_rating += 1f; // so that if missed everything, it show 0% rather than -100%
        
        Dictionary<string, object> rating_info = new Dictionary<string, object>();

        rating_info.Add("rating", rating);
        rating_info.Add("missing", missing);
        rating_info.Add("missing_total", missing_total);
        rating_info.Add("weighted_rating", weighted_rating);

        return rating_info;
    }

    public void addToOrder(OrderForm receivedOrder)
    {
        cholocate_cookie_counter += receivedOrder.cholocate_cookie_counter;
        oatmeal_raisin_cookie_counter += receivedOrder.oatmeal_raisin_cookie_counter;
        normal_milk_counter += receivedOrder.normal_milk_counter;
        warm_milk_counter += receivedOrder.warm_milk_counter;

        UpdateCounters();

        total_amount += receivedOrder.total_amount;

        if (receivedOrder.getTotalQuantity() > 0)
        {
            discount_applied = discount_applied? discount_applied : receivedOrder.discount_applied;
        }
        prepare_time = get_estimated_prepare_time();
    }

    public override string ToString()
    {
        return $"{cholocate_cookie_counter} cholocate chips cookies, {oatmeal_raisin_cookie_counter} oatmeal raisin cookies, " +
            $"{normal_milk_counter} cold milks and {warm_milk_counter} warm milks.";
    }
}
