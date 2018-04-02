using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

    // Game Manager reference variable
    private GameManager GM;
    // Character controller game object, Camera and network hud
    private CharacterController CC;
    private Camera playerCam = null;
    private NetworkManagerHUD NetHud;
    // Vector that holds the direction the player moves along
    private Vector3 moveDirection = Vector3.zero;
    // Float for setting the characters speed and movement
    public float fl_movementSpeed;
    private float vertical;
    private float horizontal;
    // Float for setting the sensitivity of looking around
    public float fl_lookSensitivity;
    // Float variables for looking along the x and y axis
    private float fl_lookX = 0.0f;
    private float fl_lookY = 0.0f;
    // Float for storing the current x axis rotation of the camera
    private float fl_currentLookX;
    // Float for the height at which the player can jump
    private float fl_jumpHeight;
    // Float for the rate at which the player falls due to gravity
    private float fl_gravity;
    // Variables for object carrying logic
    private bool bl_carryingObject = false;
    private GameObject objectID;
    private NetworkIdentity playerID;

    // Use this for initialization
    void Start () {

        if (isLocalPlayer)
        {
            // Retrieves all components for current local player
            GM = GameManager.instance;
            CC = GetComponent<CharacterController>();
            NetHud = FindObjectOfType<NetworkManagerHUD>();
            playerCam = GetComponentInChildren<Camera>();
            playerID = GetComponent<NetworkIdentity>();
            // Disables scene elements
            NetHud.enabled = false;
            Camera.main.gameObject.SetActive(false);
        }

        Initialise();
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer) return;
        if (GM.BL_Paused) return;
        
        Interact();
        LookAround();
        Movement();
        CmdCarryObject(objectID);
    }

    private void Initialise()
    {
        if (isLocalPlayer)
        {
            // Assigns all default variables
            fl_movementSpeed = 4f;
            fl_lookSensitivity = 2f;
            fl_jumpHeight = 8f;
            fl_gravity = 20f;
        }            
        else
        {
            // Removes unneccessary components for other players on the host
            GameObject camera = transform.GetChild(0).gameObject;
            camera.SetActive(false);
            return;
        }        
    }

    private void LookAround()
    {
        if (GameManager.instance.BL_MobileMode)
        {
            // Stores the current location of the mouse's x axis
            fl_currentLookX = CrossPlatformInputManager.GetAxis("Look X") * fl_lookSensitivity;
            // Increments the x axis location from previous horizontal mouse movement (multiplied by sensitivity)
            fl_lookX += CrossPlatformInputManager.GetAxis("Look X") * fl_lookSensitivity;
            // Increments the y axis location from previous vertical mouse movement (multiplied by sensitivity)
            fl_lookY += CrossPlatformInputManager.GetAxis("Look Y") * fl_lookSensitivity;
        }
        else
        {
            // Stores the current location of the mouse's x axis
            fl_currentLookX = Input.GetAxis("Mouse X") * fl_lookSensitivity;
            // Increments the x axis location from previous horizontal mouse movement (multiplied by sensitivity)
            fl_lookX += Input.GetAxis("Mouse X") * fl_lookSensitivity;
            // Increments the y axis location from previous vertical mouse movement (multiplied by sensitivity)
            fl_lookY += Input.GetAxis("Mouse Y") * fl_lookSensitivity;
        }       

        // Clamps the vertical movement to a range of -70 and 70 degrees
        fl_lookY = Mathf.Clamp(fl_lookY, -60, 60);

        // Rotates the character depeding on the position they are facing along the x axis
        transform.Rotate(0, fl_currentLookX, 0);
        // Sets the camera rotation variables using its eulerAngles (inverting the Y axis)
        playerCam.transform.eulerAngles = new Vector3(-fl_lookY, fl_lookX, 0);
        if (!GameManager.instance.BL_MobileMode) Cursor.lockState = CursorLockMode.Locked;

    }

    private void Movement()
    {
        // Checks if the character controller is grounded against a collider
        if (CC.isGrounded)
        {
            if (GameManager.instance.BL_MobileMode)
            {
                // Float variables that hold the vertical and horizontal movement (yaw and pitch) multiplied by speed
                vertical = CrossPlatformInputManager.GetAxis("Vertical") * fl_movementSpeed;
                horizontal = CrossPlatformInputManager.GetAxis("Horizontal") * fl_movementSpeed;
            }
            else
            {
                // Float variables that hold the vertical and horizontal movement (yaw and pitch) multiplied by speed
                vertical = Input.GetAxis("Vertical") * fl_movementSpeed;
                horizontal = Input.GetAxis("Horizontal") * fl_movementSpeed;
            }           

            // Assigns the variables to their appropriate vector3 fields which will then be set to the move direction vector
            moveDirection = new Vector3(horizontal, 0, vertical);
            // Combines player movement and rotation
            moveDirection = transform.rotation * moveDirection;

            // Checks for jump input
            if (CrossPlatformInputManager.GetButton("Jump"))
            {
                // Sets the y position of the move direction vector equal to the jump height
                moveDirection.y = fl_jumpHeight;
            }
        }
        else
        {
            // Reduces the jump height by the gravity rate after each frame
            moveDirection.y -= fl_gravity * Time.deltaTime;
        }

        // Moves the character using the Move() method in the character controller (multiplied by delta time)
        CC.Move(moveDirection * Time.deltaTime);
    }

    private void Interact()
    {
        // Checks for input
        if (Input.GetMouseButtonDown(0))
        {
            // Generates raycast at the mouse position
            RaycastHit hit;
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 2f))
            {
                // Handles logic for carrying objects
                if (hit.collider.tag == "Object")
                {
                    if (!bl_carryingObject)
                    {
                        objectID = hit.collider.gameObject;
                        objectID.GetComponent<Rigidbody>().isKinematic = true;
                        CmdSetAuth(objectID, playerID);
                        bl_carryingObject = true;                        
                    }
                    else
                    {
                        DropObject(objectID);                       
                        bl_carryingObject = false;
                    }
                }
            }
        }        
        
    }

    [Command]
    public void CmdSetAuth(GameObject objectId, NetworkIdentity player)
    {
        // Retrieves the network identity of the object and the other client owner
        var networkIdentity = objectId.GetComponent<NetworkIdentity>();
        var otherOwner = networkIdentity.clientAuthorityOwner;

        // Returns the function if the player's network identity is the same as the current objects owner
        if (otherOwner == player.connectionToClient)
        {
            return;
        }
        else
        {
            // Removes client authority if there is another client with authority over the object
            if (otherOwner != null)
            {
                networkIdentity.RemoveClientAuthority(otherOwner);
            }
            // Assigns client authority to the player's client connection
            networkIdentity.AssignClientAuthority(player.connectionToClient);
        }
    }


    [Command]
    private void CmdCarryObject(GameObject obj)
    {       
        // Checks if object is not null before calling an RPC function so that all clients can see the changes
        if (obj == null) return;
        RpcCarryObject(obj);
    }

    [ClientRpc]
    private void RpcCarryObject(GameObject obj)
    {
        if (bl_carryingObject)
        {
            // Holds the object at the center position relative to the camera            
            obj.transform.position = playerCam.transform.position + playerCam.transform.forward * 2f;
        }
    }

    private void DropObject(GameObject obj)
    {
        // Checks if the player is carrying object and that the object is not null
        if (obj != null)
        {
            // Restores physics to the object and adds a slight push force when released
            Rigidbody _RB = obj.GetComponent<Rigidbody>();
            _RB.isKinematic = false;
            _RB.AddForce(playerCam.transform.forward * 100f, ForceMode.Force);
        }
    }
}
