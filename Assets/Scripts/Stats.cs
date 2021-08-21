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

    public float total_expected_points = 0f;
    public float total_actual_points = 0f;
    public float success_rating = 0f;

    public float total_expected_budget = 800f;
    public float total_actual_budget = 0f;
    public float budget_rating = 0f;

    public SpriteRenderer player_reaction;
    public List<Sprite> player_faces;

    //debug
    //public float DEBUG_VALUE = 0f;

    public void Start()
    {
        //Debug.Log($"Inside Stat's Start(): {this.name}");
        gameTimer.max_time_in_seconds = max_gametime_seconds;
        gameTimer.reset_timer();

        BudgetPB.progress = 0f;
        SuccessPB.progress = 0f;
        TimePB.progress = 1f;

        BudgetPB.updateProgress();
        SuccessPB.updateProgress();
        TimePB.updateProgress();

        ExitUI.SetActive(false);
        //Debug.Log($"End of Stat's Start(): {this.name}");
    }

    private void Update()
    {
        if (!isGameOver)
        {
            isGameOver = gameTimer.tick_n_check(Time.deltaTime);

            if (isGameOver)
            {
                Debug.Log("Time ran out!");//use Debug.Log whenever GameOver() is called.
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
        if (value > 1f)
        { value = 1f; }
        else if (value < 0f)
        { value = 0f; }

        SuccessPB.updateProgressByValue(value);
    }

    public void updateRatingActual_VS_Expected(int actual, int expected)
    {
        total_actual_points += actual;
        total_expected_points += expected;

        success_rating = total_actual_points / total_expected_points;

        //Debug.Log($"new actual: {actual} new expected: {expected}");
        //Debug.Log($"total actual: {total_actual_points} total expected: {total_expected_points}");

        updateSuccess(success_rating);
    }

    public void updateBudgetActual(int actual)
    {
        total_actual_budget += (float)actual;

        budget_rating = total_actual_budget / total_expected_budget;

        //Debug.Log($"new actual: {actual}");
        //Debug.Log($"total actual: {total_actual_budget} total expected: {total_expected_budget} new budget rating: {budget_rating}");

        updateBudget(budget_rating);
    }

    void updatePlayerReactionFaces()
    {
        /*
         0 = neutral
         1 = great!
         2 = bad
         */

        int face_index = 0;

        if (budget_rating >= 0.8f && success_rating >= 0.8f)
        {
            face_index = 1;
        }
        else if (budget_rating <0.3f && success_rating < 0.3f)
        {
            face_index = 2;
        }

        player_reaction.sprite = player_faces[face_index];
    }    

    public void GameOver()
    {
        Debug.Log("GAMEOVER");

        MenuUI.SetActive(false);
        ExitUI.SetActive(true);

        // UPDATING BUDGET TEXT
        budget_text.text = $"{budget_text_start}{Mathf.RoundToInt(total_actual_budget)}";

        // UPDATING RATING TEXT
        rating_text.text = $"{rating_text_start}{Mathf.RoundToInt(success_rating * 100)}%";

        // UPDATING TIME TEXT
        string time_string = BuffTime(gameTimer.time);   //BuffTime(DEBUG_VALUE);//DEBUG
        time_text.text = $"{time_text_start}{time_string}";

        updatePlayerReactionFaces();
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
