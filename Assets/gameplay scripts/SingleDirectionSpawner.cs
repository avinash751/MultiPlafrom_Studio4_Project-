using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SingleDirectionSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] protected int numberOfPrefabsToSpawn;
    [SerializeField] protected Vector3 spawnOffset;
    protected bool isObjectSpawned = false;
    [SerializeField] protected bool infiniteSpawning = false;
    [SerializeField] protected float timeBetweenSpawns = 1f;

    [SerializeField] protected List<GameObject> spawnedObjects = new List<GameObject>();
    [SerializeField] protected bool allowToSpawn ;


    private void OnEnable()
    {
        GameManager.GameIsInPlayMode += AllowSpawning;
        GameManager.GameIsOver += AllowSpawning;
        StartSpawningProcess();
    }

    private void OnDisable()
    {
        GameManager.GameIsInPlayMode -= AllowSpawning;
        GameManager.GameIsOver -= AllowSpawning;
    }
    void Start()
    {
        spawnedObjects = new List<GameObject>(numberOfPrefabsToSpawn);
    }

    private void Update()
    {
        StartSpawningInfinitely();
    }

    protected virtual void StartSpawningProcess()
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
            SpawnObject(prefabToSpawn, _currentObjectToSpawnPosition);
        }
    }

    protected void SpawnObject(GameObject _objectToSpawn, Vector3 _positionToSpawn)
    {
        var _newObjectToSpawn = Instantiate(_objectToSpawn, _positionToSpawn, Quaternion.identity);
        spawnedObjects.Add(_newObjectToSpawn);
        _newObjectToSpawn.transform.SetParent(transform);
    }

    protected void ResetSpawnStats()
    {
        isObjectSpawned = false;
        spawnedObjects.Clear();
    }

    void StartSpawningInfinitely()
    {
        if (!infiniteSpawning || !allowToSpawn) return;

        if (spawnedObjects.Count == numberOfPrefabsToSpawn)
        {
            ResetSpawnStats();
            Invoke(nameof(StartSpawningProcess), timeBetweenSpawns);
            return;
        }
    }

    void AllowSpawning(bool _enable)
    {
        gameObject.SetActive(false);
        allowToSpawn = _enable;
        gameObject.SetActive(true);
        // this is to make sure that when the game is in play mode, the spawner will start spawning by enabling the object again
    }

    void DisableSpawning()
    {
        allowToSpawn = false;
    }
}
