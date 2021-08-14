using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 This manager handles the spawning, removal and configuration of NPCs.
 */
public class NPC_Manager : MonoBehaviour
{
    public GameObject NPC_prefab;
    public List<NPC> NPCs = new List<NPC>();
    public int max_NPCs_allowed = 5;

    public Timer spawning_timer;
    public float max_spawning_time = 7f;

    public TextSystem text_system;

    public Sprite[] NPC_Sprites;

    public aWayPointSuperManager SuperManager;

    public Collider2D right_wall;
    public Collider2D left_wall;

    Dictionary<string, int> order_options_quantity = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        spawning_timer.max_time_in_seconds = max_spawning_time;
        spawning_timer.reset_timer();
        clearOrderOptions();
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
        if(CountNPCs() <= max_NPCs_allowed)
        {
            bool isTimerDone = spawning_timer.tick_n_check(Time.deltaTime);
            if (isTimerDone)
            {
                spawnNPC();
            }
        }

        foreach (NPC robot in NPCs)
        {
            if (robot.ready_for_next_point)
            {
                robot.currentWayPoint = robot.nextWayPoint;
                Tuple<int, aWayPoint> newPoint = SuperManager.getNextStateAndWayPoint(robot.current_state, robot.currentWayPoint);
                robot.nextWayPoint = newPoint.Item2;
                robot.ready_for_next_point = false;
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

    public void configureOptions()
    {
        // Setting quantity values per option
        clearOrderOptions();

        // using this or directly in foreach loop wouldn't let me modify the dictionary
        //Dictionary<string, int>.KeyCollection keys = order_options_quantity.Keys; 
        List<string> keys = new List<string>(order_options_quantity.Keys);

        foreach (string key in keys )
        {
            int quantity = UnityEngine.Random.Range(0, UI_Manger.MAX_AMOUNT);
            order_options_quantity[key] = quantity;
            //print($"Key: {key} amount: {quantity.ToString()}");
        }

        int cholocate_cookie_counter        = order_options_quantity[keys[0]];
        int oatmeal_raisan_cookie_counter   = order_options_quantity[keys[1]];
        int normal_milk_counter             = order_options_quantity[keys[2]];
        int warm_milk_counter               = order_options_quantity[keys[3]];

        int total = 0;
        bool discount = false;

        // Making the expected order form
        OrderForm expectedOrderForm = new OrderForm(cholocate_cookie_counter, oatmeal_raisan_cookie_counter, normal_milk_counter, warm_milk_counter, total, discount);


        // Determine what sprite to use and the color
        int sprite_index = UnityEngine.Random.Range(0, NPC_Sprites.Length);
        Sprite npc_sprite = NPC_Sprites[sprite_index];

        GameObject npc_game_obj = GameObject.Instantiate(NPC_prefab, SuperManager.getSpawningTransform());

        NPC npc_obj = npc_game_obj.GetComponent<NPC>();
        npc_obj.currentWayPoint = SuperManager.spawningPoint;
        Tuple<int, aWayPoint> newPoint = SuperManager.getNextStateAndWayPoint((int)NPC.State.spawned, npc_obj.currentWayPoint);
        npc_obj.nextWayPoint = newPoint.Item2;

        // prevent NPC colliding with right and left wall
        Physics2D.IgnoreCollision(right_wall, npc_game_obj.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(left_wall , npc_game_obj.GetComponent<Collider2D>());

        npc_obj.justSpawnedHandler();
        //npc_obj.nextWayPoint = SuperManager.
        npc_obj.setSprite(npc_sprite);
        npc_obj.setOrderForm(expectedOrderForm);
        NPCs.Add(npc_obj);
    }
}
