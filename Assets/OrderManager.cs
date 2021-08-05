using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    static int orders = 7;

    public OrderBox[] orderBoxes = new OrderBox[orders];

    private void Start()
    {
        Debug.Log("Press 'I' to do stuff");
        resetALLOrderBoxesAndHide();
    }

    public int get_next_available_order_index()
    {
        int order_index = -1;  // no spots available

        for (int orderbox_index = 0; orderbox_index < orderBoxes.Length; orderbox_index++)
        {
            if (orderBoxes[orderbox_index] != null && orderBoxes[orderbox_index].isOrderAvailable())
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
        for (int orderbox_index = 0; orderbox_index < orderBoxes.Length; orderbox_index++)
        {
            if (orderBoxes[orderbox_index] != null)
            {
                orderBoxes[orderbox_index].makeAvailabile();
            }
        }
    }

    public void setOrderForm(OrderBox box, OrderForm form)
    {
        box.setOrderForm(form);
    }


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
    }
}
