using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public TextSystem txtSys;
    public NPC_Manager npc_manager;
    public Timer gameTimer;

    public float max_gametime = 300f; //for 5 minutes (5 min * 60 sec/min)

    public void Start()
    {
        gameTimer.max_time_in_seconds = max_gametime;
        gameTimer.reset_timer();
    }


}
