using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Class to manage the space partitioning of the game, organizing the game objects in a grid
// Implements Singleton pattern
public class SpacePartitionManager : MonoBehaviour
{
    public static SpacePartitionManager Instance;

    public int gridWidth = 100;
    public int gridHeight = 100;
    public int cellSize = 1;
    public int enemyBatches = 10;

    public List<GameObject>[] grid;

    public int cellsPerRow;

    public List<GameObject>[] batches = new List<GameObject>[50];
    private float runLogicTimer = 0f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        grid = new List<GameObject>[(gridWidth * gridHeight) / (cellSize*cellSize)];

        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new List<GameObject>();
        }

        cellsPerRow = gridWidth / cellSize; //assume square grid

        batches = new List<GameObject>[enemyBatches];
        for (int i = 0; i < batches.Length; i++)
        {
            batches[i] = new List<GameObject>();
        }
    }

    private void FixedUpdate()
    {
        runLogicTimer += Time.deltaTime;
        RunBatchLogic((int)(runLogicTimer * 50) % batches.Length);
    }

    private void RunBatchLogic(int batchID)
    {
        foreach (GameObject obj in batches[batchID])
        {
            if (obj != null)
            {
                MoveToPlayer movement = obj.GetComponent<MoveToPlayer>();
                if (movement != null)
                {
                    movement.RunLogic();
                }
            }
        }
    }
    public void AddObjectToBatch(GameObject obj)
    {
        int batchID = GetCellIndex(obj.transform.position) % batches.Length;
        batches[batchID].Add(obj);
    }

    public int AddObject(GameObject obj)
    {
        int cellIndex = GetCellIndex(obj.transform.position);

        if (cellIndex < 0 || cellIndex >= grid.Length -1)
        {
            Debug.Log("Object out of bounds " + obj.name);
            ObjectPoolManager.Instance.DespawnObject(obj); // Instead of destroy, deactivation in pool
            return -1;
        }

        grid[cellIndex].Add(obj);
        return cellIndex;
    }

    public void AddObject(GameObject obj, int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= grid.Length -1)
        {
            Debug.Log("Object out of bounds " + obj.name);
            ObjectPoolManager.Instance.DespawnObject(obj); // Instead of destroy, deactivation in pool
            return;
        }

        grid[cellIndex].Add(obj);
    }
    public int RemoveObject(GameObject obj)
    {
        int cellIndex = GetCellIndex(obj.transform.position);
        grid[cellIndex].Remove(obj);
        return cellIndex;
    }

    public void RemoveObject(GameObject obj, int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= grid.Length -1)
        {
            Debug.Log("Object out of bounds " + obj.name);
            ObjectPoolManager.Instance.DespawnObject(obj); // Instead of destroy, deactivation in pool
            return;
        }

        grid[cellIndex].Remove(obj);
    }

    // Calculate the index of the cell in which the object is located
    public int GetCellIndex(Vector3 position)
    {
        float adjustedX = position.x + gridWidth / 2;
        float adjustedY = position.y + gridHeight / 2;

        int x_index = (int)(adjustedX / cellSize);
        int y_index = (int)(adjustedY / cellSize);

        int t = x_index +y_index * cellsPerRow;

        return x_index + y_index * cellsPerRow;
    }

    // Get the list of objects in the cell where the object is located
    public List<GameObject> GetCellObjects(GameObject obj)
    {
        int cellIndex = GetCellIndex(obj.transform.position);
        return grid[cellIndex];
    }

    public List<GameObject> GetCellObjects(int cellIndex)
    {
        return grid[cellIndex];
    }

    // Return a list of cell IDs that are within the radius of the current cell ID
    public static List<int> GetNearbyCells(int currentCellID, int radius = 1)
    {
        List<int> cellIDs = new List<int>();

        int widthRange = Instance.gridWidth;
        int heightRange = Instance.gridHeight;
        int amountCells = Instance.grid.Length;

        for(int i_x = -radius; i_x <= radius; i_x++)
        {
            for (int i_y = -radius; i_y <= radius; i_y++)
            {
                int newGroup = currentCellID + i_x + i_y * widthRange;

                bool isWithinWidth = newGroup % widthRange >= 0 && newGroup % widthRange < widthRange;
                bool isWithinHeight = newGroup / widthRange >= 0 && newGroup / widthRange < heightRange;
                bool isWithinRange = isWithinWidth && isWithinHeight;

                bool isWithinPartitions = newGroup >= 0 && newGroup < amountCells;

                if (isWithinRange && isWithinPartitions)
                {
                    cellIDs.Add(newGroup);
                }
            }
        }

        return cellIDs;
    }

    // Get all objects in the grid groups referenced by the list of cell IDs
    public static List<GameObject> GetAllObjectsInGridGroups(List<int> spatialGroups)
    {
        List<GameObject> objects = new List<GameObject>();

        foreach (int cellID in spatialGroups)
        {
            objects.AddRange(Instance.grid[cellID]);
        }

        return objects;
    }
}
