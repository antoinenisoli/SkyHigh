using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainGame : MonoBehaviour
{
    public static MainGame Instance;

    [Header("Generate Grid")]
    [SerializeField] Transform grid;
    [SerializeField] GameObject cell;
    [SerializeField] Vector2 gridSize;

    [Header("Turns")]
    [SerializeField] GameObject building;
    public Turn CurrentTurn;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(gridSize.x / 2 - cell.transform.localScale.x/2, 0, gridSize.y / 2 - cell.transform.localScale.z/2);
        Gizmos.DrawWireCube(grid.position + size, new Vector3(gridSize.x, 1, gridSize.y));
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CurrentTurn = new Turn(null, 5, "Build mode");
    }

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        for (int i = 0; i < gridSize.x; i += (int)cell.transform.localScale.x)
        {
            for (int j = 0; j < gridSize.y; j += (int)cell.transform.localScale.z)
            {
                GameObject newCell = Instantiate(cell, grid);
                newCell.transform.localPosition = new Vector3(i, 0, j);
            }
        }
    }

    public void PlaceBuilding(Vector3 position)
    {
        GameObject newBuilding = Instantiate(building, position - Vector3.up * 2, building.transform.rotation);
        newBuilding.transform.DOMoveY(position.y, 2);
        ResourceManager.Instance.Cost(50, 0);
    }
}
