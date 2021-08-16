using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPoint : MonoBehaviour
{
    public bool isFree = true;
    List<GameObject> current_objects = new List<GameObject>();

    public static bool showSpriteAtStart = false; // for when you want to DEBUG the wayPoints

    private void Start()
    {
        if (!showSpriteAtStart)
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool isWayPointFree()
    {
        return isFree;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print($"HIT! {collision.name}");
        if (collision.tag == "npc")
        {
            //isFree = false;
            current_objects.Add(collision.gameObject);
            isFree = false;

            /*if (collision.GetComponent<NPC>().current_state == (int)NPC.State.exitting)
            {
                print($"Enter Exitting object at {transform.name}");
            }*/
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "npc")
        {
            isFree = false;
        }
    }*/


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "npc")
        {
            /*if (collision.GetComponent<NPC>().current_state == (int)NPC.State.exitting)
            {
                print($"Exiting exitting object at {transform.name}");
            }

            if (collision.GetComponent<NPC>().current_state == (int)NPC.State.dying)
            {
                print($"Exiting dying object at {transform.name}");
            }*/

            GameObject g_obj = collision.gameObject;

            current_objects.Remove(g_obj);

            if (current_objects.Count == 0)
            {
                isFree = true;
            }
        }
    }
}
