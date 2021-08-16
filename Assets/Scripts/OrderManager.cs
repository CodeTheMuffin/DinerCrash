using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    static int orders = 7;

    public GameObject order_box_prefab;

    public OrderBoxManager[] orderBoxManagers = new OrderBoxManager[orders];

    private void Start()
    {
        //Debug.Log("Press 'I' to do stuff");
        resetALLOrderBoxesAndHide();
    }

    public int get_next_available_order_index()
    {
        int order_index = -1;  // no spots available

        for (int orderbox_index = 0; orderbox_index < orderBoxManagers.Length; orderbox_index++)
        {
            if (orderBoxManagers[orderbox_index] != null && orderBoxManagers[orderbox_index].isOrderAvailable())
            {
                order_index = orderbox_index;
                break;
            }
        }

        return order_index;
    }

    // Manly used at the start() or a new level
    public void resetALLOrderBoxesAndHide()
    {
        for (int orderbox_index = 0; orderbox_index < orderBoxManagers.Length; orderbox_index++)
        {
            if (orderBoxManagers[orderbox_index] != null)
            {
                orderBoxManagers[orderbox_index].makeAvailabile();
            }
        }
    }

    public void setOrderForm(OrderBoxManager box, OrderForm form)
    {
        // if there is no order box, then create one using a prefab!
        if (!box.isOrderBoxSet())
        {
            box.setOrderBox(GameObject.Instantiate(order_box_prefab, box.transform));
        }

        box.setOrderForm(form);
    }


    /*DEBUG
     private void Update()
    {
        quickTest();
    }

    void quickTest()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Press 'I' to do stuff"); // ALSO IN START
            //print("Next free order spot: " + get_next_available_order_index().ToString());
            resetALLOrderBoxesAndHide();
        }
    }*/
}
