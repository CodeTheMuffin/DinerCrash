using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSystem : MonoBehaviour
{
    const int MAX_CHARS_PER_ROW = 10; // The max number of chars per row
    const int MAX_ROWS = 2; // X rows to show on screen at a time

    public void Start()
    {
        string xx = "01234567890123456";
        //print(xx.ToString());

        //print(adjustText(xx).ToString());

    }

    public string adjustText(string text)
    {
        string complete_string = "";

        int char_counter = 0;
        int row_counter = 0;
        //print(MAX_CHARS_PER_ROW);

        foreach (char s in text)
        {
            //print(">> " + s);
            if (row_counter < MAX_CHARS_PER_ROW)
            {
                //print(s);
                complete_string += s;
            }
            else
            {
                //print("Got one! "+ s);
                complete_string = complete_string+ s + '-';
            }

            row_counter = (row_counter + 1) % MAX_CHARS_PER_ROW+1;
            char_counter++;
        }
        //print(complete_string.ToString());

        return complete_string;
    }
}
