using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPoint : MonoBehaviour
{
    public bool isFree = true;
    List<GameObject> current_objects = new List<GameObject>();

    public static bool showSprite = true;

    private void Start()
    {
        if (!showSprite)
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
        print($"HIT! {collision.name}");
        if (collision.tag == "npc")
        {
            print(">>Hit NPC!!!");
            //isFree = false;
            current_objects.Add(collision.gameObject);
            isFree = false;
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
            GameObject g_obj = collision.gameObject;

            current_objects.Remove(g_obj);

            if (current_objects.Count == 0)
            {
                isFree = true;
            }
        }
    }
}
