using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBox : MonoBehaviour
{
    public int order_index = 0;
    public GameObject order_box;

    enum Options {
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

    /*enum Status { 
        empty = 0,
        ready_for_pick_up = 1,
        being_served = 2, //player picked up order 
    }*/

    public GameObject order_background;

    public GameObject[] order_options = new GameObject[4];

    // if the order location is ready to used for another order
    public bool order_spot_available = true;

    Transform order_box_original_transform;

    public OrderForm orderForm; 

    // Start is called before the first frame update
    void Awake()
    {
        order_box_original_transform = order_background.transform;
        turnOffAnimatedBackground();
    }


    // Used to determine if a new order can be placed at this order box location
    public bool isOrderAvailable()
    {
        bool isAvailable = false;

        if ( !isOrderBoxSet() || (!isFormSet() && order_spot_available))
        {
            isAvailable = true;
        }

        return isAvailable;
    }

    public bool isOrderBoxSet()
    { return order_box != null; }

    public bool isFormSet()
    { return orderForm != null; }

    public bool canOrderBePickedUp()
    {
        print("box id: " +order_index.ToString() + "___isOrderSet: " + isOrderBoxSet().ToString() + "___isFormSet: "+ isFormSet().ToString() + "___spot avail: " +(!order_spot_available).ToString());
        print("Combined: " + (isOrderBoxSet() && isFormSet() && !order_spot_available).ToString());
        return isOrderBoxSet() && isFormSet() && !order_spot_available;
    }

    public void setOrderBox(GameObject orderbox)
    {
        order_box = orderbox;
        reassignAllOrderOptions();
    }

    
    public void reassignAllOrderOptions()
    {
        if (order_box)
        {
            //Clears the order_options[]
            System.Array.Clear(order_options, 0, order_options.Length);

            // look at all the children and if there is a child's name that matches the key-value pair and can be used in the Options enum
            // then assign that child's gameobject to the respected index
            foreach (Transform child in order_box.transform)
            {
                if (option_names_to_index.ContainsKey(child.name))
                {
                    int index = option_names_to_index[child.name];

                    // if the child's name exists as a value in the Options enum
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
    }
    

    public void resetPosition()
    {
        if (isOrderBoxSet())
        {
            order_box.GetComponent<Transform>().position = order_box_original_transform.position;
            order_box.GetComponent<Transform>().localPosition = order_box_original_transform.localPosition;
            order_box.GetComponent<Transform>().rotation = order_box_original_transform.rotation;
            order_box.GetComponent<Transform>().localScale = order_box_original_transform.localScale;
        }
    }

    public void setOrderForm(OrderForm form)
    {
        print("Setting orderform");
        orderForm = form;

        hideAllOptions();
        /*
         cholocate_cookie
         oatmeal_raisan_cookie
         normal_milk
         warm_milk
         */


        orderBoxVisibility(true);
        orderBackgroundVisibility(true);

        print("Starting to set option visible");

        if (orderForm.cholocate_cookie_counter > 0)
        { optionVisibility( (int)Options.cholocate_cookie, true); }

        if (orderForm.oatmeal_raisan_cookie_counter > 0)
        { optionVisibility( (int)Options.oatmeal_raisan_cookie, true); }

        if (orderForm.normal_milk_counter > 0)
        { optionVisibility( (int)Options.normal_milk, true); }

        if (orderForm.warm_milk_counter > 0)
        { optionVisibility( (int)Options.warm_milk, true); }

        order_spot_available = false;
        print("Done");
    }


    public void clearForm()
    {
        orderForm = null;
    }

    public void clearOrderBox()
    {
        order_box = null;
    }

    public void orderBoxVisibility(bool visible)
    {
        if (isOrderBoxSet())
        {
            order_box.SetActive(visible);
        }
    }

    public void orderBackgroundVisibility(bool visible)
    {
        order_background.SetActive(visible);
    }

    /* OPTIONS */
    public void optionVisibility(int index, bool visible)
    {
        order_options[index].SetActive(visible);
    }

    public void showAllOptions()
    {
        foreach (GameObject obj in order_options)
        {
            obj.SetActive(true);
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
    
    
    // When you want to make an order spot avaialable for a new order
    public void makeAvailabile()
    {
        resetPosition();
        clearForm();
        orderBoxVisibility(false);
        orderBackgroundVisibility(true);
        turnOffAnimatedBackground();
        hideAllOptions();
        order_spot_available = true;
    }

    // like makeAvailabile(), but assumed that the player is holding the order
    public void pickUpOrder()
    {
        clearForm();
        clearOrderBox();
        order_spot_available = true;
    }

    // This is for when the player gets close to the order box
    // Mimics highlighting and able to pickup order box
    public void turnOnAnimatedPickUpBackground()
    {
        // mimics the red in my color pallate #9f294e a deep red color
        order_background.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(159, 41, 78, 255);
    }

    // This is for when the player gets close to the order box
    // Mimics highlighting and able to put down an order box (if one is being held)
    public void turnOnAnimatedPutDownBackground()
    {
        // mimics the red in my color pallate #49a790 a dark blue-green color
        order_background.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(73, 167, 144, 255);
    }

    public void turnOffAnimatedBackground()
    {
        //mimics the white in my color pallate #fdf7ed
        order_background.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(253, 247, 237, 255);
    }
}
