using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderForm
{
    public int cholocate_cookie_counter = 0;
    public int oatmeal_raisan_cookie_counter = 0;
    public int normal_milk_counter = 0;
    public int warm_milk_counter = 0;
    public int total_amount = 0;
    public bool discount_applied = false;

    // Start is called before the first frame update
    public OrderForm(int cholocate_counter, int oatmeal_counter, int milk_counter, int w_milk_counter, int total, bool discount)
    {
        cholocate_cookie_counter = cholocate_counter;
        oatmeal_raisan_cookie_counter = oatmeal_counter;
        normal_milk_counter = milk_counter;
        warm_milk_counter = w_milk_counter;
        total_amount = total;
        discount_applied = discount;
    }

}
