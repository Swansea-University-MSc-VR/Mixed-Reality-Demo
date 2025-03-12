using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.AI;
using EditorAttributes;


public class NavMeshAgentSpawner : MonoBehaviour
{
    public ARPlaneManager m_ARPlaneManager;
    public GameObject navMeshAgentPrefab;
    public Transform player;

    public void OnClickSpawnNavMeshAgent()
    {
        StartCoroutine(SpawnNavMeshAgent());
    }
    private IEnumerator SpawnNavMeshAgent()
    {
        if (m_ARPlaneManager.enabled == false)
        {
            TurnOnPlanes();
            yield return new WaitForSeconds(1f);
        }

        // get random point on navmesh
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 3f; // 5f is the spawn radius
        randomDirection.y = 0; // Keep it on the same vertical plane as the player
        Vector3 randomPosition = player.transform.position + randomDirection;
       
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, 3f, NavMesh.AllAreas);

        // Calculate direction from spawn position to player
        Vector3 directionToPlayer = player.transform.position - hit.position;
        directionToPlayer.y = 0; // Keep rotation only on the horizontal plane

        // Create rotation that looks in the direction of the player
        Quaternion rotationTowardPlayer = Quaternion.LookRotation(directionToPlayer);

        // Instantiate with the calculated rotation
        GameObject agent = Instantiate(navMeshAgentPrefab, hit.position, rotationTowardPlayer);

        yield return null;
    }

    private void TurnOnPlanes()
    {       
        m_ARPlaneManager.enabled = true;
    }

    [Button]
    public void TestNavMeshAgentSpawn()
    {
        StartCoroutine(SpawnNavMeshAgent());
    }
}