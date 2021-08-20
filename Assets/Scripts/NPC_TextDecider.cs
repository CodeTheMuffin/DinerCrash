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
    public static List<string> repeats_keys = new List<string>();
    public static List<string> issues_keys = new List<string>();
    public static List<string> empty_keys = new List<string>();
    public static List<string> exit_keys = new List<string>();

    public static Dictionary<string, string> optionDict = new Dictionary<string, string>();
    public static Dictionary<string, string> requestDict = new Dictionary<string, string>();
    public static Dictionary<string, string> nothingDict = new Dictionary<string, string>();
    public static Dictionary<string, string> repeatsDict = new Dictionary<string, string>();
    public static Dictionary<string, string> issuesDict = new Dictionary<string, string>();
    public static Dictionary<string, string> emptyDict = new Dictionary<string, string>();
    public static Dictionary<string, string> exitDict = new Dictionary<string, string>();

    public const string OPTIONS  = "OPTIONS";
    public const string REQUESTS = "REQUESTS";
    public const string NOTHINGS = "NOTHINGS";
    public const string REPEATS  = "REPEATS";
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
        SUB_setKeysListsAndDicts(REPEATS    ,repeats_keys  ,repeatsDict);
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


        //int random_request_choice = Random.Range(0, request_keys.Count);
        //string random_request_key = request_keys[random_request_choice];
        //print(random_reqeuest_key);
        //print($"Total quantity: {total_quantity}\t Total Selected: {total_selected}");


        // The starting text will contain {0} for quantity} {1} for the option
        //string starting_request_text = requestDict[random_request_key];
        //formatted_text_pattern was called starting_request_text
        string formatted_text_pattern = getRandomText(request_keys, requestDict);


        /*string request_text = "";

        int selected = 0;

        string option_key = "";


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
        }*/

        string request_text = get_substituted_text_with_pattern(formatted_text_pattern);

        return request_text + ".";
    }

    // requires formatted_text_pattern to be in {0} {1} format.
    // probably should rename function, but it returns the selected order items (aka non Zero quantity) from expected Order
    // method should probably be in the OrderForm class TBH, but not enough time!
    string get_substituted_text_with_pattern(string formatted_text_pattern)
    {
        string formatted_text = "";
        int total_selected = expectedOrder.getTotalOptionsSelected();
        int selected = 0;

        // option and expectedOrder.counters are tied to the OrderOptions.Options enum
        for (int option = 0; option < expectedOrder.counters.Length; option++)
        {
            //option should represent values in OrderOption.Options enum.  example: (int)OrderOptions.Options.cholocate_cookie
            // and OrderForm's counter, which is also associated to OrderOption.Options enum
            string option_key = option_keys[option];
            string request_formatted = get_request_with_option(expectedOrder.counters[option], option_key, formatted_text_pattern);
            formatted_text += request_formatted;

            // see if text needs to be adjusted
            if (request_formatted.Length > 0)
            {
                selected++;

                // last selected
                if (selected + 1 == total_selected) { formatted_text_pattern = ", and {0} {1}"; }
                // if there are more options
                else { formatted_text_pattern = ", {0} {1}"; }

                if (selected == total_selected)
                { break; }
            }
        }

        return formatted_text;
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

    // formatted_text_pattern MUST contain {0} and {1}
    public string get_request_with_option(int quantity, string option_key, string formatted_text_pattern)
    {
        string s = "";

        if (quantity > 0)
        {
            s = System.String.Format(formatted_text_pattern, quantity, optionDict[option_key]);

            if (quantity > 1) // make plural
            { s += "s"; }
        }

        return s;
    }

    public void updateNPCtext_unformatted(string unformatted_text)
    {
        textSYS.updateNPCtext_unformatted(unformatted_text);
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
    public string getMissingText(int[] missedItems, int[] form_counters, int total_selected)
    {
        // xx and forloop are for lazy debugging. remove later.
        string xx = "";
        foreach (int x in missedItems)
        {
            xx += $"{x}, ";
        }
        //print($"Inputted missedItems: {xx}");
        
        string missing_text = "";
        string format = "{0} {1}";
        string option_key = "";
        int selected = 0;

        for (int index = 0; index < missedItems.Length; index++)
        {
            if (form_counters[index] > 0)
            { selected++; }

            if (missedItems[index] > 0 )//if its is missed
            {
                option_key = option_keys[index];
                missing_text += get_request_with_option(missedItems[index], option_key, format);

                //if (index + 1 == missedItems.Length) // if last item
                if (selected + 1 == total_selected) // if last item
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
    public string getRepeatText()
    {
        string random_text = getRandomText(repeats_keys, repeatsDict); // this should contian only {0}
        string simple_format = "{0} {1}";

        string format_text_pattern = get_substituted_text_with_pattern(simple_format);//this requires {0} and {1}

        string semi_formatted_text = System.String.Format(random_text, format_text_pattern);//now combine them!

        return textSYS.adjustTextRegex(semi_formatted_text).Item1;
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
