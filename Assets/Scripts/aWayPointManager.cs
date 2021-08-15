using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class aWayPointManager : MonoBehaviour
{
    public aWayPoint[] myPoints;

    // scrambles myPoints, then goes through the scrambled list and find the first wayPoint that is free

    public aWayPoint getRandomStandingWayPoint()
    {
        aWayPoint newPoint = myPoints[0];

        aWayPoint[] scrambledPoints = getScrambledPoints();

        foreach (aWayPoint scram_point in scrambledPoints)
        {
            if (scram_point.isWayPointFree())
            {
                newPoint = scram_point;
                break;
            }
        }

        //for standing waypoints only
        return newPoint;
    }

    aWayPoint[] getScrambledPoints()
    {
        // Got idea from: 
        // https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net#answer-108845
        
        List<aWayPoint> inputList = myPoints.ToList();
        aWayPoint[] output = new aWayPoint[myPoints.Length];
        int i = 0;
        //System.Random randomizer = new System.Random();

        while(inputList.Count > 0)
        {
            //int index = randomizer.Next(inputList.Count);
            int index = UnityEngine.Random.Range(0,inputList.Count-1);
            output[i++] = inputList[index];
            inputList.RemoveAt(index);
        }

        return output;
    }

    public int getNextWayPointIndex(aWayPoint point)
    {
        int index = -1;
        bool point_found = false;

        for (int i = 0; i < myPoints.Length; i++)
        {
            if (myPoints[i] == point)
            {
                point_found = true;
                if (i + 1 < myPoints.Length)
                {
                    index = i + 1;
                }
                break;
            }
        }

        if (!point_found)
            index = -10;

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
