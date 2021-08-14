using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 This manager handles the spawning, removal and configuration of NPCs.
 */
public class NPC_Manager : MonoBehaviour
{
    public GameObject NPC_prefab;
    public List<NPC> NPCs = new List<NPC>();
    public int max_NPCs_allowed = 5;

    public Timer spawning_timer;

    [SerializeField]
    private float max_spawning_time = 3f;

    public TextSystem text_system;

    public Sprite[] NPC_Sprites;

    public aWayPointSuperManager SuperManager;

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
            int quantity = Random.Range(0, UI_Manger.MAX_AMOUNT);
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
        int sprite_index = Random.Range(0, NPC_Sprites.Length);
        Sprite npc_sprite = NPC_Sprites[sprite_index];

        print("jere");
        GameObject npc_game_obj = GameObject.Instantiate(NPC_prefab, SuperManager.getSpawningTransform());
        print("Done");

        NPC npc_obj = npc_game_obj.GetComponent<NPC>();
        npc_obj.setSprite(npc_sprite);
        npc_obj.setOrderForm(expectedOrderForm);
    }
}
