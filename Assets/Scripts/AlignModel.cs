using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class AlignModel : MonoBehaviour
{
    public Transform mainCamera;

    public GameObject alignmentObject;
    public ARPlaneManager m_ARPlaneManager;

    public InputActionReference rotateAction;
    public GameObject turnProviders;

    public float rotationSpeed = 5f;

    private GameObject _spawnedPrefab;
    private GameObject _spawnedARelements;

    private XRGrabInteractable _grabInteractable;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").transform;

        _grabInteractable = alignmentObject.GetComponent<XRGrabInteractable>();

        _grabInteractable.selectEntered.AddListener(OnSelectEntered);
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    public void OnClickAlignModel()
    {
        StartCoroutine(TurnOnAlignmentPrefab());
    }

    private IEnumerator TurnOnAlignmentPrefab()
    {
        if (!m_ARPlaneManager.enabled)
        {
            m_ARPlaneManager.enabled = true;
            yield return new WaitForSeconds(1f);
        }

        // Find the lowest horizontal plane
        ARPlane floorPlane = null;
        float lowestY = float.MaxValue;

        foreach (var plane in m_ARPlaneManager.trackables)
        {
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            {
                if (plane.transform.position.y < lowestY) // Check for lowest plane
                {
                    lowestY = plane.transform.position.y;
                    floorPlane = plane;
                }
            }
        }

        // Turn on the alignment object
        alignmentObject.SetActive(true);

        // move alignment object 2 meters in front of the camera
        alignmentObject.transform.position = mainCamera.position + mainCamera.forward * 2f;

        // move it to 0 height for now
        alignmentObject.transform.position = new Vector3(alignmentObject.transform.position.x, 0, alignmentObject.transform.position.z);


        // Ensure a valid plane is found
        if (floorPlane != null)
        {
            // Align the prefab to the floor plane height
            alignmentObject.transform.position = new Vector3(alignmentObject.transform.position.x, floorPlane.transform.position.y, alignmentObject.transform.position.z);
                     
            Debug.Log($"Spawned prefab at {floorPlane.transform.position}");
        }
        else
        {
            Debug.LogWarning("No suitable floor plane detected.");
        }
    }

    public void OnClickReset()
    {
        //
    }

    private void Update()
    {
        if (turnProviders.activeInHierarchy == false)
        {
            Vector2 input = rotateAction.action.ReadValue<Vector2>();
            float rotation = input.x * rotationSpeed * Time.deltaTime;
            alignmentObject.transform.Rotate(Vector3.up, rotation);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        turnProviders.SetActive(false);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        turnProviders.SetActive(true);
    }
}