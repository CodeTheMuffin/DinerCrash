using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manger : MonoBehaviour
{
    public Stats GameStat;
    public GameObject OrderingMenu;
    public OrderManager orderManager;

    public const int MAX_AMOUNT = 10;// to use modulus on; actual max value is 9

    int cholocate_cookie_counter = 0;
    int oatmeal_raisin_cookie_counter = 0;
    int normal_milk_counter = 0;
    int warm_milk_counter = 0;

    //prices the NPC will pay
    int cost_cholocate_cookie = 3;
    int cost_oatmeal_raisin_cookie = 4;
    int cost_normal_milk = 2;
    int cost_warm_milk = 6;

    //supply cost
    const int supply_cost_of_box = 1;
    const int supply_cost_cholocate_cookie = 1;
    const int supply_cost_oatmeal_raisin_cookie = 1;
    const int supply_cost_normal_milk = 1;
    const int supply_cost_warm_milk = 1;

    int total_amount = 0;
    float discount_percentage = 0.5f;
    bool discount_applied = false;

    GameObject eventSystem;

    public TMPro.TextMeshProUGUI cholocate_cookie_text;
    public TMPro.TextMeshProUGUI oatmeal_raisin_cookie_text;
    public TMPro.TextMeshProUGUI normal_milk_text;
    public TMPro.TextMeshProUGUI warm_milk_text;
    public TMPro.TextMeshProUGUI total_amount_text;

    public Button cholocate_cookie_button;
    public Button oatmeal_raisin_cookie_button;
    public Button normal_milk_button;
    public Button warm_milk_button;

    public Image discount_button_image;
    public Sprite normal_discount_sprite;
    public Sprite pressed_discount_sprite;

    // since its not a scriptable object, it will not show in inspector
    // but the features of an scriptable object are not what I want
    // it still works, so Im happy with this.
    public List<OrderForm> orders;

    public TextSystem text_sys;

    void Awake()
    {
        eventSystem = UnityEngine.EventSystems.EventSystem.current.gameObject;
        orders = new List<OrderForm>();
        update_total();
        closeOrderingMenu();
    }

    /*
     INCREMENTING COUNTERS
     */
    public void increment_cholocate_cookie_counter()
    {
        cholocate_cookie_counter = (cholocate_cookie_counter + 1) % MAX_AMOUNT;
        cholocate_cookie_text.text = cholocate_cookie_counter.ToString();
        update_total();
    }

    public void increment_oatmeal_raisin_cookie_counter()
    {
        oatmeal_raisin_cookie_counter = (oatmeal_raisin_cookie_counter + 1) % MAX_AMOUNT;
        oatmeal_raisin_cookie_text.text = oatmeal_raisin_cookie_counter.ToString();
        update_total();
    }

    public void increment_normal_milk_counter()
    {
        normal_milk_counter = (normal_milk_counter + 1) % MAX_AMOUNT;
        normal_milk_text.text = normal_milk_counter.ToString();
        update_total();
    }

    public void increment_warm_milk_counter()
    {
        warm_milk_counter = (warm_milk_counter + 1) % MAX_AMOUNT;
        warm_milk_text.text = warm_milk_counter.ToString();
        update_total();
    }

    public void update_total()
    {
        update_total_amount();
        update_total_amount_text();
    }
    public void update_total_amount()
    {
        total_amount = (cholocate_cookie_counter * cost_cholocate_cookie) +
            (oatmeal_raisin_cookie_counter * cost_oatmeal_raisin_cookie) +
            (normal_milk_counter * cost_normal_milk) +
            (warm_milk_counter * cost_warm_milk);
    }
    public void update_total_amount_text()
    {
        // To create a buffer and always show 3 digits
        string total_amount_str = "0";

        if (total_amount == 0)
        {
            total_amount_str = "000";
        }
        else if (total_amount < 10 && total_amount > 0)
        {
            total_amount_str = "00" + total_amount.ToString();
        }
        else if (total_amount < 100 && total_amount > 0)
        {
            total_amount_str = "0" + total_amount.ToString();
        }
        else //100+ and <0 values
        {
            total_amount_str = total_amount.ToString();
        }

        total_amount_text.text = total_amount_str;
    }

    public int getSupplyCost()
    {
        int supply_cost = supply_cost_of_box + 
            (cholocate_cookie_counter * supply_cost_cholocate_cookie) +
            (oatmeal_raisin_cookie_counter * supply_cost_oatmeal_raisin_cookie) +
            (normal_milk_counter * supply_cost_normal_milk) +
            (warm_milk_counter * supply_cost_warm_milk);

        return supply_cost * -1;//want to be a negative value
    }

    public void clearMenu()
    {
        cholocate_cookie_counter = -1;
        oatmeal_raisin_cookie_counter = -1;
        normal_milk_counter = -1;
        warm_milk_counter = -1;

        // probably a bad way of doing it, but should work!!
        increment_cholocate_cookie_counter();
        increment_oatmeal_raisin_cookie_counter();
        increment_normal_milk_counter();
        increment_warm_milk_counter();
        discount_applied = false;
        ToggleButtonInteractive(true);
        update_total();
        discount_button_image.sprite = normal_discount_sprite;
        /*
        // proof that the orders list is working 
        foreach (OrderForm o in orders)
        {
            print("Total: " + o.total_amount.ToString());
        }*/
    }

    // Toggles whether the interactable buttons are active or deactivated so you can't select them.
    public void ToggleButtonInteractive(bool active)
    {
        cholocate_cookie_button.interactable = active;
        oatmeal_raisin_cookie_button.interactable = active;
        normal_milk_button.interactable = active;
        warm_milk_button.interactable = active;
    }

    public void toggleDiscount()
    {
        discount_applied = !discount_applied;

        // only apply discount if it hasn't already been applied
        if (discount_applied)
        {
            // fun easter egg
            if (total_amount != 69)
            {
                total_amount = (int)Mathf.Ceil((float)total_amount * discount_percentage);
            }
            else
            {
                total_amount = -9;
            }

            discount_button_image.sprite = pressed_discount_sprite;
            update_total_amount_text();
            ToggleButtonInteractive(false);
        }
        else
        {
            discount_button_image.sprite = normal_discount_sprite;
            update_total();
            ToggleButtonInteractive(true);
        }    
    }

    public float getEstimatePreparingTime()
    {
        float estimated_prepare_time = 0f;
        estimated_prepare_time = OrderForm.get_estimated_prepare_time(cholocate_cookie_counter, oatmeal_raisin_cookie_counter, normal_milk_counter, warm_milk_counter);
        return estimated_prepare_time;
    }

    public void process_order()
    {
        OrderForm newOrderForm = new OrderForm(
            cholocate_cookie_counter, oatmeal_raisin_cookie_counter,
            normal_milk_counter, warm_milk_counter,
            total_amount, discount_applied);

        orders.Add(newOrderForm);
        int supply_cost = getSupplyCost();
        GameStat.updateBudgetActual(supply_cost);
        //hacky way to do it...
        GameStat.txtSys.updateSystemText($"Supply Cost: <color=#da4e38>${supply_cost}</color>");//price turns red


        clearMenu();
        closeOrderingMenu();

        int nextOrderBoxIndex = orderManager.get_next_available_order_index();

        if (nextOrderBoxIndex >= 0)
        {
            orderManager.setOrderForm(orderManager.orderBoxManagers[nextOrderBoxIndex], newOrderForm);
        }
    }


    /*
     ORDERING MENU
     */
    public bool haveCapacityToOrder()
    {
        int next_index = orderManager.get_next_available_order_index();
        return next_index != -1;
    }

    public bool isOrderingMenuOpen()
    {
        return OrderingMenu.activeSelf;
    }

    public void closeOrderingMenu()
    {
        OrderingMenu.SetActive(false);
    }

    public void openOrderingMenu()
    {
        OrderingMenu.SetActive(true);
    }
}
