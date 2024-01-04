#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using static BouncBall;

public class BuildingGrid : MonoBehaviour
{
    private float timeSinceLastTurret;
    public float timeBetweenTurrets = 5f;

    private float timeSinceLastSphere;
    public float timeBetweenSphere = 10f;

    public Material newMaterial;

    
    public GameObject buildingPrefab;  
    public float gridSize = 0.5f;  
    public int gridRows = 100;  
    public int gridColumns = 100;  

    private GridType[,] grid;

    private GameObject[] buildingPrefabs;

    private NavMeshSurface navMeshSurface;

    private float timeSinceLastStar;

    public Claire claire;

    public float timeBetweenStars = 40f;

    public enum GridType
    {
        Empty,
        Building,
        Road,
        Star
    }

    void Start()
    {
        InitializeGrid();
        AssignBuildingPrefabs();
        GenerateBuildingGrid();

        timeSinceLastStar = 0f;

        GameObject navMeshObject = GameObject.Find("NavMesh");

        if (navMeshObject != null)
        {
            NavMeshSurface[] navMeshSurfaces = navMeshObject.GetComponents<NavMeshSurface>();
            if (navMeshSurface == null)
                Debug.Log(true);

            foreach (var navMeshSurface in navMeshSurfaces)
            {
                if (navMeshSurface != null)
                {
                    navMeshSurface.BuildNavMesh();
                }
            }
        }
        //SpawnClaire();
        SpawnTurretWithNavMeshAgent();
        SpawnSphereWithNavMeshAgent();
    }


    void SpawnTurretWithNavMeshAgent()
    {
        GameObject turretPrefab = Resources.Load<GameObject>("Robo's turret/Prefabs/Turret_basic_lvl1");  // Adjust the prefab name
        if (turretPrefab == null)
        {
            Debug.LogError("Turret prefab not found. Make sure it's in the Resources folder.");
            return;
        }
        Vector3 randomPosition = FindRandomPosition();

        GameObject turret = Instantiate(turretPrefab, randomPosition, Quaternion.identity);
        turret.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
        NavMeshAgent agent = turret.AddComponent<NavMeshAgent>();
        agent.agentTypeID = NavMesh.GetSettingsByIndex(0).agentTypeID; 
        agent.baseOffset = 0f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        agent.speed = 0.5f;
        agent.acceleration = 2f;
        agent.stoppingDistance = 3f;
        agent.height = 0f;

    }

    void SpawnSphereWithNavMeshAgent()
    {
        Vector3 randomPosition = FindRandomPosition();
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(randomPosition.x, 0.11f, randomPosition.z); // Set y-coordinate to 0.11
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        sphere.tag = "Ball";

    SphereCollider existingCollider = sphere.GetComponent<SphereCollider>();
    Debug.Log(existingCollider.enabled+"sphere");
    if (existingCollider != null)
    {
        existingCollider.enabled = true;
        existingCollider.isTrigger = true;
        existingCollider.radius = 0.5f;
        Debug.Log("sphereActivaed");
    }
    else
    {
        Debug.LogWarning("SphereCollider not found on the sphere.");
    }

    Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = newMaterial;
        }
        BouncBall sphereScript = sphere.AddComponent<BouncBall>();
        NavMeshAgent navMeshAgent = sphere.AddComponent<NavMeshAgent>();
        navMeshAgent.agentTypeID = NavMesh.GetSettingsByIndex(1).agentTypeID;

        navMeshAgent.baseOffset = 0.11f;
        navMeshAgent.speed = 1f;
        navMeshAgent.acceleration = 3f;
        navMeshAgent.stoppingDistance = 0f;
        navMeshAgent.height = 1f;
    }


    Vector3 FindRandomPosition()
    {
        float x = Random.Range(0, gridColumns) * gridSize;
        float z = Random.Range(0, gridRows) * gridSize;

        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));

        return new Vector3(x, y, z);
    }


    void AssignBuildingPrefabs()
    {
        string folderPath = "Assets/Buildings/";

        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath })
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();
        string[] prefabNames = { "house_2_v_15", "grocery_store", "house_2_v_28" };

        buildingPrefabs = new GameObject[prefabNames.Length];

        for (int i = 0; i < prefabNames.Length; i++)
        {
            buildingPrefabs[i] = Resources.Load<GameObject>("Buildings/" + prefabNames[i]);
            if (buildingPrefabs[i] == null)
            {
                Debug.LogError($"Prefab '{prefabNames[i]}' not found in Resources folder.");
            }
        }
    }

    void InitializeGrid()
    {
        grid = new GridType[gridRows, gridColumns];
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                grid[row, col] = GridType.Empty;
            }
        }
    }

    void GenerateBuildingGrid()
    {
        System.Random random = new System.Random();

        int houseFrequency = 1;
        int emptyColumns = 3;   

        for (int col = 0; col < gridColumns;)
        {
            for (int houseCount = 0; houseCount < houseFrequency && col < gridColumns; houseCount++, col++)
            {
                PlaceBuildingInColumn(col, random);
            }
            for (int emptyColCount = 0; emptyColCount < emptyColumns && col < gridColumns; emptyColCount++, col++)
            {
            }
        }

        GenerateStars();
        CreateHighWalls();
    }


    void GenerateStars()
    {
        System.Random random = new System.Random();
        List<Vector2Int> occupiedCells = new List<Vector2Int>();

        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                if ((grid[row, col] == GridType.Empty || grid[row,col] == GridType.Road) && !IsCellOccupied(occupiedCells, row, col))
                {
                    if (ShouldPlaceStar(random, row, col))
                    {
                        GameObject starPrefab = Resources.Load<GameObject>("AurynSky/Gems Ultimate Pack/Prefabs/HardStar");
                        Vector3 position = new Vector3(col * gridSize, 0.5f, row * gridSize);
                        GameObject star = Instantiate(starPrefab, position, Quaternion.identity);

                        SphereCollider sphereCollider = star.AddComponent<SphereCollider>();
                        sphereCollider.radius = 1.5f; 
                        sphereCollider.isTrigger = true;

                        StarCollision starScript = star.AddComponent<StarCollision>();

                        float starScale = 0.2f; 
                        star.transform.localScale = new Vector3(starScale, starScale, starScale);

                        star.transform.Rotate(Vector3.right, 90f);

                        star.tag = "Star";

                        grid[row, col] = GridType.Star;
                        AnimationScript animationScript = star.GetComponent<AnimationScript>();
                        if (animationScript != null)
                        {
                            animationScript.enabled = false;
                        }

                        occupiedCells.Add(new Vector2Int(col, row));
                    }
                }
            }
        }
    }

    bool ShouldPlaceStar(System.Random random, int row, int col)
    {
        float baseProbability = 0.03f; 

        int neighborhoodRadius = 3; 
        for (int i = row - neighborhoodRadius; i <= row + neighborhoodRadius; i++)
        {
            for (int j = col - neighborhoodRadius; j <= col + neighborhoodRadius; j++)
            {
                if (i >= 0 && i < gridRows && j >= 0 && j < gridColumns && grid[i, j] == GridType.Star)
                {
                    baseProbability *= 0.8f;
                }
            }
        }
        return random.NextDouble() < baseProbability;
    }

    bool IsCellOccupied(List<Vector2Int> occupiedCells, int row, int col)
    {
        return occupiedCells.Contains(new Vector2Int(col, row));
    }


    void CreateHighWalls()
    {
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                bool isOnBoundary = row == 0 || col == 0 || row == gridRows - 1 || col == gridColumns - 1;

                if (isOnBoundary)
                {
                    GameObject highWall = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    highWall.transform.localScale = new Vector3(gridSize, 100, gridSize);
                    highWall.transform.position = new Vector3(col * gridSize + gridSize / 2f, 0, row * gridSize + gridSize / 2f);
                }
            }
        }
    }


    void PlaceBuildingInColumn(int col, System.Random random)
    {
        for (int row = 0; row < gridRows; row++)
        {
            bool isOnBoundary = row == 0 || col == 0 || row == gridRows - 1 || col == gridColumns - 1;

            if (row>1 & !isOnBoundary && random.NextDouble() < 0.4)
            {
                GameObject selectedBuildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                Vector3 position = new Vector3(col * gridSize, 0f, row * gridSize);
                GameObject building = Instantiate(selectedBuildingPrefab, position, Quaternion.identity);

                AssignMeshColliderWithFilter(building, selectedBuildingPrefab.name);

                MarkGridOccupied(row, col, building.GetComponent<Collider>().bounds.size, GridType.Building);
            }
        }
    }


    void AssignMeshColliderWithFilter(GameObject obj, string filterName)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshCollider.convex = true;
        }
        else
        {
            Debug.LogWarning($"MeshFilter not found on object {obj.name}. MeshCollider not assigned.");
        }
    }

    Bounds GetPrefabBounds(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }


void MarkGridOccupied(int startRow, int startCol, Vector3 buildingSize, GridType type)
    {
        int cellsCoveredRow = Mathf.CeilToInt(buildingSize.z / gridSize);
        int cellsCoveredCol = Mathf.CeilToInt(buildingSize.x / gridSize);

        for (int row = startRow; row < startRow + cellsCoveredRow; row++)
        {
            for (int col = startCol; col < startCol + cellsCoveredCol; col++)
            {
                if (row < gridRows && col < gridColumns)
                {
                    grid[row, col] = type;
                    MarkSurroundingCells(row, col, GridType.Road);
                }
            }
        }
    }

    void MarkSurroundingCells(int centerRow, int centerCol, GridType type)
    {
        for (int row = centerRow - 1; row <= centerRow + 1; row++)
        {
            for (int col = centerCol - 1; col <= centerCol + 1; col++)
            {
                if (row >= 0 && row < gridRows && col >= 0 && col < gridColumns && grid[row, col] == GridType.Empty)
                {
                    grid[row, col] = type;
                }
            }
        }
    }

    
    void PrintGrid()
    {
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                switch (grid[row, col])
                {
                    case GridType.Empty:
                        Debug.Log($"Empty at [{row}, {col}]");
                        break;
                    case GridType.Building:
                        Debug.Log($"Building at [{row}, {col}]");
                        break;
                    case GridType.Road:
                        Debug.Log($"Road at [{row}, {col}]");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void SpawnNewStar()
    {
        Vector3 randomPosition = FindRandomEmptyPosition();

        if (randomPosition != Vector3.zero)
        {
            GameObject starPrefab = Resources.Load<GameObject>("AurynSky/Gems Ultimate Pack/Prefabs/HardStar");
            GameObject newStar = Instantiate(starPrefab, randomPosition, Quaternion.identity);

            SphereCollider sphereCollider = newStar.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.5f; // Adjust the radius as needed
            sphereCollider.isTrigger = true;

            StarCollision starScript = newStar.AddComponent<StarCollision>();

            float starScale = 0.2f; 
            newStar.transform.localScale = new Vector3(starScale, starScale, starScale);

            newStar.transform.Rotate(Vector3.right, 90f);

            newStar.tag = "Star";

            
            int col = Mathf.FloorToInt(randomPosition.x / gridSize);
            int row = Mathf.FloorToInt(randomPosition.z / gridSize);
            grid[row, col] = GridType.Star;
            AnimationScript animationScript = newStar.GetComponent<AnimationScript>();
            if (animationScript != null)
            {
                animationScript.enabled = false;
            }
        }
    }

    private Vector3 FindRandomEmptyPosition()
    {
        int attempts = 0;
        int maxAttempts = 100; 

        while (attempts < maxAttempts)
        {
            
            Vector3 randomPosition = FindRandomPosition();

            
            int col = Mathf.FloorToInt(randomPosition.x / gridSize);
            int row = Mathf.FloorToInt(randomPosition.z / gridSize);

            
            if (row >= 0 && row < gridRows && col >= 0 && col < gridColumns && grid[row, col] == GridType.Empty)
            {
                return randomPosition; 
            }

            attempts++;
        }

        Debug.LogWarning("Unable to find a valid empty position for the new star.");
        return Vector3.zero; 
    }


    private void Update()
    {
        timeSinceLastTurret += Time.deltaTime;
        if (timeSinceLastTurret >= timeBetweenTurrets)
        {
            SpawnTurretWithNavMeshAgent();
            timeSinceLastTurret = 0f;
        }

         timeSinceLastSphere += Time.deltaTime;
        if (timeSinceLastSphere >= timeBetweenSphere)
        {
            SpawnSphereWithNavMeshAgent();
            timeSinceLastSphere = 0f;
        }
        timeSinceLastStar += Time.deltaTime;
        if (timeSinceLastStar >= timeBetweenStars)
        {
            SpawnNewStar();
            timeSinceLastStar = 0f;
        }
    }


}