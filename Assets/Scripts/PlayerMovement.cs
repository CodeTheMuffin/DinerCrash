using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //public CharacterController2D controller;
    public Animator animator;

    public float PixelsPerUnit = 8f;
    public float runSpeed = 40f;

    float HorizontalMove = 0f;
    float VerticalMove = 0f;
    //bool jump = false;
    //bool crouch = false;

    public Transform playerTransform;
    private Vector2 original_position;

    [SerializeField]
    private bool canAccessMenu = false;
    //public GameObject OrderingMenu;
    
    public UI_Manger ui_manger;

    public Transform left_side;
    public Transform right_side;
    public Transform top_side;
    public Transform bottom_side;

    bool canPickUpOrder = false;
    bool holdingOrder = false;

    //bool looking_left = true; // determines whether or not to place the order of left side or right side of the player's body
    //bool looking_up = false;
    int looking_direction = (int)LookingDirection.LEFT;

    GameObject orderboxParent; // for when the user picks up an order box
    OrderBox orderBoxInRange; // for when the user gets close to an OrderBox.
    Transform orderboxBeingHeld;

    enum LookingDirection { 
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }


    private void Awake()
    {
        original_position = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));


        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        //ESC
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        AccessOrder();
        AccessPC();
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Reset()
    {
        // Resets player's position and speed
        playerTransform.position = original_position;
        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        /*jump = false;
        animator.SetBool("isJumping", false);*/
    }
    void Move()
    {
        // Only move when the ordering menu is closed
        if (!ui_manger.isOrderingMenuOpen())
        {
            // Since a unit can be considered 8 pixels, for example, you want to move a pixel at a time rather an whole unit (8 pixels)
            HorizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * 1 / PixelsPerUnit;
            VerticalMove = Input.GetAxisRaw("Vertical") * runSpeed * 1 / PixelsPerUnit;

            // no need to check if ==0 since it should just use the last looking_left value 
            if (VerticalMove > 0)
            { looking_direction = (int)LookingDirection.UP; }
            else if (VerticalMove < 0)
            { looking_direction = (int)LookingDirection.DOWN;}

            
            // priorize horizontal direction over vertical
            if (HorizontalMove > 0)
            { looking_direction = (int)LookingDirection.RIGHT; }
            else if (HorizontalMove < 0)
            { looking_direction = (int)LookingDirection.LEFT; }

            

            /*print("Horz: " + HorizontalMove);
            print("Vert: " + VerticalMove);*/


            //controller.Move(HorizontalMove * Time.fixedDeltaTime, false, false);

            playerTransform.Translate(new Vector3(HorizontalMove, VerticalMove, 0));
        }
    }

    void AccessPC()
    {
        // Going to the computer and pressing E to open ordering menu
        if (canAccessMenu && !ui_manger.isOrderingMenuOpen() && Input.GetKeyDown(KeyCode.E))
        {
            ui_manger.openOrderingMenu();
        }

        // Ordering menu is open and pressing Q to close menu
        if (canAccessMenu && ui_manger.isOrderingMenuOpen() && Input.GetKeyDown(KeyCode.Q))
        {
            ui_manger.closeOrderingMenu();
        }

    }

    void AccessOrder()
    {
        // If Im within range to pick up order and not already holding an order and press E
        // then pick up order
        if (canPickUpOrder && !holdingOrder && Input.GetKeyDown(KeyCode.E))
        {
            holdingOrder = true;

            // reassigning the orderbox to the players as a child gameobject
            foreach (Transform child in orderboxParent.transform)
            {
                if (child.tag == "order_box")
                {
                    child.SetParent(gameObject.transform);
                    orderboxBeingHeld = child;
                    break;
                }
            }
        }

        // Update each movement
        if (holdingOrder && orderboxBeingHeld)
        {
            switch (looking_direction)
            {
                case (int)LookingDirection.UP:
                    orderboxBeingHeld.position = top_side.position;
                    break;
                case (int)LookingDirection.DOWN:
                    orderboxBeingHeld.position = bottom_side.position;
                    break;
                case (int)LookingDirection.LEFT:
                    orderboxBeingHeld.position = left_side.position;
                    break;
                case (int)LookingDirection.RIGHT:
                    orderboxBeingHeld.position = right_side.position;
                    break;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Entering: " + collision.name);

        if (collision.tag == "computer")
        {
            canAccessMenu = true;
        }
        else if (collision.tag == "order_box_parent")
        {
            canPickUpOrder = true;
            orderboxParent = collision.gameObject;
            orderBoxInRange = orderboxParent.GetComponent<OrderBox>();
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        print("Hit!" + collision.tag);
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {
        print("Leaving: " + collision.name);
        if (collision.tag == "computer")
        {
            canAccessMenu = false;
        }
        else if (collision.tag == "order_box_parent")
        {
            canPickUpOrder = false;
            orderboxParent = null;
            orderBoxInRange = null;
        }
    }

}
