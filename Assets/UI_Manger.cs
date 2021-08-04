using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manger : MonoBehaviour
{
    public GameObject OrderingMenu;

    const int MAX_AMOUNT = 10;

    int cholocate_cookie_counter = 0;
    int oatmeal_raisan_cookie_counter = 0;
    int normal_milk_counter = 0;
    int warm_milk_counter = 0;

    public TMPro.TextMeshProUGUI cholocate_cookie_text;
    public TMPro.TextMeshProUGUI oatmeal_raisan_cookie_text;
    public TMPro.TextMeshProUGUI normal_milk_text;
    public TMPro.TextMeshProUGUI warm_milk_text;
    public TMPro.TextMeshProUGUI total_amount_text;

    void Awake()
    {
        close_OrderingMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     INCREMENTING COUNTERS
     */
    public void increment_cholocate_cookie_counter()
    {
        cholocate_cookie_counter = (cholocate_cookie_counter + 1) % MAX_AMOUNT;
        cholocate_cookie_text.text = cholocate_cookie_counter.ToString();
    }

    public void increment_oatmeal_raisan_cookie_counter()
    {
        oatmeal_raisan_cookie_counter = (oatmeal_raisan_cookie_counter + 1) % MAX_AMOUNT;
        oatmeal_raisan_cookie_text.text = oatmeal_raisan_cookie_counter.ToString();
    }

    public void increment_normal_milk_counter()
    {
        normal_milk_counter = (normal_milk_counter + 1) % MAX_AMOUNT;
        normal_milk_text.text = normal_milk_counter.ToString();
    }

    public void increment_warm_milk_counter()
    {
        warm_milk_counter = (warm_milk_counter + 1) % MAX_AMOUNT;
        warm_milk_text.text = warm_milk_counter.ToString();
    }


    /*
     ORDERING MENU
     */
    public bool isOrderingMenuOpen()
    {
        return OrderingMenu.activeSelf;
    }

    public void close_OrderingMenu()
    {
        OrderingMenu.SetActive(false);
    }

    public void open_OrderingMenu()
    {
        OrderingMenu.SetActive(true);
    }
}
