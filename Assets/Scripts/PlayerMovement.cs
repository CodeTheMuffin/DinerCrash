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
    bool canThrowAwayOrder = false;

    //bool looking_left = true; // determines whether or not to place the order of left side or right side of the player's body
    //bool looking_up = false;
    int looking_direction = (int)LookingDirection.LEFT;

    GameObject orderboxParent; // for when the user picks up an order box
    OrderBoxManager orderBoxInRange; // for when the user gets close to an OrderBox.
    Transform orderboxBeingHeld;
    List<GameObject> orderBoxInCloseRangeCollision = new List<GameObject>();
    bool collisionDetectionChange = false; // used so we dont go through the list each frame. set to true if a collision was added/removed

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
            { looking_direction = (int)LookingDirection.DOWN; }


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
        if (canAccessMenu && !holdingOrder)
        {
            // Going to the computer and pressing E to open ordering menu
            if (!ui_manger.isOrderingMenuOpen() && Input.GetKeyDown(KeyCode.E))
            {
                ui_manger.openOrderingMenu();
            }

            // Ordering menu is open and pressing Q to close menu
            if (canAccessMenu && ui_manger.isOrderingMenuOpen() && Input.GetKeyDown(KeyCode.Q))
            {
                ui_manger.closeOrderingMenu();
            }
        }
    }

    void AccessOrder()
    {
        UpdateOrderSelection();

        if (orderboxBeingHeld)
        {
            AttemptToPutDownOrder();
        }
        else
        {
            AttemptToPickUpOrder();
        }

        AttemptToUpdateOrderDirectionBasedOnMovement();
        AttemptToThrowAwayOrder();
    }

    /*void AttemptToAnimateSelectedPickUpOrder()
    {
        if (canPickUpOrder && !holdingOrder && orderBoxInRange)
        {
            if (!orderBoxInRange.canOrderBePickedUp())
            {
                findNextOrderReadyForPickUp();
            }

            if (orderBoxInRange.canOrderBePickedUp())
            {
                orderBoxInRange.turnOnAnimatedPickUpBackground();
            }
        }
    }*/

    void AttemptToPickUpOrder()
    {
        //bool successfullyPickedUp = false;
        // If Im within range to pick up order and not already holding an order and press E
        // then pick up order
        if (canPickUpOrder && !holdingOrder && orderBoxInRange &&  Input.GetKeyDown(KeyCode.E))
        {
            if (orderBoxInRange.canOrderBePickedUp())
            {
                GameObject newOrderBox = orderBoxInRange.pickUpOrder(gameObject.transform);

                holdingOrder = (newOrderBox != null);
                canPickUpOrder = !holdingOrder;
                orderboxBeingHeld = holdingOrder ? newOrderBox.transform : null;
            }
        }
    }

    void AttemptToPutDownOrder()
    {
        // If Im within range to pick up order and already holding an order and press E
        // then put down order
        print("Can Put down: " + (holdingOrder && orderboxBeingHeld && orderboxParent && orderBoxInRange).ToString());

        if (holdingOrder && orderboxBeingHeld && orderboxParent && orderBoxInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (orderBoxInRange.isOrderAvailable())
            {
                orderBoxInRange.placeOrderOnCounter(orderboxBeingHeld);
                //orderboxBeingHeld.SetParent(orderboxParent.transform);
                //orderboxBeingHeld.transform.localPosition = Vector3.zero;
                canPickUpOrder = true;
                holdingOrder = false;
                orderboxBeingHeld = null;
                //orderBoxInRange.turnOnAnimatedPickUpBackground();
            }
        }
    }

    // Determines which parent box gets animated.
    void UpdateOrderSelection()
    {
        if (collisionDetectionChange)
        {
            turnAnimationOffForAllCollisions();
            collisionDetectionChange = false;
            GameObject newBox = null;

            if (holdingOrder && orderboxBeingHeld)
            {
                // find the new box that is closer to the player
                foreach (GameObject box in orderBoxInCloseRangeCollision)
                {
                    OrderBoxManager order_box = box.GetComponent<OrderBoxManager>();

                    if (order_box.isOrderAvailable() && isNewBoxCloserCompare(newBox, box))
                    {
                        newBox = box;
                    }
                }

                if (newBox != null)
                {
                    orderboxParent = newBox;
                    orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();
                    orderBoxInRange.turnOnAnimatedPutDownBackground();
                }
            }
            else // if not holdiing an order
            {
                // find the new box that is closer to the player
                foreach (GameObject box in orderBoxInCloseRangeCollision)
                {
                    OrderBoxManager order_box = box.GetComponent<OrderBoxManager>();

                    if (order_box.canOrderBePickedUp() && isNewBoxCloserCompare(newBox, box))
                    {
                        newBox = box;
                    }
                }

                if (newBox != null)
                {
                    orderboxParent = newBox;
                    orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();
                    orderBoxInRange.turnOnAnimatedPickUpBackground();
                    canPickUpOrder = true;
                }
            }
        }       
    }

    void AttemptToUpdateOrderDirectionBasedOnMovement()
    {
        // Update the direction of the order the playe is holding
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

    void AttemptToThrowAwayOrder()
    {
        if (canThrowAwayOrder && holdingOrder && orderboxBeingHeld && Input.GetKeyDown(KeyCode.E))
        {
            emptyHands();
        }
    }

    /*void findNextOrderReadyForPickUp()
    {
        // search from back to front of list
        for (int index = orderBoxInRangeCollision.Count - 1; index >= 0; index--)
        {
            print(index.ToString());
            if (orderBoxInRangeCollision[index].GetComponent<OrderBoxManager>().canOrderBePickedUp())
            {
                orderboxParent = orderBoxInRangeCollision[index];
                orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();
                break;
            }
            //canOrderBePickedUp
        }
    }*/

    void turnAnimationOffForAllCollisions()
    {
        foreach (GameObject box in orderBoxInCloseRangeCollision)
        {
            box.GetComponent<OrderBoxManager>().turnOffAnimatedBackground();
        }
    }

    // For on trigger enter
    bool isNewBoxCloser(GameObject newBox)
    {
        if (orderboxParent == null)
        { return true; }

        float old_distance = Vector2.Distance(transform.position, orderboxParent.transform.position);
        float new_distance = Vector2.Distance(transform.position, newBox.transform.position);

        if (new_distance < old_distance)
            return true;

        return false;
    }

    // For comparing two boxes distances from player
    bool isNewBoxCloserCompare(GameObject oldBox, GameObject newBox)
    {
        if (oldBox == null)
        { return true; }

        float old_distance = Vector2.Distance(transform.position, oldBox.transform.position);
        float new_distance = Vector2.Distance(transform.position, newBox.transform.position);

        if (new_distance < old_distance)
            return true;

        return false;
    }

    GameObject findNewCloserBox()
    {
        if (orderBoxInCloseRangeCollision.Count == 0)
        { return null; }

        float shortest_distance = 1000f;
        GameObject closest_object = null;

        foreach (GameObject gobj in orderBoxInCloseRangeCollision)
        {
            float dist = Vector2.Distance(transform.position, gobj.transform.position);

            if (dist <= shortest_distance)
            {
                shortest_distance = dist;
                closest_object = gobj;
            }
        }

        return closest_object;
    }

    //AKA throw away the order
    public void emptyHands()
    {
        holdingOrder = false;
        Destroy(orderboxBeingHeld.gameObject, 0.1f); //destroy in X time
        orderboxBeingHeld = null;
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
            /*ATTTEMPT 1
            canPickUpOrder = true;
            orderboxParent = collision.gameObject;

            if (!orderBoxInRangeCollision.Contains(orderboxParent))
            {
                orderBoxInRangeCollision.Add(orderboxParent);
            }
            orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();*/



            /*ATTEMPT 2
            OrderBox boxParent = collision.gameObject.GetComponent<OrderBoxManager>();

            if (boxParent.canOrderBePickedUp() && !holdingOrder)
            {
                if (!orderBoxInCloseRangeCollision.Contains(collision.gameObject))
                {
                    orderBoxInCloseRangeCollision.Add(collision.gameObject);
                }

                if (isNewBoxCloser(collision.gameObject))
                {
                    canPickUpOrder = true;
                    orderboxParent = collision.gameObject;
                    orderBoxInRange = boxParent;//orderboxParent.GetComponent<OrderBoxManager>();
                    orderBoxInRange.turnOnAnimatedPickUpBackground();
                }
            }
            else if (holdingOrder && boxParent.isOrderAvailable())
            {
                if (!orderBoxInCloseRangeCollision.Contains(collision.gameObject))
                {
                    orderBoxInCloseRangeCollision.Add(collision.gameObject);
                }

                if (isNewBoxCloser(collision.gameObject))
                {
                    orderboxParent = collision.gameObject;
                    orderBoxInRange = boxParent;//orderboxParent.GetComponent<OrderBoxManager>();
                    orderBoxInRange.turnOnAnimatedPutDownBackground();
                }
            }*/

            if (!orderBoxInCloseRangeCollision.Contains(collision.gameObject))
            {
                orderBoxInCloseRangeCollision.Add(collision.gameObject);
                collisionDetectionChange = true;
            }
        }
        else if (collision.tag == "trash can")
        {
            canThrowAwayOrder = true;
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color32(170, 170, 170, 255);
            print("Entered trash can");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        print("Leaving: " + collision.name);
        if (collision.tag == "computer")
        {
            canAccessMenu = false;
        }
        else if (collision.tag == "order_box_parent")
        {
            /* ATTEMPT 1
            canPickUpOrder = false;
            orderBoxInRange.turnOffAnimatedBackground();
            orderBoxInRange = null;
            
            if (orderBoxInRangeCollision.Contains(collision.gameObject))
            {
                orderBoxInRangeCollision.Remove(collision.gameObject);
                orderboxParent = null;
            }

            // Get the last collided orderbox
            if (orderBoxInRangeCollision.Count > 0 && orderboxParent == null)
            {
                orderboxParent = orderBoxInRangeCollision[orderBoxInRangeCollision.Count - 1];
                orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();
            }*/

            /* ATTEMPT 2
            if (orderBoxInCloseRangeCollision.Contains(collision.gameObject))
            {
                orderBoxInCloseRangeCollision.Remove(collision.gameObject);
                collision.gameObject.GetComponent<OrderBoxManager>().turnOffAnimatedBackground();

                if (GameObject.ReferenceEquals(orderboxParent, collision.gameObject))
                {
                    if (orderBoxInRange)
                    { orderBoxInRange.turnOffAnimatedBackground(); }
                    
                    orderBoxInRange = null;

                    // could be set to null
                    orderboxParent = findNewCloserBox();

                    if (orderboxParent != null)
                    {
                        orderBoxInRange = orderboxParent.GetComponent<OrderBoxManager>();

                        if (holdingOrder)
                        {
                            orderBoxInRange.turnOnAnimatedPutDownBackground();
                        }
                        else 
                        {
                            orderBoxInRange.turnOnAnimatedPickUpBackground();
                        }
                    }
                }
            }*/
            if (orderBoxInCloseRangeCollision.Contains(collision.gameObject))
            {
                orderBoxInCloseRangeCollision.Remove(collision.gameObject);
                collision.gameObject.GetComponent<OrderBoxManager>().turnOffAnimatedBackground();

                if (orderBoxInCloseRangeCollision.Count == 0)
                {
                    orderboxParent = null;
                    orderBoxInRange = null;
                }

                collisionDetectionChange = true;
            }
        }
        else if (collision.tag == "trash can")
        {
            canThrowAwayOrder = false;
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            print("leaving trash can");
        }
    }

}
