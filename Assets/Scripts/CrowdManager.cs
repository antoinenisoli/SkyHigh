#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [SerializeField] CrowdMember memberPrefab;
    [SerializeField] [Range(0, 1000)] int maxMemberAmount = 50;
    [Tooltip("The position crowd members go to on startup, and when they are not used/inactive.")]
    [SerializeField] Vector3 inactivePos = Vector3.up * -75;

    [Tooltip("How long to wait before \"spawning\" a member at a spawn position")]
    [SerializeField] float delayBetweenMemberSpawns = 1;

    [Tooltip("X = Minimum number of waypoints between the start and end of this member's route. Y = Maximum. Defaults to (1, 3).")]
    [SerializeField] Vector2Int routeMidpointRange = Vector2Int.one * -1;

    [Header("Member Collections")]
    [SerializeField] GameObject memberContainer;
    [SerializeField] CrowdMember[] members;
    [SerializeField] List<Transform> memberSpawnPositions = new List<Transform>();
    Dictionary<Transform, float> spawnCooldowns = new Dictionary<Transform, float>();

#if UNITY_EDITOR
    [Header("! Editor Only !")]
    [SerializeField] bool showCooldowns = false;
    [SerializeField] float[] editor_SpawnCooldowns;
#endif

    private void Start()
    {
        //On startup, init the members array to have the amount of members specified,
        members = new CrowdMember[maxMemberAmount];

        //Sanitize the given route length range; default if less than 0,
        routeMidpointRange = routeMidpointRange.x < 0 || routeMidpointRange.y < 0 ? new Vector2Int(1, 3) : routeMidpointRange;
        //Swap if x is bigger than y
        if (routeMidpointRange.x > routeMidpointRange.y)
        {
            int swap = routeMidpointRange.x;
            routeMidpointRange.x = routeMidpointRange.y;
            routeMidpointRange.y = swap;
        }

        //Create an empty game object to put all the member's in if one wasn't designated, for better inspector organization
        if (!memberContainer) 
            memberContainer = new GameObject("Crowd Members"); 

        for (int i = 0; i < maxMemberAmount; i++)
        {
            //Then make a new member for each element of the array.
            members[i] = Instantiate(memberPrefab, inactivePos, Quaternion.identity, memberContainer.transform);
            members[i].name = members[i].name.Replace("(Clone)", $"({i})");
        }

        EventManager.Instance.onBuildingBuilt += AddSpawnPosition;
        EventManager.Instance.onBuildingDestroyed += RemoveSpawnPosition;
        EventManager.Instance.onCrowdMemberReachedEnd += StartResetCrowdMember;
    }

    private void Update()
    {
        foreach (Transform spawnPosition in memberSpawnPositions)
        {
            spawnCooldowns[spawnPosition] -= Time.deltaTime;
        }

#if UNITY_EDITOR
        if (showCooldowns) { editor_SpawnCooldowns = spawnCooldowns.Values.ToArray(); }
#endif
    }

    private void AddSpawnPosition(Building posObject)
    {
        memberSpawnPositions.Add(posObject.CrowdEntryPosition);
        spawnCooldowns.Add(posObject.CrowdEntryPosition, delayBetweenMemberSpawns);
    }

    private void RemoveSpawnPosition(Building posObject)
    {
        memberSpawnPositions.Remove(posObject.CrowdEntryPosition);
        spawnCooldowns.Remove(posObject.CrowdEntryPosition);
    }

    private void StartResetCrowdMember(CrowdMember member) { StartCoroutine(ResetCrowdMember(member)); }

    private IEnumerator ResetCrowdMember(CrowdMember member)
    {
        //Wait until there is at least one spawn position.
        yield return new WaitUntil(() => memberSpawnPositions.Count > 0);

        //Generate a route with a start and end(the +2 at the end), plus a random number of mid points
        var whywontthisarrayinit = Random.Range(routeMidpointRange.x, routeMidpointRange.y) + 2;
        Transform[] notCursedArray = new Transform[whywontthisarrayinit];
        Transform[] newRoute = new Transform[whywontthisarrayinit];
        //Set the start and end to random member spawn positions
        newRoute[0] = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];
        newRoute[newRoute.Length - 1] = memberSpawnPositions[Random.Range(0, memberSpawnPositions.Count)];

        int lastIndex = -1;
        for (int i = 1; i < newRoute.Length - 1; i++)
        {
            int waypointIndex = lastIndex;
            while (waypointIndex == lastIndex) 
                waypointIndex = Random.Range(0, MainGame.Instance.crowdWaypoints.Length); 

            newRoute[i] = MainGame.Instance.crowdWaypoints[waypointIndex];
            lastIndex = waypointIndex;
        }

        ////Now that the route is constructed, wait until the cooldown on the start location is up
        if (spawnCooldowns.ContainsKey(newRoute[0]) && newRoute[0] != null)
            yield return new WaitUntil(() => spawnCooldowns[newRoute[0]] <= 0);
        else
            yield return null;

        //Start the cooldown at this spawn position and reset the member's route in preparation for them to set out
        spawnCooldowns[newRoute[0]] = delayBetweenMemberSpawns;
        member.ResetRoute(newRoute);
    }
}