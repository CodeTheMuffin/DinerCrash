using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject ExitUI;
    public TextSystem txtSys;

    public TMPro.TextMeshProUGUI budget_text;
    public TMPro.TextMeshProUGUI rating_text;//also called success text
    public TMPro.TextMeshProUGUI time_text;

    const string budget_text_start = "Budget: $";
    const string rating_text_start = "Rating: ";
    const string time_text_start = "Time : ";

    public NPC_Manager npc_manager;
    public Timer gameTimer;

    //PB = ProgressBar
    public ProgressBar BudgetPB;
    public ProgressBar SuccessPB;
    public ProgressBar TimePB;

    public float max_gametime_seconds = 300f; //for 5 minutes (5 min * 60 sec/min)

    public bool isGameOver = false;

    //debug
    public float DEBUG_TIMER_VALUE = 0f;

    public void Start()
    {
        gameTimer.max_time_in_seconds = max_gametime_seconds;
        gameTimer.reset_timer();

        BudgetPB.progress = 0f;
        SuccessPB.progress = 0f;
        TimePB.progress = 1f;

        BudgetPB.updateProgress();
        SuccessPB.updateProgress();
        TimePB.updateProgress();

        ExitUI.SetActive(false);
    }

    private void Update()
    {
        if (!isGameOver)
        {
            isGameOver = gameTimer.tick_n_check(Time.deltaTime);

            if (isGameOver)
            {
                GameOver();
            }
            else
            {
                TimePB.updateProgressByValue(gameTimer.getTimerCompletionPercentage());
            }
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

    void GameOver()
    {
        Debug.Log("GAMEOVER");

        MenuUI.SetActive(false);
        ExitUI.SetActive(true);

        //update text

        string time_string = BuffTime(DEBUG_TIMER_VALUE);

        time_text.text = $"{time_text_start}{time_string}";
    }

    string BuffTime(float ending_time)// in seconds
    {
        ending_time = Mathf.Floor(ending_time);//rounds down

        if (ending_time < 0f)
        { ending_time = 0f; }

        int minutes = (int)(ending_time / 60);
        int seconds = (int)(ending_time % 60);

        string time_string = "";

        if (minutes == 0)
        { time_string = "00"; }
        else if (minutes < 10)
        { time_string = $"0{minutes}"; }
        else
        { time_string = $"{minutes}"; }// max min should be 99, but the game will never be that long...
        time_string += " : ";

        if (seconds == 0)
        { time_string += "00"; }
        else if (seconds < 10)
        { time_string += $"0{seconds}"; }
        else { time_string += $"{seconds}"; }

        return time_string;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
