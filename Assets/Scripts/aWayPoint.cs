using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPoint : MonoBehaviour
{
    bool isFree = false;
    GameObject current_object;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isWayPointFree()
    {
        return isFree;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "npc")
        {
            isFree = false;
            current_object = collision.gameObject;
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
            isFree = true;
            current_object = null;
        }
    }
}
