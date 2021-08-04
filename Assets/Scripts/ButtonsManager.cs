using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

//Use the Selectable class as a base class to access the IsHighlighted method
public class ButtonsManager : Selectable, IPointerExitHandler// required interface when using the OnPointerExit method.
{
    /*enum Interact_Type
    { 
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4
    }*/

    //public Sprite[] button_sprites; // index corrisponds to the Interact_Type
    public Sprite normal_sprite;
    public Sprite highlighted_sprite;
    public Sprite pressed_sprite;
    public Sprite selected_sprite;
    public Sprite disabled_sprite;

    //public Color[] button_colors ;  // index corrisponds to the Interact_Type

    public Image button_img;
    //private int total_interact_types;

    //private bool useButton = false;

    /*private void Start()
    {


        *//*total_interact_types = System.Enum.GetNames(typeof(Interact_Type)).Length;
        int button_sprite_length = button_sprites.Length;*//*
        //int button_color_length = button_colors.Length;

        *//*if (button_sprite_length < total_interact_types)
        {
            useButton = true;
        }
        else
        {
            Debug.LogError("Too many button sprites: " + button_sprite_length + "out of " + total_interact_types);
        }*/


        /*if (button_color_length < total_interact_types)
        {
            useButton = true;
        }
        else
        {
            Debug.LogError("Too many button colors: " + button_color_length + "out of " + total_interact_types);
        }*//*

    }
*/
    // Update is called once per frame
    void Update()
    {
        /*enum Interact_Type
    {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }*/

        if (IsHighlighted())
        {
            //button_img.sprite = button_sprites[(int)Interact_Type.Highlighted];
            button_img.sprite = highlighted_sprite;
        }
        else if (IsPressed())
        {
            button_img.sprite = pressed_sprite;
        }
        else if (!IsInteractable())
        {
            button_img.sprite = disabled_sprite;
        }

    }

    
}
