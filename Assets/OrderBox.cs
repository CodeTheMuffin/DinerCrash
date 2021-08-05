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

    /*enum Status { 
        empty = 0,
        ready_for_pick_up = 1,
        being_served = 2, //player picked up order 
    }*/

    public GameObject order_background;

    public GameObject[] order_options = new GameObject[4];

    // to represent the different cookies and milk options
    /*public GameObject option01; // represents cholocate_cookie
    public GameObject option02; // represents oatmeal_raisan_cookie
    public GameObject option03; // represents normal_milk
    public GameObject option04; // represents warm_milk*/

    // if the order is ready to used for another order
    public bool order_spot_available = true;

    Transform order_box_original_transform;

    public OrderForm orderForm; 

    // Start is called before the first frame update
    void Awake()
    {
        order_box_original_transform = order_box.transform;
    }


    // Used to determine if a new order can be placed at this order box location
    public bool isOrderAvailable()
    {
        bool isAvailable = false;

        if (orderForm == null && order_spot_available)
        {
            isAvailable = true;
        }

        return isAvailable;
    }

    

    public void resetPosition()
    {
        order_box.GetComponent<Transform>().position = order_box_original_transform.position;
        order_box.GetComponent<Transform>().localPosition = order_box_original_transform.localPosition;
        order_box.GetComponent<Transform>().rotation = order_box_original_transform.rotation;
        order_box.GetComponent<Transform>().localScale = order_box_original_transform.localScale;
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
        /*if (orderForm.cholocate_cookie_counter > 0)
        { option01_visibility(true); }
        if (orderForm.oatmeal_raisan_cookie_counter > 0)
        { option02_visibility(true); }
        if (orderForm.normal_milk_counter > 0)
        { option03_visibility(true); }
        if (orderForm.warm_milk_counter > 0)
        { option04_visibility(true); }*/


        orderBoxVisibility(true);
        orderBackgroundVisibility(true);

        print("Starting to set option visible");

        if (orderForm.cholocate_cookie_counter > 0)
        { optionVisibility((int)Options.cholocate_cookie, true); }

        if (orderForm.oatmeal_raisan_cookie_counter > 0)
        { optionVisibility((int)Options.oatmeal_raisan_cookie, true); }

        if (orderForm.normal_milk_counter > 0)
        { optionVisibility((int)Options.normal_milk, true); }

        if (orderForm.warm_milk_counter > 0)
        { optionVisibility((int)Options.warm_milk, true); }

        print("Done");
    }


    public void clearForm()
    {
        orderForm = null;
    }

    public void orderBoxVisibility(bool visible)
    {
        order_box.SetActive(visible);
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
        /*option01_visibility(true);
        option02_visibility(true);
        option03_visibility(true);
        option04_visibility(true);*/

        foreach (GameObject obj in order_options)
        {
            obj.SetActive(true);
        }
    }

    public void hideAllOptions()
    {
        /*option01_visibility(false);
        option02_visibility(false);
        option03_visibility(false);
        option04_visibility(false);*/

        foreach (GameObject obj in order_options)
        {
            obj.SetActive(false);
        }
        print("Done hiding all options");
    }
    
    
    // When you want to make an order spot avaialable for a new order
    public void makeAvailabile()
    {
        resetPosition();
        clearForm();
        orderBoxVisibility(false);
        orderBackgroundVisibility(false);
        hideAllOptions();
        order_spot_available = true;
    }
    /*
        public void option01_visibility(bool visible)
        {
            option01.SetActive(visible);
        }

        public void option02_visibility(bool visible)
        {
            option02.SetActive(visible);
        }

        public void option03_visibility(bool visible)
        {
            option03.SetActive(visible);
        }

        public void option04_visibility(bool visible)
        {
            option04.SetActive(visible);
        }
    */
}
