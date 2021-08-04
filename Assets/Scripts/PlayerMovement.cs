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
   

    private void Awake()
    {
        original_position = playerTransform.position;

        //OrderingMenu.SetActive(false);
        //ui_manger.close_OrderingMenu();
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));


        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }


        AccessPC();
    }

    private void FixedUpdate()
    {

        Move();
        //controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
        //controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        //jump = false;
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
            print("ACCESS");
            ui_manger.open_OrderingMenu();
        }

        // Ordering menu is open and pressing Q to close menu
        if (canAccessMenu && ui_manger.isOrderingMenuOpen() && Input.GetKeyDown(KeyCode.Q))
        {
            ui_manger.close_OrderingMenu();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "computer")
        {
            canAccessMenu = true;
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        print("Hit!" + collision.tag);
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "computer")
        {
            canAccessMenu = false;
        }
    }

}
