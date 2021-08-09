using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBoxManager : MonoBehaviour
{
    public int order_index = 0;
    public GameObject order_background;
    public GameObject order_box_game_obj;
    public OrderBox order_box; // the OrderBox object in order_box_game_obj

    // if the order location is ready to used for another order
    public bool order_spot_available = true;

    // Start is called before the first frame update
    void Awake()
    {
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
    { return order_box_game_obj != null; }

    public bool isFormSet()
    { return order_box != null && order_box.isFormSet(); }

    public bool canOrderBePickedUp()
    {
        //print("box id: " +order_index.ToString() + "___isOrderSet: " + isOrderBoxSet().ToString() + "___isFormSet: "+ isFormSet().ToString() + "___spot avail: " +(!order_spot_available).ToString());
        //print("Combined: " + (isOrderBoxSet() && isFormSet() && !order_spot_available).ToString());
        return isOrderBoxSet() && isFormSet() && !order_spot_available;
    }

    public void setOrderBox(GameObject orderbox)
    {
        order_box_game_obj = orderbox;
        order_box = order_box_game_obj.GetComponent<OrderBox>();
        order_box.reassignAllOrderOptions();
    }

    /*public void resetPosition()
    {
        if (isOrderBoxSet())
        {
            order_box_game_obj.GetComponent<Transform>().position = order_box_game_obj_original_transform.position;
            order_box_game_obj.GetComponent<Transform>().localPosition = order_box_game_obj_original_transform.localPosition;
            order_box_game_obj.GetComponent<Transform>().rotation = order_box_game_obj_original_transform.rotation;
            order_box_game_obj.GetComponent<Transform>().localScale = order_box_game_obj_original_transform.localScale;
        }
    }*/

    public void setOrderForm(OrderForm form)
    {
        order_box.setOrderForm(form);
        //order_box.hideAllOptions();
        orderBoxVisibility(true);
        orderBackgroundVisibility(true);
        order_spot_available = false;
    }

    public void clearOrderBox()
    {
        order_box_game_obj = null;
        order_box = null;
    }

    public void orderBoxVisibility(bool visible)
    {
        if (isOrderBoxSet())
        {
            order_box_game_obj.SetActive(visible);
        }
    }

    public void orderBackgroundVisibility(bool visible)
    {
        order_background.SetActive(visible);
    }

    public void hideAllOptions()
    {
        if (isOrderBoxSet())
        {
            order_box.hideAllOptions();
        }
    }
    
    
    // When you want to make an order spot avaialable for a new order
    public void makeAvailabile()
    {
        //resetPosition();
        orderBoxVisibility(false);
        orderBackgroundVisibility(true);
        turnOffAnimatedBackground();
        hideAllOptions();
        clearOrderBox();
        order_spot_available = true;
    }

    // like makeAvailabile(), but assumed that the player is holding the order
    public GameObject pickUpOrder(Transform newParent)
    {
        if (isOrderBoxSet())
        {
            GameObject order_box = order_box_game_obj;
            order_box_game_obj.transform.SetParent(newParent);
            clearOrderBox();
            turnOnAnimatedPutDownBackground();
            order_spot_available = true;
            return order_box;
        }
        return null;
    }

    public void placeOrderOnCounter(Transform orderBox)
    {
        orderBox.SetParent(gameObject.transform);
        orderBox.localPosition = Vector3.zero;
        order_spot_available = false;
        turnOnAnimatedPickUpBackground();
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
