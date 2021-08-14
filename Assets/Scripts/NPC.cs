using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    OrderForm expectedOrder;
    public aWayPoint currentWayPoint;
    public aWayPoint nextWayPoint;

    public Timer walkingTimer;
    float walking_step;

    const float UPP = 0.125f; // unit per pixel
    public int current_state;
    public int node_index = 0;
    public bool ready_for_next_point = false; //for when the NPC reached the nextWayPoint

    Color deselectedColor = new Color(1f, 1f, 1f, 0.8f);
    Color selectedColor = new Color(1f, 1f, 1f, 1f);

    public enum State
    {
        spawned = -1,
        entering = 0,
        standing = 1,
        exitting = 2,
        dying = 666
    }

    public void justSpawnedHandler()
    {
        updateTimer();
        turnOffHighlight();
        current_state = (int)State.entering;
    }

    public void setOrderForm(OrderForm order)
    {
        expectedOrder = order;
    }

    public void updateTimer()
    {
        walkingTimer.max_time_in_seconds = UnityEngine.Random.Range(0.1f, 0.25f);
    }

    public void updateTimerForEntering()
    {
        float minTime = 1f;
        float maxTime = 2f;
        float time = Random.Range(minTime, maxTime);
        walkingTimer.max_time_in_seconds = time;
    }

    public void updateTimerForStanding()
    {
        float minTime = 1.5f;
        float maxTime = 3f;
        float time = Random.Range(minTime, maxTime);
        walkingTimer.max_time_in_seconds = time;
    }

    public void updateTimerForExitting()
    {
        float minTime = 1.25f;
        float maxTime = 2.5f;
        float time = Random.Range(minTime, maxTime);
        walkingTimer.max_time_in_seconds = time;
    }

    void FixedUpdate()
    {
        updateMovement(Time.deltaTime);
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    void calculateWalkingStep()
    {
        if (currentWayPoint && nextWayPoint)
        {
            Transform current_transform = currentWayPoint.transform;
            Transform next_transform = nextWayPoint.transform;


        }
    }

    void updateMovement(float delta_time)
    {
        if (nextWayPoint && (!ready_for_next_point || getDistanceToNextWayPoint() > 0.5f)) // if the next to next way point is 1 unit
        {
            bool canMove = walkingTimer.tick_n_check(delta_time);

            if (canMove && nextWayPoint.isWayPointFree())
            {
                transform.position = Vector2.MoveTowards(transform.position, nextWayPoint.transform.position, UPP);
                //ready_for_next_point = true;
                walkingTimer.reset_timer();
            }
        }
    }

    float getDistanceToNextWayPoint()
    {
        return Vector2.Distance(transform.position, nextWayPoint.transform.position);
    }

    public void setSprite(Sprite s)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

    public void turnOnHighlight()
    {
        gameObject.GetComponent<SpriteRenderer>().color = selectedColor;
    }

    public void turnOffHighlight()
    {
        gameObject.GetComponent<SpriteRenderer>().color = deselectedColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "waypoint")
        {
            /*
            isFree = false;
            current_object = collision.gameObject;*/
            aWayPoint collidedWayPoint = collision.GetComponent<aWayPoint>();

            if (collidedWayPoint == nextWayPoint)
            {
                collidedWayPoint.isFree = false;
                ready_for_next_point = true;
            }
        }
    }
}
