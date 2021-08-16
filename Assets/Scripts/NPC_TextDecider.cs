using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;

// To be used for known options and predetermined text, and behavioral text based on environment variables.
public class NPC_TextDecider : MonoBehaviour
{
    public TextSystem textSYS;
    JSONNode NPC_JSON_node; // from textSYS
    public OrderForm expectedOrder;

    public static List<string> option_keys = new List<string>();
    public static List<string> request_keys = new List<string>();

    public static Dictionary<string, string> optionDict = new Dictionary<string, string>();
    public static Dictionary<string, string> requestDict = new Dictionary<string, string>();

    public const string OPTIONS  = "OPTIONS";
    public const string REQUESTS = "REQUESTS";
    public const string RAW_TEXT = "RAW_TEXT";


    public void SetValues(TextSystem txt, OrderForm form)
    {
        textSYS = txt;
        NPC_JSON_node = textSYS.get_NPC_JSON();
        expectedOrder = form;
    }

    // This should match {lang}-npc_text.json file
    public static List<string> NPC_text_keys = new List<string>
        {
            "OPTIONS",      // The various options to order from (also based on expectedOrder)
            "REQUESTS",     // How the NPC will make the order
            "UPDATES",      // Optional. How the NPC may correct themselves on their requests. Does not have to be used
            "ISSUSES",      // Optional. Text used if the NPC has any issues, such as long wait time or wrong order.
            "EXIT_TEXTS"    // Optional. What the NPC says once they get their order and are leaving. 
        };

    //if not set, then set ! 
    public void setKeysListsAndDicts()
    {
        if (option_keys.Count == 0)
        {
            optionDict.Clear();
            JSONNode.KeyEnumerator keys = NPC_JSON_node[OPTIONS].Keys;

            foreach (JSONNode key in keys)
            {
                string str_key = key;
                option_keys.Add(str_key);
                optionDict.Add(str_key, NPC_JSON_node[OPTIONS][str_key][RAW_TEXT]);
            }
        }

        if (request_keys.Count == 0)
        {
            requestDict.Clear();
            JSONNode.KeyEnumerator keys = NPC_JSON_node[REQUESTS].Keys;

            foreach (JSONNode key in keys)
            {
                string str_key = key;
                request_keys.Add(str_key);
                requestDict.Add(str_key, NPC_JSON_node[REQUESTS][str_key][RAW_TEXT]);
            }
        }
    }

    public string generateRequests()
    {
        setKeysListsAndDicts();

        int total_quantity = expectedOrder.getTotalQuantity();
        int total_selected = expectedOrder.getTotalOptionsSelected();

        int random_request_choice = Random.Range(0, request_keys.Count - 1);
        string random_reqeuest_key = request_keys[random_request_choice];

        // The starting text will contain {0} for quantity} {1} for the option
        string starting_request_text = requestDict[random_reqeuest_key];
        string request_text = "";

        int selected = 0;

        string option_key = "";


        /*      
        //cholocate_cookie
        option_key = option_keys[(int)OrderOptions.Options.cholocate_cookie];
        request_text = get_request_with_option(expectedOrder.cholocate_cookie_counter, option_key, starting_request_text);



        // see if text needs to be adjusted
        if (request_text.Length> 0)
        {
            selected++;

            // last selected
            if (selected + 1 == total_selected) {   starting_request_text = ", and {0} {1}";   }
            // if there are more options
            else {   starting_request_text = ", {0} {1}";    }
        }

        //oatmeal_raisin_cookie
        option_key = option_keys[(int)OrderOptions.Options.oatmeal_raisin_cookie];
        request_text = get_request_with_option(expectedOrder.oatmeal_raisin_cookie_counter, option_key, starting_request_text);
        */



        // option and expectedOrder.counters are tied to the OrderOptions.Options enum
        for (int option = 0; option < expectedOrder.counters.Length; option++)
        {
            //option should represent values in OrderOption.Options enum.  example: (int)OrderOptions.Options.cholocate_cookie
            // and OrderForm's counter, which is also associated to OrderOption.Options enum
            option_key = option_keys[option];
            request_text += get_request_with_option(expectedOrder.counters[option], option_key, starting_request_text);

            // see if text needs to be adjusted
            if (request_text.Length > 0)
            {
                selected++;

                // last selected
                if (selected + 1 == total_selected) { starting_request_text = ", and {0} {1}"; }
                // if there are more options
                else { starting_request_text = ", {0} {1}"; }

                if (selected == total_selected)
                { break; }
            }
        }

        return request_text + ".";
    }

    public string get_request_with_option(int quantity, string option_key, string formatted_text)
    {
        string s = "";

        if (quantity > 0)
        {
            s = System.String.Format(formatted_text, quantity, optionDict[option_key]);

            if (quantity > 1) // make plural
            { s += "s"; }
        }

        return s;
    }
}
