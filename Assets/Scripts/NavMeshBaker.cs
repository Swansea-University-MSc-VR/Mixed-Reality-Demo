using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshBaker : MonoBehaviour
{
    public float updateInterval;
    private float timeSinceLastUpdate = 0.0f
        ;
    public TextMeshProUGUI planeCounterText;  
    public Transform player;
    public ARPlaneManager m_ARPlaneManager;

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastUpdate > updateInterval && m_ARPlaneManager.enabled)
        {
            timeSinceLastUpdate = 0.0f;
            LookForARPlane();
        }
        else
        {
            timeSinceLastUpdate += Time.deltaTime;
        }
    }

    public void LookForARPlane()
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            if (plane.transform.position.y < player.position.y)
            {
                plane.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }

        if (m_ARPlaneManager.trackables.count > 0)
        {
            planeCounterText.text = m_ARPlaneManager.trackables.count + " ARPlanes found";
        }
        else
        {
            planeCounterText.text = "No ARPlane found";
        }
    }
}