using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class TextSystem : MonoBehaviour
{
    const int MAX_CHARS_PER_ROW = 10; // The max number of chars per row
    const int MAX_ROWS = 2; // X rows to show on screen at a time

    public TMPro.TextMeshProUGUI chat_box_text;

    List<string> textBox;
    int scroll_page = 0;

    const char NEWLINE = '`';// If this is present, then place a new line in string.

    JSONNode warningJSON;

    public void Start()
    {
        textBox = new List<string>();

        string xx = "Hello     World!Thisis my game!!!";
        string xy = "Just for show!";
        //print(xx.ToString());

        //print(adjustText(xx).ToString());
        xx = adjustText(xy);
        update_text(xx);
        //print_text_boxes();
        //Debug.Log("dumb");

        // load file from the path
        /*TextAsset warningjsonPath = Resources.Load<TextAsset>("Text/en-warning_text");
        print(warningjsonPath);
        print(warningjsonPath.text);*/

        /*string json = LoadJson(warningjsonPath);
       print(json);*/

        //must not contain extension!
        string jsontext = LoadJson("Text/en-warning_text");
        warningJSON = JSON.Parse(jsontext);
    }

    /*public string LoadJson(string json_path)
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

    public string LoadJson(string json_path)
    {
        // This MUST be in folder: Asset/Resources/ and the JSON path must NOT include extension (.json)
        TextAsset jsontext = Resources.Load<TextAsset>(json_path);
        return jsontext ? jsontext.text : "";
    }

    // excludes the spaces
    public string[] getAllWords(string text)
    {
        string[] results = Regex.Split(text, " ");
        return results;
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

    public static List<string> breakDownWordWithRegex(string text)
    {
        string[] colorRegexTextToIgnore = new string[] { "<color=\".*\">(.*)</color>", "<color=\".*\">", "</color>" };
        string subword = text;
        List<string> breakDown = new List<string>();

        Match match_both = Regex.Match(text, colorRegexTextToIgnore[0]);
        Match match_left = Regex.Match(text, colorRegexTextToIgnore[1]);
        Match match_right = Regex.Match(text, colorRegexTextToIgnore[2]);

        string leftText = "";
        string rightText = "";
        print("Got here");

        if (match_both.Success)
        {
            print("m both g0: " + match_both.Groups[0].Value);
            print("m both g1: " + match_both.Groups[1].Value);

            subword = match_both.Groups[1].Value;

            int index = text.IndexOf(subword);
            print("index: " + index.ToString());

            leftText = text.Substring(0, index);
            rightText = text.Substring(index + subword.Length);

            print("left text: " + leftText);
            print("right text: " + rightText);

        }
        else if (match_left.Success)
        {
            leftText = match_left.Groups[0].Value;
            print("m left g0: " + leftText);
            print("m left g1: " + match_left.Groups[1].Value);

            int index = text.IndexOf(leftText);
            print("index: " + index.ToString());

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
            print("m right g0: " + rightText);
            print("m right g1: " + match_right.Groups[1].Value);
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

        print("subword: " + subword);

        if (subword == text)
        {
            print("No regex found for word: "+ text);
            breakDown.Add(text);
        }
        else
        {
            print("--------------");
            print("Left text: " + leftText + "\tlen: " + leftText.Length);
            print("Subword text: " + subword + "\tlen: " + subword.Length);
            print("Right text: " + rightText + "\tlen: " + rightText.Length);

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
                }

            }

            /*if(rightText != null && rightText != "")
            {
                breakDown.Add(rightText);
            }*/
        }

        return breakDown;
    }

    public string adjustTextRegex(string text)
    {
        /*MatchCollection allSpaces = Regex.Matches(text, " ");

        foreach (Match m in allSpaces)
        {
            print("found one: " + m);
        }*/
        textBox.Clear();
        string[] allWords = getAllWords(text);

        print(allWords.Length.ToString());

        string lineText = "";

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

            List<string> results = breakDownWordWithRegex(word);
            print("Results: "+results.ToString());
            print("Results Count: " + results.Count.ToString());

            for(int index = 0;  index< results.Count; index++)
            {
                string result = results[index];
                textBox.Add(result);

                lineText += result;

                // if there are still more, then move to new line.
                if (index + 1 < results.Count)
                {
                    lineText +="\n";
                }
            }
        }


        return lineText;
    }


    // for when you want to fill the rest of the line
    public string fillLineBuffer(int quantity)
    {
        string bufferString = "";

        for (int i = 0; i < quantity; i++)
        {
            bufferString += " ";
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
                s += textBox[i];
            }
            update_text(s);
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
            for (int i = scroll_page* MAX_ROWS; i <= MAX_ROWS; i++)
            {
                s += textBox[i];
            }

            update_text(s);
        }
        //print_text_boxes();
    }

    public void printWarningText(string warningKey)
    {
        

        if (warningJSON != null)
        {
            if (warningJSON.HasKey(warningKey))
            {
                string text = warningJSON[warningKey];
                print("original warning text: " + text);
                //string adjusted_text = adjustText(text);
                //update_text(adjusted_text);
                text = adjustTextRegex(text);
                //scroll_up();
                update_text(text);
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
