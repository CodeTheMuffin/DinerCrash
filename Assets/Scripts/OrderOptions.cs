using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OrderOptions
{
    public enum Options
    {
        cholocate_cookie = 0,
        oatmeal_raisan_cookie = 1,
        normal_milk = 2,
        warm_milk = 3
    }

    // The KEY should be the same key name in "{lang}_npc_text.json"
    public static Dictionary<string, int> option_names_to_index = new Dictionary<string, int>() {
        {"OPTION_01", (int)Options.cholocate_cookie},
        {"OPTION_02", (int)Options.oatmeal_raisan_cookie},
        {"OPTION_03", (int)Options.normal_milk},
        {"OPTION_04", (int)Options.warm_milk}
    };
}
