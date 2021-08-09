using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;

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
                string adjusted_text = adjustText(text);
                update_text(adjusted_text);
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
