using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTargetSpawner : SingleDirectionSpawner
{
    [SerializeField] GameObject LastTargetToTriggerReload;

    protected override void StartSpawningProcess()
    {
        if (isObjectSpawned || !allowToSpawn) return;
        ResetSpawnStats();

        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            if (i == 0)
            {
                SpawnObject(prefabToSpawn, transform.position);
                continue;
            }

            Vector3 _previousSpawnedObjectPosition = spawnedObjects[i - 1].transform.position;
            Vector3 _currentObjectToSpawnPosition = _previousSpawnedObjectPosition + spawnOffset;

            if (i == numberOfPrefabsToSpawn-1)
            {
                SpawnObject(LastTargetToTriggerReload,_currentObjectToSpawnPosition);
                continue;
            }
            SpawnObject(prefabToSpawn, _currentObjectToSpawnPosition);
        }
    }

    
}
