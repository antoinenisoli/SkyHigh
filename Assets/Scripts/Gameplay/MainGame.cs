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
    [SerializeField] TurnActionButton buildingButton;
    [SerializeField] float cinematicOrthoSize = 15;
    float baseOrthoSize;

    [Header("Level")]
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
    public Camera MainCam { get; set; }

    void GetLevelData()
    {
        // look up in the LD folder and find the Level Data which correspond to this level, depending on its build index.
        int i = SceneManager.GetActiveScene().buildIndex;
        string path = LevelDataBaseName + i;

        if (Resources.Load<LevelData>(path))
            LevelData = Resources.Load<LevelData>(path);
        else
            Debug.LogError("Didn't find the level data at this path : " + path);
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
        MainCam = Camera.main;
        CurrentTurn = new Turn(3, ModeType.ChooseBasicAction);
    }

    private IEnumerator Start()
    {
        CreateGrid();
        baseOrthoSize = MainCam.orthographicSize;
        EventManager.Instance.onBuildingBuilt += AddBuilding;
        EventManager.Instance.onCost?.Invoke();
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.onNewAction?.Invoke();
        EventManager.Instance.onNewTurn.AddListener(NewTurn);
    }

    public void AddBuilding(Building build)
    {
        if (!AllBuildings.ContainsValue(build))
            AllBuildings.Add(build.name + build.GetInstanceID(), build);
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
        MainCam.DOOrthoSize(cinematicOrthoSize, waitTurn * 2).SetEase(Ease.InOutSine);
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
        NextTurn();
    }

    void NextTurn()
    {
        MainCam.DOOrthoSize(baseOrthoSize, waitTurn * 2).SetEase(Ease.InOutSine);
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
        // generate the main game board, depending on the size of one Cell prefab.
        // The waypoints are the inner corners of the grid; i.e., a grid smaller in each dimension by one.
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
        // do a camera shake by tween
        MainCam.transform.DOComplete();
        MainCam.transform.DOShakePosition(animDuration, shakeStrength, shakeVibration);
    }

    public void PlaceBuilding(Vector3 position, Cell cell)
    {
        CurrentTurn.ActionsCount -= buildingButton.pointCost;
        GameObject newBuilding = Instantiate(BuildingPrefab, position - Vector3.up * 4, BuildingPrefab.transform.rotation, grid);
        newBuilding.GetComponent<Building>().Build(cell);
        BuildingPrefab = null;
        EventManager.Instance.onNewAction.Invoke();
    }
}
