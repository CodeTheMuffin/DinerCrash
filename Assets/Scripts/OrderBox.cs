using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBox : MonoBehaviour
{
    enum Options
    {
        cholocate_cookie = 0,
        oatmeal_raisan_cookie = 1,
        normal_milk = 2,
        warm_milk = 3
    }

    public static Dictionary<string, int> option_names_to_index = new Dictionary<string, int>() {
        {"opt 01", (int)Options.cholocate_cookie},
        {"opt 02", (int)Options.oatmeal_raisan_cookie},
        {"opt 03", (int)Options.normal_milk},
        {"opt 04", (int)Options.warm_milk}
    };

    public OrderForm orderForm;
    public GameObject[] order_options = new GameObject[4];

    public Timer processing_timer;
    public SpriteRenderer orderBox_sprite_renderer;

    // for evaulating the range the alpha level can be
    const int LOWEST_ALPHA = 100;
    const int HIGHEST_ALPHA = 255;

    Color32 lowest_color = new Color32(255, 255, 255, LOWEST_ALPHA);
    Color32 highest_color = new Color32(255, 255, 255, HIGHEST_ALPHA);
    Color32 lerped_color = new Color32(255, 255, 255, LOWEST_ALPHA);

    AudioManager audio_manager;
    OrderBoxManager orderbox_manager;

    public bool isFormSet()
    { return orderForm != null; }

    public void setOrderForm(OrderForm form)
    {
        orderForm = form;
        hideAllOptions();
        processing_timer.max_time_in_seconds = orderForm.prepare_time;
        processing_timer.reset_timer();
        orderBox_sprite_renderer.color = lowest_color;
        audio_manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioManager>();
        orderbox_manager = transform.parent.gameObject.GetComponent<OrderBoxManager>();
        //orderBox_sprite_renderer = gameObject.GetComponent<SpriteRenderer>();

        if (orderForm.cholocate_cookie_counter > 0)
        { optionVisibility((int)Options.cholocate_cookie, true); }

        if (orderForm.oatmeal_raisan_cookie_counter > 0)
        { optionVisibility((int)Options.oatmeal_raisan_cookie, true); }

        if (orderForm.normal_milk_counter > 0)
        { optionVisibility((int)Options.normal_milk, true); }

        if (orderForm.warm_milk_counter > 0)
        { optionVisibility((int)Options.warm_milk, true); }
    }

    public bool isOrderProcessed()
    {
        return processing_timer.isTimerDone();
    }

    public void preparing_order(float delta_time)
    {
        bool isTimerDown = processing_timer.tick_n_check(delta_time);

        lerped_color = Color32.Lerp(lowest_color, highest_color, processing_timer.getTimerCompletionPercentage());
        orderBox_sprite_renderer.color = lerped_color;
    }

    public void Update()
    {
        if (!processing_timer.isTimerDone())
        {
            preparing_order(Time.deltaTime);

            if (processing_timer.isTimerDone())
            {
                showOrderIsReady();
            }
        }
    }

    public void showOrderIsReady()
    {
        audio_manager.playOrderReadyForPickUp();
        orderbox_manager.turnOnAnimatedReadyForPickUp();
    }

    public void reassignAllOrderOptions()
    {
        //Clears the order_options[]
        System.Array.Clear(order_options, 0, order_options.Length);

        // look at all the children and if there is a child's name that matches the key-value pair and can be used in the Options enum
        // then assign that child's gameobject to the respected index
        foreach (Transform child in transform)
        {
            if (option_names_to_index.ContainsKey(child.name))
            {
                int index = option_names_to_index[child.name];

                // If the child's name exists as a value in the Options enum
                if (System.Enum.IsDefined(typeof(Options), index))
                {
                    order_options[index] = child.gameObject;
                }
                else
                {
                    Debug.LogError("ERROR [Order Box]>> Index value of : { " + index.ToString() + " } was not found in Options enum. Cannot set order_options.");
                }

            }
            else
            {
                Debug.LogError("ERROR [Order Box]>> Child name: { " + child.name + " } was not found. Cannot set order_options.");
            }

        }
    }

    public void hideAllOptions()
    {
        if (order_options.Length > 0)
        {
            foreach (GameObject obj in order_options)
            {
                if (obj)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    public void clearForm()
    {
        orderForm = null;
    }

    /* OPTIONS */
    public void optionVisibility(int index, bool visible)
    {
        order_options[index].SetActive(visible);
    }
}
