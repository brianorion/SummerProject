using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Player stats")]
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float rotationSensitivity = 1f;
    [SerializeField]
    private float jumpForce = 1f;
    [SerializeField]
    private float maximumCameraRotation = 90f;
    [SerializeField]
    private float pickUpDistance = 5f;
    [SerializeField][Range(0, 1)]
    private float inAirSpeedDebuffRatio = 0.5f;

    [Header("Associated Objects")]
    [SerializeField]
    private GameObject firstPersonCamera;
    [SerializeField]
    private Transform guide;

    private Rigidbody rb;
    private Inventory inventory;

    // these are all of the item pick up
    private Rigidbody itemRb;
    private Transform itemTransform;

    private bool changeState = false;
    private bool canPickUp = false;
    private bool inAir = false;
    private GameObject lastDropped;

    private float initialSpeed;
    private float debuffSpeed;
    private Vector3 jumpVector;

    public bool inFirstPerson = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        firstPersonCamera.SetActive(inFirstPerson);
        jumpVector = new Vector3(0f, jumpForce, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        initialSpeed = speed;
        debuffSpeed = speed * inAirSpeedDebuffRatio;
    }

    private void Update()
    {
        // we move no matter what state the character is in.
        Move();

        // this checks if we are in first person or third person. 
        if (Input.GetKeyDown(KeyCode.C) && !changeState)
        {
            changeState = true;
            inFirstPerson = false;
        }
        else if (Input.GetKeyDown(KeyCode.C) && changeState)
        {
            changeState = false;
            inFirstPerson = true;
        }
        IsFirstPerson(inFirstPerson);

        if (Input.GetKeyDown(KeyCode.G))
        {
            lastDropped = inventory.GetItemByName(guide.position, "Mystic Shield");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Vector3 inFront = firstPersonCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2.0f));
            lastDropped = inventory.GetItemByName(inFront, "Grenade");
            if (lastDropped != null)
            {
                if (lastDropped.GetComponent<Projectile>() != null)
                {
                    Projectile projectileScript = lastDropped.GetComponent<Projectile>();
                    projectileScript.ThrowItem(firstPersonCamera.transform.rotation);
                }
            }
        }
    }


    private void IsFirstPerson(bool firstPerson)
    {
        if (firstPerson)
        {
            Cursor.lockState = CursorLockMode.Locked;
            PlayerRotate();
            ClickAndDrag();
            firstPersonCamera.SetActive(firstPerson);
        }
        else
        {
            // TODO maybe in third person mode there might be something different. 
            ThirdPersonMode();
            firstPersonCamera.SetActive(firstPerson);
        }
    }

    private void Move()
    {
        // if we are in air
        if (inAir)
        {
            speed = debuffSpeed;
        }
        else
        {
            speed = initialSpeed;
        }
        if (inFirstPerson)
        {
            // this ensures that the player will face towards and walk towards the forward or right direction of the camera. 
            Vector3 camF = firstPersonCamera.transform.forward;
            Vector3 camR = firstPersonCamera.transform.right;

            camF.y = 0;
            camR.y = 0f;
            camF = camF.normalized;
            camR = camR.normalized;

            Vector3 velocity = camF * Input.GetAxis("Vertical") + camR * Input.GetAxis("Horizontal");
            velocity = velocity.normalized * speed;

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        else
        {
            // in third person, we only need to move in terms of the world axis. We don't need to worry about the position of the camera. 
            Vector3 velocity = Vector3.zero;
            velocity.x = Input.GetAxis("Horizontal");
            velocity.z = Input.GetAxis("Vertical");

            velocity = velocity.normalized * speed;

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        // the jumping functionality doesn't depend on whether or not the player is in first person or in third person. Thus, I am doing it outside of the if statement. 
        if (Input.GetKeyDown(KeyCode.Space) && !inAir)
        {
            // At the moment, a person can have infinite jumps. Too lazy to fix it so I will do it later. 
            rb.AddForce(jumpVector, ForceMode.Impulse);
            inAir = true;
        }
    }

    private void PlayerRotate()
    {
        float yRot = Input.GetAxis("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * rotationSensitivity;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (firstPersonCamera != null)
        {
            CameraRotate();
        }
    }
    
    private void CameraRotate()
    {
        
        float xRot = Input.GetAxis("Mouse Y");

        Vector3 rotation = new Vector3(xRot, 0f, 0f) * rotationSensitivity;

        firstPersonCamera.transform.Rotate(-rotation);
        float currentX = firstPersonCamera.transform.localEulerAngles.x;
        currentX = Mathf.Clamp(currentX, -270f, maximumCameraRotation);

        firstPersonCamera.transform.localEulerAngles = new Vector3(currentX, 0f, 0f);
    }

    private void ThirdPersonMode()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void ClickAndDrag()
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = firstPersonCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.distance <= pickUpDistance)
                {
                    PickUpItems(hit);
                }
            }
        }
        else if(Input.GetMouseButtonUp(0) && canPickUp)
        {
            if (itemRb != null)
            {
                canPickUp = false;
                itemRb.useGravity = true;
                itemRb.isKinematic = false;
                itemTransform.position = guide.position;
                itemTransform.parent = null;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            inAir = false;
        }
    }

    /// <summary>
    /// This method deals with the storing part of the grabbing and dragging method. 
    /// It checks whether or not the player clicked an item or if they clicked a ground. 
    /// </summary>
    /// <param name="hit">this variable stores data about the object that the player has just hit. </param>
    private void PickUpItems(RaycastHit hit)
    {
        // if the thing that has been selected is an Item.
        if (hit.transform.tag == "Item")
        {
            // we need to notify the Inventory script that we have collected an item.
            Debug.Log("Picked up an Item");
            Item item = hit.transform.GetComponent<Item>();
            Debug.Log(item.name);
            inventory.UpdateItem(item);
            Destroy(hit.transform.gameObject);
        }
        else if (hit.transform.tag != "Ground")
        {
            Debug.Log("I pick up something");
            canPickUp = true;
            itemRb = hit.rigidbody;
            itemTransform = hit.transform;


            itemRb.useGravity = false;
            itemRb.isKinematic = true;
            itemTransform.position = guide.position;
            itemTransform.rotation = guide.rotation;
            itemTransform.parent = guide;
        }
    }

}
