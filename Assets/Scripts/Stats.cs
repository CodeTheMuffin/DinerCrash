using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public TextSystem txtSys;
    public NPC_Manager npc_manager;
    public Timer gameTimer;

    //PB = ProgressBar
    public ProgressBar BudgetPB;
    public ProgressBar SuccessPB;
    public ProgressBar TimePB;

    public float max_gametime_seconds = 300f; //for 5 minutes (5 min * 60 sec/min)

    public bool isGameOver = false;

    public void Start()
    {
        gameTimer.max_time_in_seconds = max_gametime_seconds;
        gameTimer.reset_timer();

        BudgetPB.progress   = 0f;
        SuccessPB.progress  = 0f;
        TimePB.progress     = 1f;

        BudgetPB.updateProgress();
        SuccessPB.updateProgress();
        TimePB.updateProgress();
    }

    private void Update()
    {
        isGameOver = gameTimer.tick_n_check(Time.deltaTime);

        if (isGameOver)
        {
            Debug.Log("GAMEOVER");
        }
        else
        {
            TimePB.updateProgressByValue(gameTimer.getTimerCompletionPercentage());
        }
    }

    public void updateBudget(float value) // 0 to 1f
    {
        BudgetPB.updateProgressByValue(value);
    }

    public void updateSuccess(float value) // 0 to 1f
    {
        SuccessPB.updateProgressByValue(value);
    }
}
