using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class FirstPersonCamera : MonoBehaviour
{
    public CinemachineCamera cam;
    
    // references to gameobjects to look at
    public Transform defaultLookAt;
    public Transform cardLookAt;
    public Transform deadLookAt;
    public GameObject poison;
    public GameObject poison1;
    public GameObject revolver;
    
    public bool focusCenter, focusCards, focusPoison, focus2ndPoison, focusRevolver, focusDead, isFocusing, lockCam;
    private float fov;

    // raycast
    public float detectionRange = 5.0f;
    public LayerMask optionLayerMask;

    // references to other script
    private CinemachineRotationComposer crc;
    private GameManager gm;


    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        crc = FindObjectOfType<CinemachineRotationComposer>();

        // Lock and Hide the Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        focusCenter = true;
        focusCards = false;
        focusPoison = false;
        focus2ndPoison = false;
        focusRevolver = false;
        focusDead = false;
        isFocusing = false;
        lockCam = false;
        fov = 40f;
        
        // sets up camera in the right direction and add dampening
        cam.LookAt = defaultLookAt.transform;
        AddDampening();
    }


    // Update is called once per frame
    void Update()
    {
        DetectItem();

        // when d is hold down, will focus on 1st poison if it is active
        if(Input.GetKeyDown(KeyCode.D) && !focusRevolver && (!focusPoison || !focus2ndPoison) && !lockCam && gm.playerLost)
        {
            if(poison.activeSelf == false)
            {
                FocusOn2ndPoison();
                // items ui
                gm.waitingForPlayerUI.SetActive(false);
                gm.poisonUIInteract.SetActive(true);
            }
            else
            {   
                FocusOnPoison();
                // items ui
                gm.waitingForPlayerUI.SetActive(false);
                gm.poisonUIInteract.SetActive(true);
            }
            
        }

        // when d is released, will unfocused on the poison
        if(Input.GetKeyUp(KeyCode.D) && !focusRevolver && (focusPoison || focus2ndPoison) && !lockCam && gm.playerLost)
        {
            if(poison.activeSelf == false)
            {
                UnFocusOn2ndPoison();
                // items ui
                gm.waitingForPlayerUI.SetActive(true);
                gm.poisonUIInteract.SetActive(false);
            }
            else
            {   
                UnFocusOnPoison();
                // items ui
                gm.waitingForPlayerUI.SetActive(true);
                gm.poisonUIInteract.SetActive(false);
            }
            
        }

        // when a is pressed down, will focus on gun
        if(Input.GetKeyDown(KeyCode.A) && !focusRevolver && (!focusPoison || !focus2ndPoison) && !lockCam && gm.playerLost)
        {
            FocusOnGun();
            // items ui
            gm.waitingForPlayerUI.SetActive(false);
            gm.revolverUIInteract.SetActive(true);
            
        }

        // when a is released, will unfocus on gun
        if(Input.GetKeyUp(KeyCode.A) && focusRevolver && (!focusPoison || !focus2ndPoison) && !lockCam && gm.playerLost)
        {
            UnFocusOnGun();
            gm.waitingForPlayerUI.SetActive(true);
            gm.revolverUIInteract.SetActive(false);
        } 

        cam.Lens.FieldOfView = fov;
        if(isFocusing)
        {
            if(fov > 30)
            {
                fov -= 20*Time.deltaTime;
            }
        }
        
        if(!isFocusing)
        {
            if(fov < 40)
            {
                fov += 35*Time.deltaTime;
            }
        }

        if(focusCenter)
        {
            cam.LookAt = defaultLookAt.transform;
        }

        if(focusCards)
        {
            cam.LookAt = cardLookAt.transform;
        }

        if(focusPoison)
        {
            cam.LookAt = poison.transform;
        }

        if(focus2ndPoison)
        {
            cam.LookAt = poison1.transform;
        }

        if(focusRevolver)
        {
            cam.LookAt = revolver.transform;
        }

        if(focusDead)
        {
            cam.LookAt = deadLookAt.transform;
        }

        if(!poison.activeSelf && !poison1.activeSelf)
        {
            gm.poisonUIInteractD.SetActive(false);
        }
    }

    void DetectItem()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, detectionRange, optionLayerMask))
        {
            //Debug.Log("Item detected: " + hit.collider.gameObject.name);

            if(Input.GetKeyDown(KeyCode.E) && hit.collider.gameObject.name == "Bottle1") 
            {
                hit.collider.gameObject.GetComponent<Poison>().UsePoison();
                // dissable items ui
                gm.poisonUIInteract.SetActive(false);
            }

            if(Input.GetKeyDown(KeyCode.E) && hit.collider.gameObject.name == "Bottle2") 
            {
                hit.collider.gameObject.GetComponent<Poison1>().UsePoison();
                // dissable items ui
                gm.poisonUIInteract.SetActive(false);
            }

            if(Input.GetKeyDown(KeyCode.E) && hit.collider.gameObject.name == "Revolver") 
            {
                hit.collider.gameObject.GetComponent<RevolverRoulette>().UseGun();
                // dissable items ui
                gm.revolverUIInteract.SetActive(false);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * detectionRange);
    }

    public void RemoveDampening()
    {
        crc.Damping = new Vector2(0.5f,0.5f);
    }

    public void AddDampening()
    {
        crc.Damping = new Vector2(2f,2f);
    }

    public void FocusOnPoison()
    {
        isFocusing = true;
        focusCenter = false;
        focusPoison = true;
    }

    public void UnFocusOnPoison()
    {
        isFocusing = false;
        focusCenter = true;
        focusPoison = false;
    }

    public void FocusOn2ndPoison()
    {
        isFocusing = true;
        focusCenter = false;
        focus2ndPoison = true;
    }

    public void UnFocusOn2ndPoison()
    {
        isFocusing = false;
        focusCenter = true;
        focus2ndPoison = false;
    }

    public void FocusOnGun()
    {
        isFocusing = true;
        focusCenter = false;
        focusRevolver = true;
    }

    public void UnFocusOnGun()
    {
        isFocusing = false;
        focusCenter = true;
        focusRevolver = false;
    }

    public void FocusDead()
    {
        focusCenter = false;
        focusDead = true;
    }

    public void FocusOnCards()
    {
        isFocusing = true;
        focusCenter = false;
        focusCards = true;
    }

    public void UnFocusOnCards()
    {
        isFocusing = false;
        focusCenter = true;
        focusCards = false;
    }
}
