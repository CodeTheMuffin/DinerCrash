using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GameUtility;

public class Player : MonoBehaviour
{
    public InputMaster controls;

    //public CharacterController2D controller;
    public Transform playerTransform;
    public Animator animator;

    public float runSpeed = 40f;
    float HorizontalMove = 0f;
    float VerticalMove = 0f;

    public float PixelsPerUnit = 8f;


    // Start is called before the first frame update
    void Awake()
    {
        controls = new InputMaster();
        //controls.Player.Movement.performed += context => Move(context.ReadValue<Vector2>());
    }

    private void Update()
    {
        // https://forum.unity.com/threads/new-input-system-how-to-use-the-hold-interaction.605587/

        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        if (kb.spaceKey.isPressed)
        {
            Debug.Log("Space Ace.");
        }

        if (controls.Player.Movement.triggered)
        { 
            //print(controls.Player.Movement.ReadValue<Vector2>());
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
            Move(direction);


        }
    }

    /*
     Do this if you want to just grab if a certain key was pressed or not...
     */
    /*private void Update()
    {
        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        if (kb.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space Ace.");
        }
    }*/


    void Move(Vector2 direction)
    {
        // Since a unit can be considered 8 pixels, for example, you want to move a pixel at a time rather an whole unit (8 pixels)
        HorizontalMove = Mathf.Ceil(direction.x) * runSpeed * 1/PixelsPerUnit;
        VerticalMove = Mathf.Ceil(direction.y) * runSpeed * 1/PixelsPerUnit;
        print("Horz: " + HorizontalMove);
        print("Vert: " + VerticalMove);


        //controller.Move(HorizontalMove * Time.fixedDeltaTime, false, false);

        playerTransform.Translate(new Vector3(HorizontalMove, VerticalMove, 0));
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
