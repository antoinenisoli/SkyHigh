using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ModeType
{
    Building,
    ActionChoose,
    Store,
    None,
}

public class MainGame : MonoBehaviour
{
    public static MainGame Instance;
    Camera mainCam;
    public Goal MainGoal;

    [Header("Generate Grid")]
    [SerializeField] Transform grid;
    [SerializeField] GameObject cell;
    [SerializeField] Vector2Int gridSize;

    [Header("Turns")]
    [SerializeField] float waitTurn = 4f;
    public int turnCount = 10;
    public GameObject BuildingPrefab { get; set; }
    public Turn CurrentTurn;

    private void OnDrawGizmos()
    {
        if (cell && grid)
        {
            Gizmos.color = Color.green;
            Vector3 size = new Vector3(gridSize.x * cell.transform.localScale.x / 2 - cell.transform.localScale.x / 2, 0, gridSize.y * cell.transform.localScale.z / 2 - cell.transform.localScale.z / 2);
            Gizmos.DrawWireCube(grid.position + size, new Vector3(gridSize.x * cell.transform.localScale.x, 1, gridSize.y * cell.transform.localScale.z));
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mainCam = Camera.main;
        CurrentTurn = new Turn(null, 3, ModeType.None);
    }

    private IEnumerator Start()
    {
        CreateGrid();
        EventManager.Instance.onCost.Invoke();
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.onNewTurn.Invoke();
        EventManager.Instance.onNewAction.Invoke();
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
    }

    public void NewTurn()
    {
        CurrentTurn = new Turn(null, 3, ModeType.None);
        turnCount--;
        
        if (turnCount > 0)
        {
            StartCoroutine(InvokeNewTurn());
        }
        else 
        {
            if (MainGoal.Check())
                print("win");
            else
                print("loose");
        }
    }

    IEnumerator InvokeNewTurn()
    {
        yield return new WaitForSeconds(waitTurn);
        CurrentTurn = new Turn(null, 3, ModeType.None);
        EventManager.Instance.onCost.Invoke();
        EventManager.Instance.onNewAction.Invoke();
        Building[] buildings = FindObjectsOfType<Building>();
        foreach (var item in buildings)
        {
            item.Effect();
        }
    }

    public void SetMode(ModeType newMode)
    {
        CurrentTurn.currentMode = newMode;
    }

    public void CreateGrid()
    {
        int xScale = gridSize.x * 5;
        int zScale = gridSize.y * 5;
        for (int i = 0; i < xScale; i += (int)cell.transform.localScale.x)
        {
            for (int j = 0; j < zScale; j += (int)cell.transform.localScale.z)
            {
                GameObject newCell = Instantiate(cell, grid);
                newCell.transform.localPosition = new Vector3(i, 0, j);
            }
        }
    }

    public void PlaceBuilding(Vector3 position)
    {
        GameObject newBuilding = Instantiate(BuildingPrefab, position - Vector3.up * 2, BuildingPrefab.transform.rotation);
        float duration = 2;
        newBuilding.transform.DOMoveY(position.y, duration);
        mainCam.transform.DOComplete();
        mainCam.transform.DOShakePosition(duration, 0.3f, 90);
        BuildingPrefab = null;
        EventManager.Instance.onNewAction.Invoke();
    }
}
