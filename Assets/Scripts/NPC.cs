using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPC_TextDecider text_decider;
    OrderForm expectedOrder;
    public aWayPoint currentWayPoint;
    public aWayPoint nextWayPoint;
    public aWayPoint orderingWayPoint;
    public aWayPoint standingWayPoint;

    public Timer walkingTimer;
    float walking_step;

    const float UPP = 0.125f; // unit per pixel
    public int current_state;
    public int node_index = 0;
    public bool ready_for_next_point = false; //for when the NPC reached the nextWayPoint
    public bool ready_to_order; // reached order waypoint
    public bool need_new_standing_point = false; // for when their current standing spot was taken; find new one!
    public bool wasOrderPlaced = false; // for when the player placed the order for the NPC // also indicates delivery

    //when combined all of them, consider not applying discount if only one of them have it
    // if applied, it would throw off weighted rating
    public List<OrderForm> orders = new List<OrderForm>();

    public string request_text = "";// should already be formatted to textbox
    public List<string> request_text_box = new List<string>();

    public ProgressBar progress_bar;
    public Color progress_color_entering = Color.green;
    public Color progress_color_ordering = Color.blue;
    public Color progress_color_standing = Color.yellow;

    public float progress_entering_wait_time = 30f;//Random.Range(20f, 30f);
    public float progress_ordering_wait_time = 25f;// Random.Range(15f, 25f);
    public float progress_standing_wait_time = 20f;// Random.Range(10f, 20f);



    Color deselectedColor = new Color(1f, 1f, 1f, 0.9f);
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

        request_text = text_decider.generateRequestsAndFormat();

        progress_entering_wait_time = (float)System.Math.Round(Random.Range(20f, 30f));
        progress_ordering_wait_time = (float)System.Math.Round(Random.Range(15f, 25f));
        progress_standing_wait_time = (float)System.Math.Round(Random.Range(10f, 20f));

        progress_bar.setProgessMaxTime(progress_entering_wait_time);
        progress_bar.progress_timer.timer_stopped = true;
        progress_bar.bar_color = progress_color_entering;
        progress_bar.resetProgress();

        //hide progress bar until reached first entering way point!
        progress_bar.gameObject.SetActive(false);

        //print(request_text);
    }

    public void tellPlayerOrder()
    {
        text_decider.updateNPCtext(request_text);
    }

    public void setOrderForm(OrderForm order)
    {
        expectedOrder = order;
    }

    // when the player hands over the order to the customer
    public void receiveOrderFromPlayer(OrderForm order)
    {
        orders.Add(order);

        /*
         KEYS:
        "rating"
        "missing"
        "missing_total"
        "weighted_rating"
         */
        Dictionary<string, object> rating_info = OrderForm.rateOrderReceived(expectedOrder, order);

        text_decider.updateSystemText($"Weighted Rating:{System.Math.Round((float)rating_info["weighted_rating"] * 100)}%");
        //print($"Weighted rating: {(float)rating_info["weighted_rating"]*100}%");
    }

    public void updateTimer()
    {
        walkingTimer.max_time_in_seconds = UnityEngine.Random.Range(0.05f, 0.2f); // the amount of time to move 0.125 units (1 pixel)
    }

    public void updateProgressbar(float delta_time)
    { 
        
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

            if (canMove)
            {
                if (nextWayPoint.isWayPointFree())
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextWayPoint.transform.position, UPP);
                    //ready_for_next_point = true;
                    walkingTimer.reset_timer();
                }
                else if (current_state == (int)NPC.State.standing && ready_for_next_point)
                {
                    need_new_standing_point = true;
                }
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

    public void prepareForStanding()
    {
        // after order is placed, transition from entering to standing
        current_state = (int)NPC.State.standing;
        ready_for_next_point = false;
        need_new_standing_point = true;
        ready_to_order = true;// maybe set to false??
        wasOrderPlaced = true;
    }

    public void prepareForExitting()
    { 
        current_state = (int)NPC.State.exitting;
        ready_for_next_point = true;
        print("Preparing for exitting");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print($"Something collided with NPC {collision.name} {collision.tag}");
        if (collision.tag == "waypoint")
        {
            /*
            isFree = false;
            current_object = collision.gameObject;*/
            aWayPoint collidedWayPoint = collision.GetComponent<aWayPoint>();

            if (collidedWayPoint == nextWayPoint)
            {
                //unhide progress bar now that it reached (persumably) the first entering way point!
                if (!progress_bar.gameObject.activeSelf)
                {
                    progress_bar.gameObject.SetActive(true);
                    progress_bar.progress_timer.reset_timer();
                }

                collidedWayPoint.isFree = false;
                if (collidedWayPoint != orderingWayPoint && collidedWayPoint != standingWayPoint)
                {
                    ready_for_next_point = true;
                }
                else
                {
                    ready_for_next_point = false;
                    ready_to_order = true;
                }
            }
        }
    }
}
