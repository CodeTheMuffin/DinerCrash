using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    OrderForm expectedOrder;

    public void setOrderForm(OrderForm order)
    {
        expectedOrder = order;
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void setSprite(Sprite s)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }
}
