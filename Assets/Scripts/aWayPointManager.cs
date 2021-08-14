using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPointManager : MonoBehaviour
{
    public aWayPoint[] myPoints;

    public aWayPoint getRandomStandingWayPoint()
    {
        //for standing waypoints only
        return myPoints[0];
    }


    public int getNextWayPointIndex(aWayPoint point)
    {
        int index = -1;

        for (int i = 0; i < myPoints.Length; i++)
        {
            if (myPoints[i] == point)
            {
                if (i + 1 < myPoints.Length)
                {
                    index = i + 1;
                }
                break;
            }
        }

        return index;
    }

    public aWayPoint getWayPoint(int index)
    {
        if (index >= 0 && index < myPoints.Length)
        {
            return myPoints[index];
        }
        return null;
    }
}
