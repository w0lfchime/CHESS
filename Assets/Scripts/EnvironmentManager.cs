using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Environment Settings")]
    public GameObject[] environmentPrefabs;
    public Color[] environmentBackgroundColors;

    [Header("Spawn Settings")]
    public Transform environmentParent;
    public Camera mainCamera;

    private GameObject currentEnvironment;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (environmentPrefabs.Length > 0)
        {
            SpawnEnvironmentAtIndex(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnEnvironmentAtIndex(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnEnvironmentAtIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnEnvironmentAtIndex(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnEnvironmentAtIndex(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpawnEnvironmentAtIndex(4);
        }
    }

    public void SpawnEnvironmentAtIndex(int index)
    {
        if (environmentPrefabs == null || environmentPrefabs.Length == 0)
        {
            Debug.LogWarning("EnvironmentManager: No environment prefabs assigned!");
            return;
        }

        if (index < 0 || index >= environmentPrefabs.Length)
        {
            Debug.LogWarning($"EnvironmentManager: Index {index} is out of range (0-{environmentPrefabs.Length - 1})");
            return;
        }

        GameObject prefabToSpawn = environmentPrefabs[index];
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"EnvironmentManager: Environment prefab at index {index} is null!");
            return;
        }

        ClearCurrentEnvironment();

        Transform parent = environmentParent != null ? environmentParent : transform;
        currentEnvironment = Instantiate(prefabToSpawn, parent.position, parent.rotation, parent);

        SetCameraBackgroundColor(index);

        Debug.Log($"EnvironmentManager: Spawned environment '{prefabToSpawn.name}' (Index: {index})");
    }

    private void SetCameraBackgroundColor(int index)
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("EnvironmentManager: No camera assigned!");
            return;
        }

        if (environmentBackgroundColors != null && index < environmentBackgroundColors.Length)
        {
            mainCamera.backgroundColor = environmentBackgroundColors[index];
            Debug.Log($"EnvironmentManager: Set camera background to {environmentBackgroundColors[index]}");
        }
        else
        {
            Debug.LogWarning($"EnvironmentManager: No background color defined for environment {index}");
        }
    }

    public void ClearCurrentEnvironment()
    {
        if (currentEnvironment != null)
        {
            Destroy(currentEnvironment);
            currentEnvironment = null;
        }
    }

    public string GetCurrentEnvironmentName()
    {
        return currentEnvironment != null ? currentEnvironment.name : "None";
    }
}
