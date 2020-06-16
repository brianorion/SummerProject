using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField]
    private GameObject thirdPersonCamera;

    private PlayerController playerController;
    private GameObject thirdPersonCameraPrefab;

    private void Awake()
    {
        thirdPersonCameraPrefab = Instantiate(thirdPersonCamera);
        Debug.Log("Instantiated this prefab");
        thirdPersonCameraPrefab.SetActive(false);
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        InThirdPerson();
        CameraFollow();
    }

    private void InThirdPerson()
    {
        if(!playerController.inFirstPerson)
        {
            thirdPersonCameraPrefab.SetActive(true);
            
            // this disables the crosshair in the canvas when we are in third person mode.
            GameCanvas.canvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            thirdPersonCameraPrefab.SetActive(false);

            // we want to enable the crosshair in the canvas when we are in first person mode. 
            GameCanvas.canvas.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void CameraFollow()
    {
        thirdPersonCameraPrefab.transform.position = transform.position;
    }
}