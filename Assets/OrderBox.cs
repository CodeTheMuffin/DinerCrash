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

    public GameObject[] order_options = new GameObject[4];

    // to represent the different cookies and milk options
    /*public GameObject option01; // represents cholocate_cookie
    public GameObject option02; // represents oatmeal_raisan_cookie
    public GameObject option03; // represents normal_milk
    public GameObject option04; // represents warm_milk*/

    // if the order is ready to used for another order
    public bool order_spot_available = true;

    Vector2 order_box_original_position;

    public OrderForm orderForm; 

    // Start is called before the first frame update
    void Start()
    {
        order_box_original_position = order_box.transform.position;
    }

    public bool isOrderAvailable()
    {
        bool isAvailable = false;

        if (orderForm == null && order_spot_available)
        {
            isAvailable = true;
        }

        return isAvailable;
    }

    public void hideOrder()
    {
        order_box.SetActive(false);
    }

    public void showOrder()
    {
        order_box.SetActive(true);
    }

    public void resetPosition()
    {
        order_box.GetComponent<Transform>().position = order_box_original_position;
    }

    public void setOrderForm(OrderForm form)
    {
        orderForm = form;
        hide_all_options();
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



        if (orderForm.cholocate_cookie_counter > 0)
        { option_visibility((int)Options.cholocate_cookie, true); }

        if (orderForm.oatmeal_raisan_cookie_counter > 0)
        { option_visibility((int)Options.oatmeal_raisan_cookie, true); }

        if (orderForm.normal_milk_counter > 0)
        { option_visibility((int)Options.normal_milk, true); }

        if (orderForm.warm_milk_counter > 0)
        { option_visibility((int)Options.warm_milk, true); }

    }


    public void clearForm()
    {
        orderForm = null;
    }

    /* OPTIONS */
    public void option_visibility(int index, bool visible)
    {
        order_options[index].SetActive(visible);
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
    public void show_all_options()
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

    public void hide_all_options()
    {
        /*option01_visibility(false);
        option02_visibility(false);
        option03_visibility(false);
        option04_visibility(false);*/

        foreach (GameObject obj in order_options)
        {
            obj.SetActive(false);
        }
    }


}
