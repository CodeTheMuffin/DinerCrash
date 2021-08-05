using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    static int orders = 7;

    public OrderBox[] orderBoxes = new OrderBox[orders];

    public int get_next_available_order_index()
    {
        int order_index = -1;  // no spots available

        for (int orderbox_index = 0; orderbox_index < orderBoxes.Length; orderbox_index++)
        {
            if (orderBoxes[orderbox_index].isOrderAvailable())
            {
                order_index = orderbox_index;
                break;
            }
        }

        return order_index;
    }
}
