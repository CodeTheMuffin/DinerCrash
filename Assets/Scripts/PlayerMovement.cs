using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;
    float horizontalMove = 0f;
    //bool jump = false;
    //bool crouch = false;

    public Transform player_transform;
    private Vector2 original_position;
    private void Start()
    {
        original_position = player_transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        /*if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }*/


        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    private void Reset()
    {
        // Resets player's position and speed
        player_transform.position = original_position;
        //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        /*jump = false;
        animator.SetBool("isJumping", false);*/
    }


    /*public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }*/

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
        //controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        //jump = false;
    }
}
