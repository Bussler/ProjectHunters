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

    public List<GameObject>[] grid;

    public int cellsPerRow;

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
    }

    public int AddObject(GameObject obj)
    {
        int cellIndex = GetCellIndex(obj.transform.position);
        grid[cellIndex].Add(obj);
        return cellIndex;
    }

    public void AddObject(GameObject obj, int cellIndex)
    {
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
    public List<GameObject> GetNearbyObjects(GameObject obj)
    {
        int cellIndex = GetCellIndex(obj.transform.position);
        return grid[cellIndex];
    }

    public List<GameObject> GetNearbyObjects(int cellIndex)
    {
        return grid[cellIndex];
    }
}
