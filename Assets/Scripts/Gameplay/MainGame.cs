using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [ContextMenuItem("Find data", nameof(GetLevelData))] public LevelData LevelData;
    [SerializeField] string LevelDataBaseName;

    [Header("Generate Grid")]
    [SerializeField] Transform grid;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] Transform waypoint;
    public Transform[] crowdWaypoints;

    [Header("Turns")]
    public Turn CurrentTurn;
    public float waitTurn = 4f;
    public int TurnCount;

    public Dictionary<string, Building> AllBuildings = new Dictionary<string, Building>();
    public GameObject BuildingPrefab { get; set; }

    void GetLevelData()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        string path = LevelDataBaseName + i;

        if (Resources.Load<LevelData>(path))
            LevelData = Resources.Load<LevelData>(path);
        else
            Debug.LogError("Didn't find the asset at path : " + path);
    }

    private void OnDrawGizmos()
    {
        if (LevelData && LevelData.gridCell && grid)
        {
            Gizmos.color = Color.green;
            Vector3 size = new Vector3(gridSize.x * LevelData.gridCell.transform.localScale.x / 2 - LevelData.gridCell.transform.localScale.x / 2, 0, gridSize.y * LevelData.gridCell.transform.localScale.z / 2 - LevelData.gridCell.transform.localScale.z / 2);
            Gizmos.DrawWireCube(grid.position + size, new Vector3(gridSize.x * LevelData.gridCell.transform.localScale.x, 1, gridSize.y * LevelData.gridCell.transform.localScale.z));
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (!LevelData)
            GetLevelData();

        TurnCount = LevelData.turnStartCount;
        mainCam = Camera.main;
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
    }

    private IEnumerator Start()
    {
        CreateGrid();
        EventManager.Instance.onBuildingBuilt += AddBuilding;
        EventManager.Instance.onCost?.Invoke();
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.onNewTurn?.Invoke();
        EventManager.Instance.onNewAction?.Invoke();
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
    }

    public void AddBuilding(Building build)
    {
        AllBuildings.Add(build.name, build);
    }

    public void NewTurn()
    {
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
        TurnCount--;
        UIManager.Instance.UpdateUI();
        StartCoroutine(InvokeNewTurn());
    }

    IEnumerator InvokeNewTurn()
    {
        yield return new WaitForSeconds(waitTurn);
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
        foreach (var item in AllBuildings)
        {
            yield return new WaitForSeconds(1f);
            item.Value.Effect();
        }

        yield return new WaitForSeconds(1f);
        ResourceManager.Instance.PayDay();

        yield return new WaitForSeconds(2f);
        if (TurnCount > 0)
        {
            if (CurrentTurn.myEvent)
                EventManager.Instance.onNewRandomEvent?.Invoke(CurrentTurn.myEvent);
            else
                EventManager.Instance.onNewAction?.Invoke();
        }
        else
            EventManager.Instance.onEndGame?.Invoke(LevelData.MainGoal.Check());
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

        int xScale = gridSize.x * (int)LevelData.gridCell.transform.localScale.x;
        int zScale = gridSize.y * (int)LevelData.gridCell.transform.localScale.z;
        for (int i = 0; i < xScale; i += (int)LevelData.gridCell.transform.localScale.x)
        {
            for (int j = 0; j < zScale; j += (int)LevelData.gridCell.transform.localScale.z)
            {
                GameObject newCell = Instantiate(LevelData.gridCell, grid);
                newCell.transform.localPosition = new Vector3(i, 0, j);

                if (i < xScale - (int)LevelData.gridCell.transform.localScale.x
                    && j < zScale - (int)LevelData.gridCell.transform.localScale.z)
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
