using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPointSuperManager : MonoBehaviour
{
    public aWayPoint spawningPoint;
    public aWayPointManager[] pointManagers;
    //0 = Entrance      1= Standing/waiting         2= Exitting 

    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public Transform getSpawningTransform()
    {
        return spawningPoint.GetComponent<Transform>();
    }

    public Tuple<int, aWayPoint> getNextStateAndWayPoint(int current_state, aWayPoint current_point)
    {
        int new_state = -5;
        aWayPoint new_waypoint = null;

        if (current_state == (int)NPC.State.spawned && current_point == spawningPoint)
        {
            new_state = (int)NPC.State.entering;
            new_waypoint = pointManagers[(int)NPC.State.entering].getWayPoint(0);
        }
        else if (current_state == (int)NPC.State.entering)
        {
            int index = pointManagers[current_state].getNextWayPointIndex(current_point);

            if (index == -1) // no more left, then go to next available state
            {
                new_state = current_state + 1;
                new_waypoint= pointManagers[current_state + 1].getRandomStandingWayPoint();
                /*if(System.Enum.IsDefined(typeof(NPC.State), current_state +1)) // if there is another state to move to
                {
                    if (current_state + 1 == (int)NPC.State.spawned)
                    {
                        return pointManagers[current_state + 1].getRandomStandingWayPoint();
                    }
                }*/
            }
            else
            {
                new_state = current_state;
                new_waypoint= pointManagers[current_state].getWayPoint(index);
            }
        }
        else if (current_state == (int)NPC.State.standing)
        {
            new_state = (int)NPC.State.exitting;
            new_waypoint= pointManagers[current_state + 1].getWayPoint(0);
        }
        else if(current_state == (int)NPC.State.exitting)
        {
            int index = pointManagers[current_state].getNextWayPointIndex(current_point);

            if (index == -1) // no more left, then go to next available state
            {
                //prepare for deletion
                new_state = (int)NPC.State.dying;
                new_waypoint = null;
            }
            else
            {
                new_state = current_state;
                new_waypoint = pointManagers[current_state].getWayPoint(index);
            }
        }

        return Tuple.Create(new_state, new_waypoint);
    }

}
