using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 This manager handles the spawning, removal and configuration of NPCs.
 */
public class NPC_Manager : MonoBehaviour
{
    public Stats GameStats;
    public AudioManager audio_manager;
    public GameObject NPC_prefab;
    public List<NPC> NPCs = new List<NPC>();
    public List<NPC> DyingNPCs = new List<NPC>();
    public int max_NPCs_allowed = 7; // this at a time in a room.
    public int total_NPCs_allowed_in_game = 50;//total number of allowed NPCs during a game run

    public Timer spawning_timer;
    public float max_spawning_time = 7f;
    public int spawn_counter = 0;

    public TextSystem text_system;

    public Sprite[] NPC_Sprites;

    public TextSystem textSYS;
    public aWayPointSuperManager SuperManager;
    aWayPoint orderWayPoint;

    public Collider2D right_wall;
    public Collider2D left_wall;

    public Transform AreaToDie;

    Dictionary<string, int> order_options_quantity = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        spawning_timer.max_time_in_seconds = max_spawning_time;
        //spawning_timer.reset_timer();
        clearOrderOptions();
        orderWayPoint = SuperManager.getOrderingWayPoint();
    }

    public void clearOrderOptions()
    {
        foreach (string key in OrderOptions.option_names_to_index.Keys)
        {
            if (order_options_quantity.ContainsKey(key))
            {
                order_options_quantity[key] = 0;
            }
            else
            {
                order_options_quantity.Add(key, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStats.isGameOver)
        {


            if (CountNPCs() < max_NPCs_allowed && spawn_counter < total_NPCs_allowed_in_game)
            {
                bool isTimerDone = spawning_timer.tick_n_check(Time.deltaTime);
                if (isTimerDone)
                {
                    spawnNPC();
                }
            }
            else if (CountNPCs() == 0 && spawn_counter >= total_NPCs_allowed_in_game)
            {
                textSYS.updateSystemText($"Max NPC counter of {total_NPCs_allowed_in_game} reached.");
                Debug.Log("MAX NPCS allowed in game reached. Ending game.");
                GameStats.isGameOver = true;
                GameStats.GameOver();
                return;
            }

            //handling robots NPCs
            foreach (NPC robot in NPCs)
            {
                if (robot.current_state == (int)NPC.State.dying)
                {
                    DyingNPCs.Add(robot);
                    //Repositioned so it can get out of last WayPoint collider and be removed from memory
                    robot.transform.position = AreaToDie.position;
                    continue;
                }

                robot.updateProgressbar(Time.deltaTime);

                if (robot.ready_for_next_point)
                {
                    robot.currentWayPoint = robot.nextWayPoint;
                    Tuple<int, aWayPoint> newPoint = SuperManager.getNextStateAndWayPoint(robot.current_state, robot.currentWayPoint);
                    int new_state = newPoint.Item1;

                    if (new_state == (int)NPC.State.standing)
                    {
                        robot.standingWayPoint = newPoint.Item2;
                    }

                    robot.current_state = new_state;
                    robot.nextWayPoint = newPoint.Item2;
                    robot.ready_for_next_point = false;
                }
                else if (robot.need_new_standing_point)
                {
                    aWayPoint newPoint = SuperManager.getRandomStandingWayPoint();
                    if (newPoint != null)
                    {
                        robot.nextWayPoint = newPoint;
                    }
                    robot.need_new_standing_point = false;
                    robot.standingWayPoint = newPoint;
                }
            }

            if (DyingNPCs.Count > 0)
            {
                foreach (NPC deadNPC in DyingNPCs)
                {
                    NPCs.Remove(deadNPC);
                    Destroy(deadNPC.gameObject, 2f);//Destroy in 2 seconds
                }
                DyingNPCs.Clear();
            }
        }
    }

    public int CountNPCs()
    { return NPCs.Count;}

    public void spawnNPC()
    {
        spawning_timer.reset_timer();
        configureOptions();
    }

    OrderForm createRandomExpectedOrderForm()
    {
        // using this or directly in foreach loop wouldn't let me modify the dictionary
        //Dictionary<string, int>.KeyCollection keys = order_options_quantity.Keys; 
        List<string> keys = new List<string>(order_options_quantity.Keys);

        foreach (string key in keys)
        {
            //int max_value = UnityEngine.Random.Range(3, UI_Manger.MAX_AMOUNT);
            //int quantity = UnityEngine.Random.Range(0, max_value);//UI_Manger.MAX_AMOUNT);
            int quantity = UnityEngine.Random.Range(0, UI_Manger.MAX_AMOUNT);
            order_options_quantity[key] = quantity;
            //print($"Key: {key} amount: {quantity.ToString()}");
        }

        int cholocate_cookie_counter = order_options_quantity[keys[0]];
        int oatmeal_raisin_cookie_counter = order_options_quantity[keys[1]];
        int normal_milk_counter = order_options_quantity[keys[2]];
        int warm_milk_counter = order_options_quantity[keys[3]];

        int total = 0;
        bool discount = false;

        // Making the expected order form
        OrderForm expectedOrderForm = new OrderForm(cholocate_cookie_counter, oatmeal_raisin_cookie_counter, normal_milk_counter, warm_milk_counter, total, discount);
        return expectedOrderForm;
    }

    public void configureOptions()
    {
        // Setting quantity values per option
        clearOrderOptions();

        OrderForm expectedOrderForm = createRandomExpectedOrderForm();

        // Determine what sprite to use and the color
        int sprite_index = UnityEngine.Random.Range(0, NPC_Sprites.Length);
        Sprite npc_sprite = NPC_Sprites[sprite_index];

        GameObject npc_game_obj = GameObject.Instantiate(NPC_prefab, SuperManager.getSpawningTransform());

        NPC npc_obj = npc_game_obj.GetComponent<NPC>();
        npc_obj.currentWayPoint = SuperManager.spawningPoint;
        Tuple<int, aWayPoint> newPoint = SuperManager.getNextStateAndWayPoint((int)NPC.State.spawned, npc_obj.currentWayPoint);
        npc_obj.nextWayPoint = newPoint.Item2;
        npc_obj.orderingWayPoint = orderWayPoint;

        // prevent NPC colliding with right and left wall for spawning and exitting/deletion
        Physics2D.IgnoreCollision(right_wall, npc_game_obj.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(left_wall , npc_game_obj.GetComponent<Collider2D>());

        
        npc_obj.setSprite(npc_sprite);
        npc_obj.setOrderForm(expectedOrderForm);
        npc_obj.text_decider.SetValues(textSYS, expectedOrderForm);
        npc_obj.GameStats = GameStats;
        npc_obj.audio_src = audio_manager;
        npc_obj.justSpawnedHandler();

        NPCs.Add(npc_obj);
        spawn_counter++;
    }
}
