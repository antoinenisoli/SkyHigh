using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ModeType
{
    ChooseBasicAction,
    ExecuteTurn,
    Building,
    Charity,
    Store,
}

public class MainGame : MonoBehaviour
{
    public static MainGame Instance;
    Camera mainCam;
    [SerializeField] LevelData data;
    public Goal MainGoal;

    [Header("Charity Actions")]
    public List<CharityAction> allCharityActions = new List<CharityAction>();

    [Header("Generate Grid")]
    [SerializeField] Transform grid;
    [SerializeField] GameObject cell;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Transform waypoint;
    public Transform[] crowdWaypoints;

    [Header("Turns")]
    [SerializeField] float waitTurn = 4f;
    public int turnCount = 10;
    public GameObject BuildingPrefab { get; set; }
    public Turn CurrentTurn;

    [Header("Random events")]
    public List<RandomEvent> allRandomEvents = new List<RandomEvent>();

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
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
    }

    private IEnumerator Start()
    {
        CreateGrid();
        EventManager.Instance.onCost?.Invoke();
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.onNewTurn?.Invoke();
        EventManager.Instance.onNewAction?.Invoke();
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
    }

    public void NewTurn()
    {
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
        turnCount--;
        UIManager.Instance.UpdateUI();
        StartCoroutine(InvokeNewTurn());
    }

    IEnumerator InvokeNewTurn()
    {
        yield return new WaitForSeconds(waitTurn);
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
        Building[] buildings = FindObjectsOfType<Building>();
        foreach (var item in buildings)
        {
            yield return new WaitForSeconds(1f);
            item.Effect();
        }

        yield return new WaitForSeconds(1f);
        ResourceManager.Instance.PayDay();

        yield return new WaitForSeconds(2f);
        if (turnCount > 0)
        {
            if (CurrentTurn.myEvent)
                EventManager.Instance.onNewRandomEvent?.Invoke(CurrentTurn.myEvent);
            else
                EventManager.Instance.onNewAction?.Invoke();
        }
        else
            EventManager.Instance.onEndGame?.Invoke(MainGoal.Check());
    }

    public void SetMode(ModeType newMode)
    {
        CurrentTurn.currentMode = newMode;
    }

    public void CreateGrid()
    {
        //The waypoints are the inner corners of the grid; i.e., a grid smaller in each dimension by one
        crowdWaypoints = new Transform[(gridSize.x - 1) * (gridSize.y - 1)];
        int waypointCount = 0;

        int xScale = gridSize.x * (int)cell.transform.localScale.x;
        int zScale = gridSize.y * (int)cell.transform.localScale.z;
        for (int i = 0; i < xScale; i += (int)cell.transform.localScale.x)
        {
            for (int j = 0; j < zScale; j += (int)cell.transform.localScale.z)
            {
                GameObject newCell = Instantiate(cell, grid);
                newCell.transform.localPosition = new Vector3(i, 0, j);

                if (i < xScale - (int)cell.transform.localScale.x
                    && j < zScale - (int)cell.transform.localScale.z)
                {
                    crowdWaypoints[waypointCount] = Instantiate(waypoint, newCell.transform);
                    crowdWaypoints[waypointCount].localPosition = Vector3.one * 0.5f;
                    waypointCount++;
                }
            }
        }
    }

    public void ShakeCamera(float animDuration = 2f, float shakeStrength = 0.3f, int shakeVibration = 10)
    {
        mainCam.transform.DOComplete();
        mainCam.transform.DOShakePosition(animDuration, shakeStrength, shakeVibration);
    }

    public void PlaceBuilding(Vector3 position)
    {
        GameObject newBuilding = Instantiate(BuildingPrefab, position - Vector3.up * 4, BuildingPrefab.transform.rotation, grid);
        newBuilding.GetComponent<Building>().Build(position);
        BuildingPrefab = null;
        EventManager.Instance.onNewAction.Invoke();
    }
}
