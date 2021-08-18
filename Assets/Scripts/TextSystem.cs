using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class TextSystem : MonoBehaviour
{
    const int MAX_CHARS_PER_ROW = 12; // The max number of chars per row;       was 10
    const int MAX_ROWS = 2; // X rows to show on screen at a time

    public TMPro.TextMeshProUGUI chat_box_text;
    public Button scroll_UP_button;
    public Button scroll_DOWN_button;

    public List<string> textBox;
    int scroll_page = 0;

    const char NEWLINE = '`';// If this is present, then place a new line in string.

    public string warning_string = "";
    public JSONNode warningJSON;

    public string NPC_string = "";
    public JSONNode NPC_JSON;

    public string system_string = "";
    public JSONNode system_JSON;

    string buffer_line = "";

    //used as keys in JSON Nodes
    public const string RAW_TEXT = "RAW_TEXT";
    public const string FORMATTED_TEXT = "FORMATTED_TEXT";
    public const string TEXTBOX_TEXT = "TEXTBOX_TEXT";

    public void Start()
    {
        textBox = new List<string>();

        //string xx = "Quick test\nfun!";
        NPC_string = "Welcome to Diner Crash!";
        NPC_string = adjustTextRegex(NPC_string).Item1;

        //Tuple<string,List<string>> result = adjustTextRegex(xx);
        //NPC_string = result.Item1;

        //update_text(result.Item1);
        //textBox.AddRange(result.Item2);
        update_textbox();

        buffer_line = fillLineBuffer(MAX_CHARS_PER_ROW, "-");

        //must not contain extension!
        string jsontext = LoadJSON("Text/en-warning_text");
        warningJSON = JSON.Parse(jsontext);

        jsontext = LoadJSON("Text/en-npc_text");
        NPC_JSON = JSON.Parse(jsontext);

        //Need to keep for warning methods()
        ProcessJSONs(); // suppose to help safe some time, but TBH may not be worth the initial loading time... 

        adjust_scroll_availability();
    }

    /*public string LoadJSON(string json_path)
    {
        TextAsset jsonPath = Resources.Load<TextAsset>(json_path);

        string json = "";
        using (StreamReader r = new StreamReader(json_path))
        {
            json = r.ReadToEnd();
            //List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
        }
        return json;
    }*/

    public string LoadJSON(string json_path)
    {
        // load file from the path
        //TextAsset warningjsonPath = Resources.Load<TextAsset>("Text/en-warning_text");
        //string json = LoadJSON(warningjsonPath);

        // This MUST be in folder: Asset/Resources/ and the JSON path must NOT include extension (.json)
        TextAsset jsontext = Resources.Load<TextAsset>(json_path);
        return jsontext ? jsontext.text : "";
    }

    /*
     For each Node, add keys/values for formatted text for faster processing in future(???)
     */
    public void ProcessJSONs()
    {
        //print($"{warningJSON.GetType().ToString()}");

        foreach (KeyValuePair<string, JSONNode> kvp in warningJSON)
        {
            string raw_text = kvp.Value[RAW_TEXT];
            Tuple<string, List<string>> adjustedText = adjustTextRegex(raw_text);

            warningJSON[kvp.Key][FORMATTED_TEXT] = adjustedText.Item1;
            warningJSON[kvp.Key][TEXTBOX_TEXT] = adjustedText.Item2;

            //print($"{kvp.GetType().ToString()}");
            //print($"Key: {kvp.Key} {kvp.Key.GetType().ToString()} and Value {kvp.Value} type: {kvp.Value.GetType().ToString()}");
        }



        /*foreach (KeyValuePair<string, JSONNode> kvp in NPC_JSON)
        {
            //each kvp.Value is a another set of key value pairs
            *//*foreach (KeyValuePair<string, JSONNode> keys in kvp.Value)
            {
                print($"{keys.GetType().ToString()}");
                print($"Key: {keys.Key} Value {keys.Value} type: {keys.Value.GetType().ToString()}");
            }*//*

            foreach (JSONObject child in kvp.Value.DeepChildren)
            {
                print($"{child.GetType().ToString()}");
                *//*print($"Key: {child.Key} Value {keys.Value} type: {keys.Value.GetType().ToString()}");*//*
            }
        }*/

        recursiveNPC(NPC_JSON);
    }

    public void recursiveNPC(JSONNode node)
    {
        if (node.HasKey(RAW_TEXT))
        {
            string raw_text = node[RAW_TEXT];
            Tuple<string, List<string>> adjustedText = adjustTextRegex(raw_text);

            node[FORMATTED_TEXT] = adjustedText.Item1;
            //node[TEXTBOX_TEXT] = adjustedText.Item2;
        }
        else
        {
            if (node.IsString)
                return;

            //assumes JSON object
            foreach (JSONNode child in node.Children)
            {
                recursiveNPC(child);
            }
        }
    }

    public void clearEverything()
    {
        textBox.Clear();
        scroll_page = 0;

        warning_string = "";
        warningJSON.Clear();

        NPC_string = "";
        NPC_JSON.Clear();

        //system_string = "";
        //system_JSON.Clear();

        update_text("");
    }

    public JSONNode get_NPC_JSON()
    { return NPC_JSON; }

    // excludes the spaces
    public string[] getAllWords(string text)
    {
        //char[] delimiters = {' '};
        string pattern = "(\n)|(\t)| "; // put character in () if you want to consider it as a seperate word. Use vertical pipe | as an OR statement
        string[] results = Regex.Split(text, pattern);
        //string[] results_delimited = text.Split(delimiters);

        // exclude blank elements
        string[] final_results = (from word in results where (word != "" && word != null) select word).ToArray();

        return final_results;
    }

    public static string excludeRegexFromText(string text)
    {
        string[] textToIgnore = new string[] { "<color=\".*\">(.*)</color>", "<color=\".*\">", "</color>" };

        foreach (string s in textToIgnore)
        {
            //print("\t\ts: " + s);
            //print("\t\ttext: " + text);
            /*Match m = Regex.Match(text,s);
            if(m.Success)
            {
                print("\tm: "+m);
                int index = text.IndexOf(m.ToString());
                print("\tindex: "+ index.ToString());
                text = text.Substring(index);
            }*/
            string[] results = Regex.Split(text, s);
            string t = "";

            foreach (string r in results)
            {
                //print("\tr: " + r + "\tlen: " + r.Length.ToString());
                t += r; // will even add empty strings. which is okay. the Split excludes my searched regex pattern!
            }
            text = t;
        }

        return text;
    }

    public string adjustText(string text)
    {
        string complete_string = "";

        int text_length = text.Length;
        int first_index = 0;
        int second_index = 0;

        int max_interations = (int)Mathf.Ceil((float)text_length / MAX_CHARS_PER_ROW);

        textBox.Clear();

        for (int a = 0; a < max_interations; a++)
        {
            first_index = a * MAX_CHARS_PER_ROW;
            second_index = first_index + MAX_CHARS_PER_ROW;

            if (second_index < text_length)
            {
                // substring( index, length) 
                // NOTTTT substring(index, index + n)
                string substring = text.Substring(first_index, MAX_CHARS_PER_ROW)+"\n";
                textBox.Add(substring);
                complete_string += substring;
            }
            else
            {
                string substring = text.Substring(first_index);
                textBox.Add(substring);
                complete_string += substring;
            }
        }

        return complete_string;
    }

    // No rich text aka coloring or special formatting
    public string adjustTextWithoutConsideringRichText(string text)
    {
        StringBuilder strLine = new StringBuilder();

        textBox.Clear();
        int char_counter = 0;
        bool last_char_was_space = false;

        foreach (char c in text)
        {
            if (!last_char_was_space)
            {
                if (c == ' ')
                {
                    last_char_was_space = true;
                }

                strLine.Append(c);
                char_counter++;
            }
            else { 
                //add buffer, but does not consider next space...
            }

            if (char_counter == MAX_CHARS_PER_ROW)
            {
                textBox.Add(strLine.ToString()) ;
                strLine.Clear();
                char_counter = 0;
            }

        }

        return "";
    }

    // WITHOUT REGEX text, break down the subword and seperate by hyphens if longer than MAX_CHARS_PER_ROW
    public static List<string> breakDownFurtherSubWordOnly(string subword)
    {
        List<string> breakDown = new List<string>();

        if (subword.Length <= MAX_CHARS_PER_ROW)
        {
            breakDown.Add(subword);
        }
        else
        {
            int max_iterations = (int)Mathf.Ceil((float)subword.Length / (MAX_CHARS_PER_ROW - 1));

            for (int iter = 0; iter < max_iterations; iter++)
            {
                int i = iter * (MAX_CHARS_PER_ROW - 1);
                int len = MAX_CHARS_PER_ROW - 1;

                if (i + len >= subword.Length) // so the substring can grab the rest of the substring
                {
                    len = subword.Length - i; // or len = -1??
                }

                string sub = subword.Substring(i, len);

                if (iter + 1 < max_iterations) // if this isn't the last one
                {
                    sub += "-";
                }

                breakDown.Add(sub);
            }
        }


        return breakDown;
    }

    //public static List<string> breakDownWordWithRegex(string text)
    public static List<Tuple<string, int>> breakDownWordWithRegex(string text)
    {
        //string[] colorRegexTextToIgnore = new string[] { "<color=\".*\">(.*)</color>", "<color=\".*\">", "</color>" };
        string[] colorRegexTextToIgnore = new string[] { "<color=.*>(.*)</color>", "<color=\".*\">", "</color>" };
        string subword = text;
        List<string> breakDown = new List<string>();

        // Tuple (full word, length of the parts of the word excluding regex)
        List<Tuple<string, int>> breakDownTuple = new List<Tuple<string, int>>();

        Match match_both = Regex.Match(text, colorRegexTextToIgnore[0]);
        Match match_left = Regex.Match(text, colorRegexTextToIgnore[1]);
        Match match_right = Regex.Match(text, colorRegexTextToIgnore[2]);

        string leftText = "";
        string rightText = "";

        if (match_both.Success)
        {
            subword = match_both.Groups[1].Value;
            int index = text.IndexOf(subword);

            leftText = text.Substring(0, index);
            rightText = text.Substring(index + subword.Length);
        }
        else if (match_left.Success)
        {
            leftText = match_left.Groups[0].Value;

            int index = text.IndexOf(leftText);

            if (index == 0) // somehow regex is on left side of text
            {
                if (subword == text)
                { subword = text.Substring(leftText.Length); }
            }
            else if (index > 0) // regex on right side of text
            {
                if (subword == text)
                { subword = text.Substring(0, index); }
            }

            //subword = text.Substring(left_side.Length);
        }
        else if (match_right.Success)
        {
            rightText = match_right.Groups[0].Value;
            int index = text.IndexOf(rightText);


            if (index == 0) // somehow regex is on left side of text
            {
                if (subword == text)
                { subword = text.Substring(rightText.Length); }
            }
            else if (index > 0) // regex on right side of text
            {
                if (subword == text)
                { subword = text.Substring(0, index); }
            }
        }

        if (subword == text)
        {
            breakDown.Add(text);
            breakDownTuple.Add(Tuple.Create(text, text.Length));
        }
        else
        {
            /*if(leftText != null && leftText != "")
            {
                breakDown.Add(leftText);
            }*/

            if (subword != null && subword != "")
            {
                //breakDown.Add(subword);
                /*
                THIS WORKS
                foreach(string broken_down in breakDownFurtherSubWordOnly(subword))
                {
                    breakDown.Add(broken_down);
                }*/

                List<string> results = breakDownFurtherSubWordOnly(subword);

                for (int i = 0; i < results.Count; i++)
                {
                    string addMe = results[i];

                    if (i == 0 && leftText != null && leftText != "")
                    {
                        addMe = leftText + addMe;
                    }

                    if (i + 1 >= results.Count && rightText != null && rightText != "")
                    {
                        addMe = addMe + rightText;
                    }
                    breakDown.Add(addMe);
                    breakDownTuple.Add(Tuple.Create(addMe, results[i].Length));
                }

            }

            /*if(rightText != null && rightText != "")
            {
                breakDown.Add(rightText);
            }*/
        }

        return breakDownTuple;
    }

    //Returns a tuple(string, [string]) where the string contians the full string
    // and the [string] is intended for the textBox to be used in the UI chatbox
    public Tuple<string, List<string>> adjustTextRegex(string text)
    {
        /*MatchCollection allSpaces = Regex.Matches(text, " ");

        foreach (Match m in allSpaces)
        {
            print("found one: " + m);
        }*/


        /*
         Breakdown on how this works::
         ===============================
         1. Start out with an argument string called 'text'
         
         2. Grab all words from 'text' into a list of strings called 'allWords' (a word is considered anything with a space after it)
         
         3. Go through each word and determine if it contains regex. If it does, determine the subword (part of string without regex) and gets it length.
                If no regex, then subword is the word itself, along with the subword's length is the word's lenth.
                Tuple: (full word with regex (if applicable), subword's length)
                Call this list of tuples as 'word_textbox'
         
         4. Go through each list of tuples and determine if it can be grouped into one row where the sum of all subword's length of that row is <= MAX_CHARS_PER_ROW
                Considers adding a space tuple at the end of adding each word to the list
                Each element/item in the outer list represents a row of text that will be displayed on the chatbox
                Call this lists of lists of tuples as 'complete_textbox'
         
         5. Go to each row of 'complete_textbox' and convert it to a string! Constantly appending the compiled string to a list of strings called 'textBoxFinal' 
                and the full string with newline char '\n' at end of each row called 'completeTextFinal'.
                If last row, exclude the newline char '\n' of last row and exclude the space char if its the last char.
         
         6. Return a Tuple of the complete text and a list of the complete text for the chatbox
                Tuple : (completeTextFinal , textBoxFinal)
         
         Diagram of breakdown                                                                                   Example
         =====================================================================================================================================
         1.         text                                                   |        1.  "<color=\"red\">Warning:</color> Your hands are full!"
                      |                                                    |                                        |
                      v                                                                                             v
         2.  [word1, word2, word3, ...]                                             2.  ["<color=\"red\">Warning:</color>", "Your", "hands", "are", "full!"]
                      |                                                                                             |
                      v                                                                                             v
         3.  [ (word1, subword1.len), (word2, subword2.len),                        3.  [("<color=\"red\">Warning:</color>", 8), ("Your", 4), ("hands", 5), 
                                        (word3, subword3.len), ... ]                                                    ("are", 3), ("full!", 5)] 
                      |                                                                                             |
                      v                                                                                             v 
         4. [ [(word1, subword1.len), (" ", 1), (word2, subword2.len)],             4. [ [("<color=\"red\">Warning:</color>", 8), (" ", 1)], 
                                [(word3, subword3.len)], ...]                                          [("Your", 4), (" ",1), ("hands", 5)],
                                                                                              [("are", 3), (" ", 1), ("full!", 5)] ]
                      |                                                                                             |
                      v                                                                                             v
         5. completeTextFinal: "word1 word2\nword3"                                 5. completeTextFinal: "<color=\"red\">Warning:</color> \nYour hands\nare full!"
            textBoxFinal: ["word1 word2", "word3"]                                      textBoxFinal: ["<color=\"red\">Warning:</color> ", "Your hands ", "are full!"]
                      |                                                                                             |
                      v                                                                                             v
         6. Return (completeTextFinal, textBoxFinal) as tuple                       6. Return (completeTextFinal, textBoxFinal) as tuple
         */

        // Step 1 and Step 2.
        string[] allWords = getAllWords(text);

        List<List<Tuple<string, int>>> complete_textbox = new List<List<Tuple<string, int>>>();
        List<Tuple<string, int>> word_textbox = new List<Tuple<string, int>>();


        // convert the list of strings (words) to a list of tuples (where the string is the full word, int is the length of the string without regex)
        //Step 3
        word_textbox = convertWordsToComplexWordsList(allWords);
        //Step 4
        complete_textbox = convertComplexWordsListToAListOfComplexWordsLists(word_textbox);


        // Now convert that complex lists of lists into a list of strings and a long string with "\n" for each line
        // Excludes last space char and newline char
        // AND each list/row should already include spaces
        // Step 5
        Tuple<string,List<string>> finalStrings=  convertListOfTuplesToString(complete_textbox);

        string completeTextFinal = finalStrings.Item1;
        List<string> textBoxFinal = finalStrings.Item2;
        
        //Step 6
        return Tuple.Create(completeTextFinal, textBoxFinal);
    }

    public string convertListOfTuplesToString(List<Tuple<string, int>> row_list)
    {
        List<string> all_strings = (from aTuple in row_list select aTuple.Item1).ToList();
        string complete_string = string.Join("", all_strings);
        return complete_string;
    }

    public Tuple<string, List<String>> convertListOfTuplesToString(List<List<Tuple<string, int>>> complete_textbox)
    {
        int completeTextLength = complete_textbox.Count;
        List<string> textBoxFinal = new List<string>();
        string completeTextFinal = "";

        for (int index = 0; index < completeTextLength; index++)
        {
            string completeLineText = convertListOfTuplesToString(complete_textbox[index]);

            //If NOT last row, then add new line
            // don't want to add in front of empty string (first index)

            if (index > 0)
            {
                // Since this statement occurs BEFORE adding the word, add newline to seperate rows
                completeTextFinal = completeTextFinal + "\n";

                if (index + 1 == completeTextLength)// if last row
                {
                    //if last char is a space, remove it.
                    if (completeLineText.Substring(completeLineText.Length - 1) == " ")
                    {
                        completeLineText = completeLineText.Substring(0, completeLineText.Length - 1);
                    }
                }
            }

            textBoxFinal.Add(completeLineText);
            completeTextFinal += completeLineText;
        }

        return Tuple.Create(completeTextFinal, textBoxFinal);
    }

    // Last list of lists of tuples might consists of an unneeded spaceTuple!!
    public List<List<Tuple<string, int>>> convertComplexWordsListToAListOfComplexWordsLists(List<Tuple<string, int>> word_textbox)
    {
        List<List<Tuple<string, int>>> complete_textbox = new List<List<Tuple<string, int>>>();
        int complete_textbox_index = 1; // subtract 1 to get actual index; done this as a counting trick for lists

        Tuple<string, int> spaceTuple = Tuple.Create(" ", 1);
        Tuple<string, int> newlineTuple = Tuple.Create("\n", 1);
        Tuple<string, int> emptyTuple = Tuple.Create("", 0);
        int spaceTupleLength = spaceTuple.Item2;

        int word_textbox_count = word_textbox.Count;
        string last_word_text = "";

        // convert the list of tuples into a list of lists of tuples; each outer item/element represents a row
        // ASSUMES that word is <= MAX_CHARS_PER_ROW (broken down in above command)
        //foreach (Tuple<string, int> word in word_textbox)
        for (int word_index = 0; word_index < word_textbox.Count; word_index++)
        {
            Tuple<string, int> word = word_textbox[word_index];
            string wordtext = word.Item1;
            int wordLength = word.Item2;

            // If word is a newline  char then force a new in textbox
            if (wordtext == "\n")
            {
                List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                newList.Add(emptyTuple);
                complete_textbox.Add(newList);
                last_word_text = wordtext;
                complete_textbox_index += 2; // BUG: This prevents allowing more than one newline char at a time.
                continue;
            }


            // If there is no list at current index; then create one and add the current word to it!
            if (complete_textbox.Count < complete_textbox_index || (last_word_text == "" || last_word_text == "\n"))
            {
                List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                newList.Add(word);
                last_word_text = wordtext;

                if (wordLength >= MAX_CHARS_PER_ROW)
                {
                    complete_textbox_index++;
                }
                // last part of else if is to ensure space doesn't get added to the last word)
                else if (wordLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index +1 < word_textbox_count)
                {
                    newList.Add(spaceTuple);
                    last_word_text = spaceTuple.Item1;

                    if (wordLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                    { complete_textbox_index++; }
                }
                complete_textbox.Add(newList);
            }
            else
            {
                //At this point it is ASSUMED that complete_textbox[complete_textbox_index-1] contains at least a list of one tuple.
                // And ASSUMED THAT total_length_complete_textbox_at_index should be < MAX_CHARS_PER_ROW
                int total_length_complete_textbox_at_index = 0;

                if (complete_textbox.Count > 0 && complete_textbox[complete_textbox_index - 1].Count > 0)
                {
                    total_length_complete_textbox_at_index = totalLengthOfTupleList(complete_textbox[complete_textbox_index - 1]);
                }

                //combined theorical row length (aka if the word is added to the current row; this would be it's length)
                int totalLength = wordLength + total_length_complete_textbox_at_index;

                if (totalLength > MAX_CHARS_PER_ROW)
                {
                    //then add word to a new row
                    List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                    newList.Add(word);
                    last_word_text = wordtext;

                    if (wordLength >= MAX_CHARS_PER_ROW)
                    {
                        complete_textbox_index++;
                    }
                    // last part of else if is to ensure space doesn't get added to the last word)
                    else if (wordLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index + 1 < word_textbox_count)
                    {
                        newList.Add(spaceTuple);
                        last_word_text = spaceTuple.Item1;

                        if (wordLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                        { complete_textbox_index++; }
                    }

                    complete_textbox.Add(newList);
                    complete_textbox_index++;
                }
                else if (totalLength == MAX_CHARS_PER_ROW)
                {
                    //if last item in list is space, then add to list, otherwise, add word to new row
                    List<Tuple<string, int>> currentList = complete_textbox[complete_textbox_index - 1];
                    last_word_text = wordtext;
                    if (currentList[currentList.Count - 1].Item1 == spaceTuple.Item1)
                    {
                        complete_textbox[complete_textbox_index - 1].Add(word);
                        complete_textbox_index++;
                    }
                    else
                    {
                        List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                        newList.Add(word);
                        complete_textbox.Add(newList);
                        complete_textbox_index++;
                    }
                }
                else if (totalLength < MAX_CHARS_PER_ROW)
                {
                    complete_textbox[complete_textbox_index - 1].Add(word);
                    last_word_text = wordtext;
                    // If you can add a space, determine if complete_textbox_index should be moved to new row
                    // theorically can never have totalLength + spaceTupleLength > MAX_CHARS_PER_ROW
                    // last part of else if is to ensure space doesn't get added to the last word)
                    if (totalLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index + 1 < word_textbox_count)
                    {
                        complete_textbox[complete_textbox_index - 1].Add(spaceTuple);
                        last_word_text = spaceTuple.Item1;

                        if (totalLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                        { complete_textbox_index++; }
                    }
                }
            }
        }

        return complete_textbox;
    }

    // Last list of lists of tuples might consists of an unneeded spaceTuple!!
    public List<List<Tuple<string, int>>> convertComplexWordsListToAListOfComplexWordsLists__REDO(List<Tuple<string, int>> word_textbox)
    {
        List<List<Tuple<string, int>>> complete_textbox = new List<List<Tuple<string, int>>>();
        int complete_textbox_index = 0; // subtract 1 to get actual index; done this as a counting trick for lists

        Tuple<string, int> spaceTuple = Tuple.Create(" ", 1);
        Tuple<string, int> newlineTuple = Tuple.Create("\n", 1);
        Tuple<string, int> emptyTuple = Tuple.Create("", 0);
        int spaceTupleLength = spaceTuple.Item2;

        int word_textbox_count = word_textbox.Count;
        string last_word_text = "";

        // convert the list of tuples into a list of lists of tuples; each outer item/element represents a row
        // ASSUMES that word is <= MAX_CHARS_PER_ROW (broken down in above command)
        //foreach (Tuple<string, int> word in word_textbox)
        for (int word_index = 0; word_index < word_textbox.Count; word_index++)
        {
            Tuple<string, int> word = word_textbox[word_index];
            string wordtext = word.Item1;
            int wordLength = word.Item2;

            // If word is a newline  char then force a new in textbox
            if (wordtext == "\n")
            {
                List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                newList.Add(emptyTuple);
                complete_textbox.Add(newList);
                last_word_text = wordtext;
                complete_textbox_index+=1;
                continue;
            }


            // If there is no list at current index; then create one and add the current word to it!
            if (complete_textbox.Count < complete_textbox_index && last_word_text != "\n")
            {
                List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                newList.Add(word);
                last_word_text = wordtext;

                if (wordLength >= MAX_CHARS_PER_ROW)
                {
                    complete_textbox_index++;
                }
                // last part of else if is to ensure space doesn't get added to the last word)
                else if (wordLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index + 1 < word_textbox_count)
                {
                    newList.Add(spaceTuple);
                    last_word_text = spaceTuple.Item1;

                    if (wordLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                    { complete_textbox_index++; }
                }
                complete_textbox.Add(newList);
                last_word_text = wordtext;
            }
            else
            {
                //At this point it is ASSUMED that complete_textbox[complete_textbox_index-1] contains at least a list of one tuple.
                // And ASSUMED THAT total_length_complete_textbox_at_index should be < MAX_CHARS_PER_ROW
                int total_length_complete_textbox_at_index = 0;

                if (complete_textbox.Count > 0 && complete_textbox[complete_textbox_index - 1].Count > 0)
                {
                    total_length_complete_textbox_at_index = totalLengthOfTupleList(complete_textbox[complete_textbox_index - 1]);
                }

                //combined theorical row length (aka if the word is added to the current row; this would be it's length)
                int totalLength = wordLength + total_length_complete_textbox_at_index;

                if (totalLength > MAX_CHARS_PER_ROW)
                {
                    //then add word to a new row
                    List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                    newList.Add(word);
                    last_word_text = wordtext;

                    if (wordLength >= MAX_CHARS_PER_ROW)
                    {
                        complete_textbox_index++;
                    }
                    // last part of else if is to ensure space doesn't get added to the last word)
                    else if (wordLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index + 1 < word_textbox_count)
                    {
                        newList.Add(spaceTuple);
                        last_word_text = spaceTuple.Item1;

                        if (wordLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                        { complete_textbox_index++; }
                    }

                    complete_textbox.Add(newList);
                    complete_textbox_index++;
                }
                else if (totalLength == MAX_CHARS_PER_ROW)
                {
                    //if last item in list is space, then add to list, otherwise, add word to new row
                    List<Tuple<string, int>> currentList = complete_textbox[complete_textbox_index - 1];
                    last_word_text = wordtext;
                    if (currentList[currentList.Count - 1].Item1 == spaceTuple.Item1)
                    {
                        complete_textbox[complete_textbox_index - 1].Add(word);
                        complete_textbox_index++;
                    }
                    else
                    {
                        List<Tuple<string, int>> newList = new List<Tuple<string, int>>();
                        newList.Add(word);
                        complete_textbox.Add(newList);
                        complete_textbox_index++;
                    }
                }
                else if (totalLength < MAX_CHARS_PER_ROW)
                {
                    complete_textbox[complete_textbox_index - 1].Add(word);
                    last_word_text = wordtext;
                    // If you can add a space, determine if complete_textbox_index should be moved to new row
                    // theorically can never have totalLength + spaceTupleLength > MAX_CHARS_PER_ROW
                    // last part of else if is to ensure space doesn't get added to the last word)
                    if (totalLength + spaceTupleLength <= MAX_CHARS_PER_ROW && word_index + 1 < word_textbox_count)
                    {
                        complete_textbox[complete_textbox_index - 1].Add(spaceTuple);
                        last_word_text = spaceTuple.Item1;

                        if (totalLength + spaceTupleLength == MAX_CHARS_PER_ROW)
                        { complete_textbox_index++; }
                    }
                }
            }
        }

        return complete_textbox;
    }

    public List<Tuple<string, int>> convertWordsToComplexWordsList(string[] allWords)
    {
        List<Tuple<string, int>> word_textbox = new List<Tuple<string, int>>();

        foreach (string word in allWords)
        {
            /*if (word.Length > MAX_CHARS_PER_ROW)
            {
                textBox.Add(word.Substring(0, MAX_CHARS_PER_ROW - 1) + "-");
                textBox.Add(word.Substring(MAX_CHARS_PER_ROW - 1));
            }
            else { 
                
            }*/
            // TODO: NEED TO UPDATE THIS SECTION. NOT PROPERLY GETTING word joining!!

            //List<string> results = breakDownWordWithRegex(word);


            /*int total_length_complete_textbox_at_index = 0;

            if (complete_textbox.Count > 0 && complete_textbox[complete_textbox_index].Count > 0)
            {
                total_length_complete_textbox_at_index = totalLengthOfTupleList(complete_textbox[complete_textbox_index]);
            }*/

            List<Tuple<string, int>> results = breakDownWordWithRegex(word);
            /*print("Results: " + results.ToString());
            int results_count = results.Count;
            print("Results Count: " + results_count.ToString());

            int total_length_in_results = totalLengthOfTupleList(results);
            print($"Total Length: {total_length_in_results.ToString()}");*/

            word_textbox.AddRange(results);

            /*if (total_length_complete_textbox_at_index + total_length_in_results < MAX_CHARS_PER_ROW)
            {
                // keep complete index the same
                complete_textbox.Insert(complete_textbox_index, results);

                continue;// move to next word
            }
            else if (total_length_complete_textbox_at_index + total_length_in_results == MAX_CHARS_PER_ROW)
            {
                complete_textbox.Insert(complete_textbox_index, results);
                complete_textbox_index++;

                continue;
            }

            List<Tuple<string, int>> lineResults = new List<Tuple<string, int>>();

            for(int index = 0;  index< results.Count; index++)
            {
                //string result = results[index];
                int total_length_for_lineResults = totalLengthOfTupleList(lineResults);

                string result = results[index].Item1;
                int subwordLength = results[index].Item2;

                

                textBox.Add(result);

                lineText += result;

                // if there are still more, then move to new line.
                if (index + 1 < results.Count)
                {
                    lineText +="\n";
                }
            }*/
        }
        return word_textbox;
    }

    // for when you want to concatenate two strings (ie. warning and NPC text)
    // ASSUMES the text has already been processed and a newline char indicates a new line/row
    public List<string> convertStringToListOfStrings(string text)
    {
        string[] rows = text.Split('\n');
        List<string> results = rows.ToList();
        return results;
    }

    public int totalLengthOfTupleList(List<Tuple<string, int>> row_list)
    {
        List<int> all_lengths = (from aTuple in row_list select aTuple.Item2).ToList();
        int total_lengths = all_lengths.Sum();
        return total_lengths;
    }

    // for when you want to fill the rest of the line
    public string fillLineBuffer(int quantity, string bufferChar = " ")
    {
        string bufferString = "";

        for (int i = 0; i < quantity; i++)
        {
            bufferString += bufferChar;
        }

        return bufferString;
    }

    public void update_text(string text)
    {
        chat_box_text.text = text;//adjustText(text);
    }

    public void scroll_down()
    {
        //int total_scroll_pages = (int)Mathf.Ceil((float)textBox.Count / MAX_ROWS);
        /*print("Scroll page: " + scroll_page.ToString());
        print("textBox count: " + textBox.Count.ToString());
        print((scroll_page + 1) * MAX_ROWS < textBox.Count);*/

        if ((scroll_page + 1) * MAX_ROWS < textBox.Count)
        {
            scroll_page++;
            string s = "";
            //print("NEW Scroll page: " + scroll_page.ToString());
            for (int i = scroll_page * MAX_ROWS; i < (scroll_page + 1) * MAX_ROWS && i< textBox.Count; i ++)
            {
                //print(">>" + textBox[i]);
                s += textBox[i] + "\n";
            }
            update_text(s);
            adjust_scroll_availability();
        }
        //print_text_boxes();
    }

    public void scroll_up()
    {
        /*print("Alignment: "+chat_box_text.alignment);
        print("AutoSizeContainer: "+chat_box_text.autoSizeTextContainer);
        print("Bounds: "+chat_box_text.bounds);
        print("Overflowing?: "+chat_box_text.isTextOverflowing);*/

        if (scroll_page - 1 >= 0)
        {
            scroll_page--;
            //string s = textBox[scroll_page];
            string s = "";

            /*if (scroll_page + 1 < textBox.Count)
            {
                s += "\n" + textBox[scroll_page + 1];
            }*/

            // Since we are coming from a lower page to a higher page, we know there are atleast MAX_ROWS worth of lines to print
            //for (int i = scroll_page* MAX_ROWS; i <= MAX_ROWS; i++)
            for (int i = scroll_page* MAX_ROWS; i < (scroll_page + 1) * MAX_ROWS && i < textBox.Count; i++)
            {
                s += textBox[i] + "\n";
            }

            update_text(s);
            adjust_scroll_availability();
        }
        //print_text_boxes();
    }

    public void adjust_scroll_availability()
    {
        scroll_UP_button.interactable = true;
        scroll_DOWN_button.interactable = true;

        if (textBox.Count == 0) //if nothing to show then disable buttons!
        {
            scroll_UP_button.interactable = false;
            scroll_DOWN_button.interactable = false;
            return;
        }


        if (scroll_page == 0)
        {
            //can't scroll up+
            scroll_UP_button.interactable = false;
        }

        if ((scroll_page + 1) * MAX_ROWS >= textBox.Count)
        {
            scroll_DOWN_button.interactable = false;
        }
    }

    public void update_textbox()
    {
        string message = "";

        /*if (warning_string.Length > 0 && NPC_string.Length > 0)
        {
            message = warning_string + "\n" + buffer_line + "\n" + NPC_string;
        }
        else if (warning_string.Length > 0)
        {
            message = warning_string;
        }
        else if (NPC_string.Length > 0)
        {
            message = NPC_string;
        }*/
        //Priority: warning, system, then NPC

        message = warning_string;
        if (warning_string.Length > 0 && (NPC_string.Length > 0 || system_string.Length > 0))
        { message += "\n" + buffer_line + "\n"; }

        message += system_string;
        if (system_string.Length > 0 && NPC_string.Length > 0)
        { message += "\n" + buffer_line + "\n"; }

        message += NPC_string;

        if (message.Length > 0)
        {
            List<string> newTextbox = convertStringToListOfStrings(message);

            textBox.Clear();
            textBox.AddRange(newTextbox);
            update_text(message);
            scroll_page = 0;
            adjust_scroll_availability();
        }
    }    

    public void printWarningText(string warningKey)
    {
        if (warningJSON != null)
        {
            if (warningJSON.HasKey(warningKey))
            {
                warning_string = warningJSON[warningKey][FORMATTED_TEXT];

                update_textbox();
                //string adjusted_text = adjustText(text);
                //update_text(adjusted_text);

                /*string text = warningJSON[warningKey];
                Tuple<string, List<string>> adjustedText = adjustTextRegex(text);
                text = adjustedText.Item1;
                textBox.Clear();
                textBox.AddRange(adjustedText.Item2);

                //scroll_up();
                update_text(text);*/
            }
            else
            {
                Debug.LogError("ERROR>> The warning key: { " + warningKey + " } does not exist.");
            }
        }
        else 
        {
            Debug.LogError("ERROR>> The warning JSON file either was not loaded or does not exist.");
        }
    }

    public void updateNPCtext(string formatted_text)
    {
        warning_string = "";
        system_string = "";
        NPC_string = formatted_text;
        update_textbox();
    }

    public void updateSystemText(string text)
    {
        system_string = adjustTextRegex(text).Item1;
        update_textbox();
    }

    // For debugging purposes
    void print_text_boxes()
    {
        Debug.Log("total: " + textBox.Count.ToString());
        foreach (string s in textBox)
        {
            Debug.Log(":: " + s.Length.ToString());
            Debug.Log(s);
        }
        Debug.Log("");
    
    }
}
