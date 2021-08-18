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
    public static List<string> nothing_keys = new List<string>();
    public static List<string> issues_keys = new List<string>();
    public static List<string> empty_keys = new List<string>();
    public static List<string> exit_keys = new List<string>();

    public static Dictionary<string, string> optionDict = new Dictionary<string, string>();
    public static Dictionary<string, string> requestDict = new Dictionary<string, string>();
    public static Dictionary<string, string> nothingDict = new Dictionary<string, string>();
    public static Dictionary<string, string> issuesDict = new Dictionary<string, string>();
    public static Dictionary<string, string> emptyDict = new Dictionary<string, string>();
    public static Dictionary<string, string> exitDict = new Dictionary<string, string>();

    public const string OPTIONS  = "OPTIONS";
    public const string REQUESTS = "REQUESTS";
    public const string NOTHINGS = "NOTHINGS";
    public const string ISSUES   = "ISSUES";
    public const string EMPTY    = "EMPTY"; // for receiving an empty order box
    public const string EXIT_TEXTS= "EXIT_TEXTS"; // for receiving an satisfied order box
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
            "NOTHINGS",     // On the super rare case, the NPC orders nothing.
            "UPDATES",      // Optional. How the NPC may correct themselves on their requests. Does not have to be used
            "ISSUSES",      // Optional. Text used if the NPC has any issues, such as long wait time or wrong order.
            "EMPTY",        // Optional. What the NPC says once they get an empty order and wasn't expecting it. 
            "EXIT_TEXTS"    // Optional. What the NPC says once they get their order and are leaving satisfied. 
        };

    //if not set, then set ! 
    public void setKeysListsAndDicts()
    {
        SUB_setKeysListsAndDicts(OPTIONS    ,option_keys   ,optionDict);
        SUB_setKeysListsAndDicts(REQUESTS   ,request_keys  ,requestDict);
        SUB_setKeysListsAndDicts(NOTHINGS   ,nothing_keys  ,nothingDict);
        SUB_setKeysListsAndDicts(ISSUES     ,issues_keys   ,issuesDict);
        SUB_setKeysListsAndDicts(EMPTY      ,empty_keys    ,emptyDict);
        SUB_setKeysListsAndDicts(EXIT_TEXTS ,exit_keys     ,exitDict);
    }

    void SUB_setKeysListsAndDicts(string key_node, List<string> key_list, Dictionary<string, string> key_dict)
    {
        if (key_list.Count == 0)
        {
            key_dict.Clear();
            JSONNode.KeyEnumerator keys = NPC_JSON_node[key_node].Keys;

            foreach (JSONNode key in keys)
            {
                string str_key = key;
                key_list.Add(str_key);
                key_dict.Add(str_key, NPC_JSON_node[key_node][str_key][RAW_TEXT]);
            }
        }
    }

    public string generateRequests()
    {
        setKeysListsAndDicts();

        int total_quantity = expectedOrder.getTotalQuantity();
        int total_selected = expectedOrder.getTotalOptionsSelected();


        if (total_quantity == 0) // On the super rare case, the NPC wants nothing
        {
            return generateNothingText();
        }


        int random_request_choice = Random.Range(0, request_keys.Count);
        string random_request_key = request_keys[random_request_choice];
        //print(random_reqeuest_key);
        //print($"Total quantity: {total_quantity}\t Total Selected: {total_selected}");

        // The starting text will contain {0} for quantity} {1} for the option
        string starting_request_text = requestDict[random_request_key];
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
            string request_formatted = get_request_with_option(expectedOrder.counters[option], option_key, starting_request_text);
            request_text += request_formatted;

            // see if text needs to be adjusted
            if (request_formatted.Length > 0)
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

    //for when the NPC super rare, doesn't want anything.
    public string generateNothingText()
    {
        int random_choice = Random.Range(0, nothing_keys.Count);
        string random_key = nothing_keys[random_choice];
        string nothing_text = nothingDict[random_key];

        return nothing_text;
    }

    public string generateRequestsAndFormat()
    {
        string req_text = generateRequests();

        System.Tuple<string, List<string>> output = textSYS.adjustTextRegex(req_text);
        req_text = output.Item1;

        return req_text;
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

    public void updateNPCtext(string formatted_text)
    {
        textSYS.updateNPCtext(formatted_text);
    }

    public void updateSystemText(string text)
    {
        textSYS.updateSystemText(text);
    }

    // MissedItems closely follows OrderOptions.Options enum and correlates to {lang}-npc_text.json's "OPTIONS" values
    public string getMissingText(int[] missedItems)
    {
        // xx and forloop are for lazy debugging. remove later.
        string xx = "";
        foreach (int x in missedItems)
        {
            xx += $"{x}, ";
        }
        print($"Inputted missedItems: {xx}");
        
        string missing_text = "";
        string format = "{0} {1}";
        string option_key = "";

        for (int index = 0; index < missedItems.Length; index++)
        {
            if (missedItems[index] > 0)//if its is missed
            {
                option_key = option_keys[index];
                missing_text += get_request_with_option(missedItems[index], option_key, format);

                if (index + 1 == missedItems.Length) // if last item
                {format = ", and {0} {1}";}
                else
                { format = ", {0} {1}"; }
            }
        }

        return missing_text;
    }

    public string getRandomText(List<string> key_list, Dictionary<string, string> key_dict)
    {
        int random_choice = Random.Range(0, key_list.Count);
        string random_key = key_list[random_choice];

        string response_text = key_dict[random_key];
        return response_text;//unformatted
    }

    //for when the player hands them an empty box...
    public string getEmptyText()
    {
        return textSYS.adjustTextRegex(getRandomText(empty_keys, emptyDict)).Item1;
    }

    //for when the player hands them an order that isn't right...
    public string getIssuesText(string missing_text)// missing_text is what was missing from the order
    {
        string random_issues_text = getRandomText(issues_keys, issuesDict);
        string semi_formatted_text = System.String.Format(random_issues_text, missing_text);

        return textSYS.adjustTextRegex(semi_formatted_text).Item1;
    }

    //for when the player hands them the correct order or better!
    public string getExitTexts()
    {
        return textSYS.adjustTextRegex(getRandomText(exit_keys, exitDict)).Item1;
    }

    /*// BELOW FOR DEBUGGING      COMMENT OUT
    // Need to stop spawning or limit to 1
    public int index = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //string str = optionDict[option_keys[index]];
            //string str = requestDict[request_keys[index]];
            string str = nothingDict[nothing_keys[index]];
            print($"Index: {index}\tstring: {str}");
            System.Tuple<string, List<string>> info = textSYS.adjustTextRegex(str);

            //textSYS.clearEverything();
            textSYS.NPC_string = info.Item1;
            //textSYS.textBox = info.Item2;
            textSYS.update_textbox();

            //index = (index + 1) % option_keys.Count;
            //index = (index + 1) % request_keys.Count;
            index = (index + 1) % nothing_keys.Count;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            int i = 0;
            foreach (string key in request_keys)
            {
                print($"i: {i}\tkey: {key}\tvalue: {requestDict[key]}");
                i++;
            }
        }
    }*/
}
