using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aWayPointSuperManager : MonoBehaviour
{
    public aWayPoint spawningPoint;
    public aWayPointManager[] pointManagers;
    //0 = Entrance      1= Standing/waiting         2= Exitting 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform getSpawningTransform()
    {
        return spawningPoint.GetComponent<Transform>();
    }
}
