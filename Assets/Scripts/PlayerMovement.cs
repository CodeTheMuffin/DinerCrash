using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Stats GameStats;
    //public CharacterController2D controller;
    public Animator animator;
    public TextSystem txtSys;

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
    bool NPCcollisionDetectionChange = false;
    NPC selectedNPC;

    List<NPC> NPCsInCloseRangeCollision = new List<NPC>();
    Color32 normal_trash_can_color = new Color32(255,255,255, 255);
    Color32 highlighted_trash_can_color = new Color32(170, 170, 170, 255);

    public SpriteRenderer trashcan;

    AudioManager audio_manager;

    public bool testCollision = true;

    enum LookingDirection
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }

    private void Awake()
    {
        original_position = playerTransform.position;
        audio_manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        //debug
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }*/

        //ESC
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (!GameStats.isGameOver)
        {
            UpdateNPCSelection();
            AccessNPC();
            AccessOrder();
            AccessPC();
        }
    }

    private void FixedUpdate()
    {
        if (!GameStats.isGameOver)
        {
            Move();
        }
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
        if (canAccessMenu)
        {
            // Going to the computer and pressing E to open ordering menu
            if (!ui_manger.isOrderingMenuOpen() && !selectedNPC && Input.GetKeyDown(KeyCode.E))
            {
                bool capacityToOrder = ui_manger.haveCapacityToOrder();

                if (!holdingOrder && capacityToOrder)
                { ui_manger.openOrderingMenu(); }
                else
                {
                    audio_manager.playUI_denied();
                    if (holdingOrder)
                    {
                        ui_manger.text_sys.printWarningText("WARNING_NoPC_NoFreeHands");
                    }
                    else if (!capacityToOrder)
                    {
                        ui_manger.text_sys.printWarningText("WARNING_NoPC_NoRoom");
                    }
                }
            }
            // Ordering menu is open 
            else if (ui_manger.isOrderingMenuOpen())
            {
                if (Input.GetKeyDown(KeyCode.Q)) //pressing Q to close menu
                {
                    ui_manger.closeOrderingMenu();
                }
                else if (Input.GetButtonDown("Jump"))//space bar to process order
                {
                    ui_manger.process_order();
                }
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

    void AccessNPC()
    {
        //selected NPC should only have ready_to_order as true
        if (selectedNPC) // TODO: prepare ordering for NPC
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (holdingOrder)
                {
                    if (selectedNPC.wasOrderPlaced)
                    {
                        // give NPC the order
                        selectedNPC.receiveOrderFromPlayer(orderboxBeingHeld.GetComponent<OrderBox>().orderForm);
                        emptyHands();
                        selectedNPC.prepareForExitting();
                    }
                }
                else // if not holding anything
                {
                    if (selectedNPC.ready_to_order)
                    {
                        if (!selectedNPC.wasOrderPlaced)
                        {
                            // get order
                            //print("Order received.");
                            //print(selectedNPC.getExpectedForm());
                            //print(selectedNPC.request_text.Replace("\n"," "));
                            selectedNPC.tellPlayerOrder();
                            selectedNPC.prepareForStanding();
                        }
                        else // order was placed
                        {
                            // maybe show what they want/ asked for again in text box??
                            selectedNPC.repeatOrder();
                        }
                    }
                }

                /*if (selectedNPC.ready_to_order)
                {
                    if (!selectedNPC.wasOrderPlaced)
                    {
                        // get order
                        print("Order received.");
                        selectedNPC.prepareForStanding();
                    }
                    else //order was already placed
                    {
                        if (holdingOrder)
                        {
                            
                        }
                        selectedNPC.prepareForExitting();
                    }
                }*/
                
            }
        }
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
        if (canPickUpOrder && !holdingOrder && orderBoxInRange && !selectedNPC && Input.GetKeyDown(KeyCode.E))
        {
            if (orderBoxInRange.canOrderBePickedUp())
            {
                GameObject newOrderBox = orderBoxInRange.pickUpOrder(gameObject.transform);

                holdingOrder = (newOrderBox != null);
                canPickUpOrder = !holdingOrder;
                orderboxBeingHeld = holdingOrder ? newOrderBox.transform : null;

                //print($"Holding order with: {newOrderBox.GetComponent<OrderBox>().orderForm}");

                if (orderboxBeingHeld)
                {
                    showWhatImHolding();
                    audio_manager.playPlayerPickOrderUp();
                }
            }
        }
    }

    void AttemptToPutDownOrder()
    {
        // If Im within range to pick up order and already holding an order and press E
        // then put down order

        if (holdingOrder && orderboxBeingHeld && orderboxParent && orderBoxInRange && !selectedNPC && Input.GetKeyDown(KeyCode.E))
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
                audio_manager.playPlayerPutOrderOnCounter();
            }
        }
    }

    void unhighlightEverything()
    {
        turnAnimationOffForAllCollisions();
        trashcan.color = normal_trash_can_color;
        turnHighlightOffForAll_NPC_Collisions();
    }

    void UpdateNPCSelection()
    {
        if (NPCcollisionDetectionChange)
        {
            NPCcollisionDetectionChange = false;
            //turnAnimationOffForAllCollisions();
            //trashcan.color = normal_trash_can_color;
            unhighlightEverything();

            collisionDetectionChange = false;//// COULD BE A BUG but preventing selecting more than one thing! Priortize NPC collisions
            //turnHighlightOffForAll_NPC_Collisions();

            GameObject newNPC = null;
            NPC newNPC_script = null;
            /*if (holdingOrder && orderboxBeingHeld)
            {

            }*/

            foreach (NPC npc in NPCsInCloseRangeCollision)
            {
                if (isNewObjectCloserCompare(newNPC, npc.gameObject))
                {
                    //NPC npc_script = npc.GetComponent<NPC>();

                    if (holdingOrder)
                    {
                        if (npc.wasOrderPlaced)
                        {
                            newNPC = npc.gameObject;
                            newNPC_script = npc;
                        }
                    }
                    else
                    {
                        if (npc.ready_to_order)//if they were at/touched the order waypoint
                        {
                            newNPC = npc.gameObject;
                            newNPC_script = npc;
                        }
                    }
                }
            }

            if (newNPC_script != null)
            {
                selectedNPC = newNPC_script;
                selectedNPC.turnOnHighlight();
            }
        }
    }

    void showWhatImHolding()
    {
        if (orderboxBeingHeld)
        {
            OrderForm temp_form = orderboxBeingHeld.GetComponent<OrderBox>().orderForm;
            string holding_text= "Holding: ";

            if (temp_form.getTotalQuantity() == 0)
            {
                holding_text += "An empty box.";
            }
            else
            {
                holding_text += temp_form.ToString();
            }

            txtSys.updateNPCtext_unformatted(holding_text);
            //string formatted_holding_text = txtSys.adjustTextRegex(holding_text).Item1;
            //txtSys.updateNPCtext(formatted_holding_text);
        }
    }

    // Determines which parent box gets animated.
    void UpdateOrderSelection()
    {
        if (collisionDetectionChange && !selectedNPC)
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

                    if (order_box.isOrderAvailable() && isNewObjectCloserCompare(newBox, box))
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

                    if (order_box.canOrderBePickedUp() && isNewObjectCloserCompare(newBox, box))
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
        if (canThrowAwayOrder && holdingOrder && orderboxBeingHeld && !selectedNPC && Input.GetKeyDown(KeyCode.E))
        {
            audio_manager.playPlayerThrowOrderAway();
            txtSys.updateNPCtext_unformatted("Trashed order.");
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
    void turnHighlightOffForAll_NPC_Collisions()
    {
        foreach (NPC npc in NPCsInCloseRangeCollision)
        {
            npc.GetComponent<NPC>().turnOffHighlight();
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
    bool isNewObjectCloserCompare(GameObject oldObject, GameObject newObject)
    {
        if (oldObject == null)
        { return true; }

        float old_distance = Vector2.Distance(transform.position, oldObject.transform.position);
        float new_distance = Vector2.Distance(transform.position, newObject.transform.position);

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
        if (testCollision)
        {
            print("Entering: " + collision.name);
        }

        if (collision.tag == "computer")
        {
            canAccessMenu = true;
        }
        else if (collision.tag == "order_box_parent")
        {
            if (!orderBoxInCloseRangeCollision.Contains(collision.gameObject))
            {
                orderBoxInCloseRangeCollision.Add(collision.gameObject);
                collisionDetectionChange = true;
            }
        }
        else if (collision.tag == "npc")
        {
            NPC npc_script = collision.gameObject.GetComponent<NPC>();
            if (!NPCsInCloseRangeCollision.Contains(npc_script))
            {
                NPCsInCloseRangeCollision.Add(npc_script);
                NPCcollisionDetectionChange = true;
            }
        }
        else if (collision.tag == "trash can")
        {
            if (!selectedNPC)
            {
                canThrowAwayOrder = true;
                collision.gameObject.GetComponent<SpriteRenderer>().color = highlighted_trash_can_color;
            }
            if (testCollision)
            {
                print("Entered trash can");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (testCollision)
        {
            print("Leaving: " + collision.name);
        }

        if (collision.tag == "computer")
        {
            canAccessMenu = false;
        }
        else if (collision.tag == "order_box_parent")
        {
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
        else if (collision.tag == "npc")
        {
            NPC npc_script = collision.gameObject.GetComponent<NPC>();
            if (NPCsInCloseRangeCollision.Contains(npc_script))
            {
                NPCsInCloseRangeCollision.Remove(npc_script);
                collision.gameObject.GetComponent<NPC>().turnOffHighlight();
            }

            if (NPCsInCloseRangeCollision.Count == 0)
            {
                selectedNPC = null;
            }

            NPCcollisionDetectionChange = true;
        }
        else if (collision.tag == "trash can")
        {
            canThrowAwayOrder = false;
            collision.gameObject.GetComponent<SpriteRenderer>().color = normal_trash_can_color;
            if (testCollision)
            {
                print("leaving trash can");
            }
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (testCollision)
        {
            print("Entering Collision: " + collision.collider.name);
        }

        if (collision.collider.tag == "npc")
        {
            if (!NPCsInCloseRangeCollision.Contains(collision.gameObject))
            {
                NPCsInCloseRangeCollision.Add(collision.gameObject);
                NPCcollisionDetectionChange = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (testCollision)
        {
            print("Leaving Collision: " + collision.collider.name);
        }

        if (collision.collider.tag == "npc")
        {
            if (NPCsInCloseRangeCollision.Contains(collision.gameObject))
            {
                NPCsInCloseRangeCollision.Remove(collision.gameObject);
                collision.gameObject.GetComponent<NPC>().turnOffHighlight();
            }

            if (NPCsInCloseRangeCollision.Count == 0)
            {
                selectedNPC = null;
            }

            NPCcollisionDetectionChange = true;
        }
    }*/
}